Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Text

Namespace MetalUp.Express

    Module Program
        Public Sub Main()
            Console.OutputEncoding = Encoding.UTF8
            Console.WriteLine(Display(""))
        End Sub

#Region "<StudentCode>"
        Public Function IsPrime(ByVal n As Integer) As Boolean
            Return Not Enumerable.Range(2, n \ 2 - 1).Any(Function(f) IsFactor(f, n))
        End Function
#End Region

#Region "<Helpers>"
        Public Function Display(ByVal obj As Object) As String
            If obj Is Nothing Then
                Return ""
            End If
            If TypeOf obj Is String Then
                Return $"{obj}"
            End If
            If TypeOf obj Is Boolean Then
                Return If(DirectCast(obj, Boolean), "true", "false")
            End If
            Dim type = obj.GetType()
            If type.IsGenericType Then
                If type.GetGenericTypeDefinition() Is GetType(ImmutableList(Of )) Then
                    Dim toDisplay = DirectCast(obj, IEnumerable).Cast(Of Object)().Select(Function(o) Display(o)).ToList()
                    Return $"{String.Join(","c, toDisplay)}"
                End If
                If type.GetGenericTypeDefinition() Is GetType(List(Of )) Then
                    Return "Result is an ordinary List. Use ImmutableList only."
                End If
            End If
            If TypeOf obj Is IEnumerable Then
                Return "Result is an IEnumerable. Convert to ImmutableList to display contents"
            End If
            Return obj.ToString()
        End Function


        Public Function ArgString(ParamArray arguments As Object()) As String
            Return arguments.Aggregate("", Function(s, a) s & Display(a) & ", ").TrimEnd(" "c, ","c)
        End Function

        Public Function FailMessage(functionName As String, ByVal expected As Object, ByVal actual As Object, ParamArray args As Object()) As String
            Return $"!!!Calling {functionName}({ArgString(args)}) Expected: {Display(expected)} Actual: {actual}~~~"
        End Function
#End Region

#Region "<HiddenCode>"
        Public Function IsFactor(ByVal f As Integer, ByVal n As Integer) As Boolean
            Return n Mod f = 0
        End Function
#End Region

    End Module

End Namespace
