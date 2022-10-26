import {  UserDefinedFunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from "./language-helpers";

export function wrapVBExpression(expression : string) {
    return `
    Imports System
    Imports System.Collections
    Imports System.Collections.Generic
    Imports System.Linq

    Module Program
        ${ReadyMadeFunctionsPlaceholder}

        ${UserDefinedFunctionPlaceholder}

        Private Function Display(obj As Object) As String

            If (obj Is Nothing) Then
                Return Nothing
            End If


            If (TypeOf obj Is String) Then
                Return $"{obj}"
            End If

            If (TypeOf obj Is Boolean) Then
                If (DirectCast(obj, Boolean)) Then
                    Return "True"
                Else
                    Return "False"
                End If
            End If

            If (TypeOf obj Is IEnumerable) Then

                Dim s = (DirectCast(obj, IEnumerable).Cast(Of Object)().Select(Function(o) Display(o)))
                Return String.Join(","c, s)
            End If

            Return obj.ToString()
        End Function

        Sub Main(args As String())
            System.Console.WriteLine(Display(${(expression)}))
        End Sub
    End Module
    `;
}

export function wrapVBFunctions(userDefinedFunction : string) {
    return `
    Imports System
    Imports System.Linq
    Imports System.Collections
    Imports System.Collections.Generic

    
    Module Program
        ${ReadyMadeFunctionsPlaceholder}

        ${userDefinedFunction}

        Sub Main(args As String())
            
        End Sub
    End Module
    `;
}

export function wrapVBTests(tests : string) {
    return `
    Imports System
    Imports System.Collections
    Imports System.Collections.Generic
    Imports System.IO
    Imports System.Linq
    Imports System.Reflection
    Imports System.Text
    
    Module Program
    
        Dim fail = "Test failed calling "
    
        Function ArgString(ParamArray arguments As Object()) As String
            Return arguments.Aggregate("", Function(s, a) s + Display(a) + ", ").TrimEnd(" "c, ","c)
        End Function
    
        Sub TestFunction(functionName As String, expected As Object, actual As Object, ParamArray args As Object())   
            If (Not Display(actual).Equals(Display(expected))) Then
                Console.WriteLine(fail + $"{functionName}({ArgString(args)}) Expected: {Display(expected)}  Actual: {Display(actual)}")
                Throw New TestFailure()
            End If    
        End Sub

        Function FailMessage(functionName As String, ByVal expected As Object, ByVal actual As Object, ParamArray args As Object()) As String
            Return $" Calling {functionName}({ArgString(args)}) Expected: {Display(expected)} Actual: {actual}"
        End Function
  
        Sub AssertTrue(functionName As String, args As String(), actual As Boolean, message As String)    
            If (Not actual) Then
                Console.WriteLine(fail + $"{functionName}({ArgString(args)}) {message}")
                Throw New TestFailure()
            End If  
        End Sub
    
        Dim _allTestsPassed = "All tests passed."
    
        Sub AllTestsPassed()
            Console.Write(_allTestsPassed)
        End Sub
    
        Public Class TestFailure
            Inherits Exception
        End Class
    
    
        Private Function Display(obj As Object) As String
    
            If (obj Is Nothing) Then
                Return Nothing
            End If
    
    
            If (TypeOf obj Is String) Then
                Return $"{obj}"
            End If
    
            If (TypeOf obj Is Boolean) Then
                If (DirectCast(obj, Boolean)) Then
                    Return "True"
                Else
                    Return "False"
                End If
            End If
    
            If (TypeOf obj Is IEnumerable) Then
    
                Dim s = (DirectCast(obj, IEnumerable).Cast(Of Object)().Select(Function(o) Display(o)))
                Return String.Join(","c, s)
            End If
    
            Return obj.ToString()
        End Function

        ${ReadyMadeFunctionsPlaceholder}

        ${UserDefinedFunctionPlaceholder}

        ${tests}
    
        Sub Main(args As String())
            RunTests()
        End Sub
    End Module
    `;
}