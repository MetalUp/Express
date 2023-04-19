Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace MetalUp.Express

    Module Program
        Public Sub Main()
            Console.OutputEncoding = Encoding.UTF8
            Console.WriteLine(Display(""))
        End Sub

        ' StudentCode - this comment is needed error line number offset
        Public Function IsPrime(ByVal n As Integer) As Boolean
            Return Not Enumerable.Range(2, n \ 2 - 1).Any(Function(f) IsFactor(f, n))
        End Function

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
            If TypeOf obj Is Tuple Then
                Dim toDisplay = DirectCast(obj, IEnumerable).Cast(Of Object)().Select(Function(o) Display(o)).ToList()
                Return $"({String.Join(","c, toDisplay)})"
            End If
            If type.IsGenericType Then
                If type.GetGenericTypeDefinition() Is GetType(List(Of )) Then
                    Dim toDisplay = DirectCast(obj, IEnumerable).Cast(Of Object)().Select(Function(o) Display(o)).ToList()
                    Return $"{{{String.Join(","c, toDisplay)}}}"
                End If
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

        Public Function QAFailMessage(ByVal wrongAnswers As String) As String
            Return $"xxxWrong or missing answer(s) to question number(s): {wrongAnswers}.xxx"
        End Function

        Public Function WrongAnswers(ByVal student As String, ByVal answers As String) As String
            Dim studentArr = AsArray(student)
            Dim answersArr = AsArray(answers)
            Dim result = ""
            For n As Integer = 0 To answersArr.Length - 1
                If answersArr(n) IsNot Nothing AndAlso studentArr(n) <> answersArr(n) Then
                    result &= $"{n} "
                End If
            Next n
            Return result
        End Function

        Public Function AsArray(ByVal answers As String) As String()
            Dim arr = New String(19) {}
            For Each a As String In answers.Split(ControlChars.Lf).Where(Function(x) x.Length > 0)
                Dim split = a.Split(")"c)
                Dim n = AsInt(split(0).Trim())
                arr(n) = split(1).Trim().ToUpper()
            Next a
            Return arr
        End Function

        Private Function AsInt(ByVal input As String) As Integer
            Return Convert.ToInt32(Encoding.Default.GetString(Encoding.ASCII.GetBytes(input).Where(Function(b) b > 47 AndAlso b < 58).ToArray()))
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

#End Region

#Region "<HiddenCode>"
        Public Function IsFactor(ByVal f As Integer, ByVal n As Integer) As Boolean
            Return n Mod f = 0
        End Function
#End Region

    End Module

    '<Tests>

End Namespace
