Imports Microsoft.VisualBasic
Imports System.Text.RegularExpressions


<TestClass>
Public Class VB_Tests_Regex
    Public Const multipleFunctions As String = "^[\n\s]*(?:Function\s.*\n\s*Return\s.*\n\s*End Function\s*)*$" 'TODO: Needs checking!


    <TestMethod>
    Public Sub TestMultipleFunctions()
        AssertWholeStringMatches("Function Foo(a As Integer) As String" & vbLf & "  Return a*3" & vbLf & " End Function", multipleFunctions)
        AssertWholeStringMatches("Function Foo(a As Integer) As String" & vbLf & "  Return a*3" & vbLf & " End Function" & vbLf & "Function Bar(a As Integer) As String" & vbLf & "  Return a*3" & vbLf & " End Function", multipleFunctions)
        AssertWholeStringMatches("Function Foo(a As Integer) As String" & vbLf & "  Return a*3" & vbLf & " End Function " & vbLf & vbLf & "  Function Bar(a As Integer) As String" & vbLf & "  Return a*3" & vbLf & " End Function " & vbLf & "   ", multipleFunctions)
    End Sub

#Region "helpers"
    Private Sub AssertWholeStringMatches(ByVal input As String, ByVal pattern As String)
        Dim whole = $"^{pattern}$"
        Dim match = Regex.Match(input, whole, RegexOptions.IgnoreCase)
        Assert.IsTrue(match.Success)
        Assert.AreEqual(input, match.Value)
    End Sub
#End Region
End Class
