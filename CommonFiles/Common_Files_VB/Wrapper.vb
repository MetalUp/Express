Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Math
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace MetalUp.Express

    Module Program
        Public Sub Main()
            Console.OutputEncoding = Encoding.UTF8
            Console.WriteLine(Display("<Expression>"))
        End Sub

        ' StudentCode - this comment is needed error line number offset
        '<StudentCode>

        '<HiddenCode>

    End Module

    '<Tests>

    Public Module Helpers

        Public Function Display(ByVal obj As Object) As String
            If obj Is Nothing Then
                Return ""
            End If
            If TypeOf obj Is String Then
                Return $"""{obj}"""
            End If
            If TypeOf obj Is Boolean Then
                Return If(DirectCast(obj, Boolean), "true", "false")
            End If
            If TypeOf obj Is ITuple Then
                Dim tuple = DirectCast(obj, ITuple)
                Return Enumerable.Range(0, tuple.Length).Aggregate("(", Function(s, i) s & Display(tuple(i)) & (If(i < (tuple.Length - 1), ",", ")")))
            End If
            Dim type = obj.GetType()
            If type.IsGenericType AndAlso type.GetGenericTypeDefinition() Is GetType(List(Of )) Then
                Dim toDisplay = DirectCast(obj, IEnumerable).Cast(Of Object)().Select(Function(o) Display(o)).ToList()
                Return vbCrLf + $"{{{String.Join(", ", toDisplay)}}}"
            End If
            If TypeOf obj Is IEnumerable Then
                Return "Result is an IEnumerable. Convert to List to display contents"
            End If
            Return obj.ToString()
        End Function

        Public Function ArgString(ParamArray arguments As Object()) As String
            Return arguments.Aggregate("", Function(s, a) s & Display(a) & ", ").TrimEnd(" "c, ","c)
        End Function

        Public Function FailMessage(functionName As String, ByVal expected As Object, ByVal actual As Object, ParamArray args As Object()) As String
            Return $"xxxTest failed calling {functionName}({ArgString(args)}) Expected: {Display(expected)} Actual: {Display(actual)}xxx"
        End Function

        Public Function EqualIfRounded(expected As Double, actual As Double) As Boolean
            Dim places = expected.ToString().Length - expected.ToString().IndexOf(".") - 1
            Dim rounded = Math.Round(actual, places)
            Return expected = rounded
        End Function

        <Extension>
        Public Function SetItem(Of T)(ByVal list As List(Of T), ByVal i As Integer, ByVal value As T) As List(Of T)
            Return list.Take(i).Append(value).Concat(list.Skip(i + 1)).ToList()
        End Function

        <Extension>
        Public Function InsertItem(Of T)(ByVal list As List(Of T), ByVal i As Integer, ByVal value As T) As List(Of T)
            Return list.Take(i).Append(value).Concat(list.Skip(i)).ToList()
        End Function

        <Extension>
        Public Function RemoveItem(Of T)(ByVal list As List(Of T), ByVal i As Integer) As List(Of T)
            Return list.Take(i).Concat(list.Skip(i)).ToList()
        End Function

    End Module

End Namespace
