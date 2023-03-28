Imports System.Collections.Immutable
Imports System.Text

Public Module Helpers
    Public Function Display(ByVal obj As Object) As String
        If obj Is Nothing Then Return Nothing
        If TypeOf obj Is String Then Return $"{obj}"
        If TypeOf obj Is Boolean Then Return If(CType(obj, Boolean), "true", "false")

        If TypeOf obj Is IEnumerable Then
            Dim disp = (CType(obj, IEnumerable)).Cast(Of Object)().Select(Function(o) Display(o))
            Return $"{{{String.Join(","c, disp)}}}"
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

    Public Function WrongAnswers(ByVal expected As String, ByVal actual As String) As String
        Dim studentArr = AsArray(actual)
        Dim answersArr = AsArray(expected)
        Dim result = ""
        For n As Integer = 0 To answersArr.Length - 1
            If answersArr(n) IsNot Nothing AndAlso Not IsMatch(answersArr(n), studentArr(n)) Then
                result &= $"{n} "
            End If
        Next n
        Return result
    End Function

    Public Function AsArray(ByVal answers As String) As String()
        Dim arr = New String(50) {}
        answers = answers.Replace(vbLf, "^")
        Dim answers2 = answers.Split("^"c).Where(Function(x) x.Trim().Length > 0).ToList()
        For Each answer In answers2.Where(Function(x) x.Length > 0)
            Dim split = answer.Split(")"c)
            Dim n As Integer = 0
            If Int32.TryParse(split(0).Trim(), n) Then
                arr(n) = split(1).Trim().ToUpper()
            End If
        Next answer
        Return arr
    End Function

    Private Function AsInt(ByVal input As String) As Integer
        Return Convert.ToInt32(Encoding.Default.GetString(Encoding.ASCII.GetBytes(input).Where(Function(b) b > 47 AndAlso b < 58).ToArray()))
    End Function

    Private Function IsMatch(ByVal expected As String, ByVal actual As String) As Boolean
        Return actual IsNot Nothing AndAlso (expected = actual OrElse expected.Contains("...") AndAlso actual.Contains(expected.Replace("...", "")))
    End Function

End Module
