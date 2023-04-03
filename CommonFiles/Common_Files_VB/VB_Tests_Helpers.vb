Imports Microsoft.VisualBasic
Imports System.Text.RegularExpressions
Imports System.Collections.Immutable

Namespace MetalUp.Express
    <TestClass>
    Public Class VB_Tests_Helpers

        <TestMethod>
        Public Sub TestDisplay()
            Assert.IsNull(Display(Nothing))
            Assert.AreEqual("""Hello""", Display("Hello"))
            Assert.AreEqual("true", Display(True))
            Assert.AreEqual("false", Display(False))
            Dim l1 = ImmutableList.Create(1, 2, 3, 4, 5)
            Assert.AreEqual("1,2,3,4,5", Display(l1))
            Dim l2 = ImmutableList.Create("foo", "bar")
            Assert.AreEqual("""foo"",""bar""", Display(l2))
            Dim l3 = New List(Of Integer) From {1, 2, 3, 4, 5}
            Assert.AreEqual("Result is an ordinary List. Use ImmutableList only.", Display(l3))
            Dim en = l3.Select(Function(x) x)
            Assert.AreEqual("Result is an IEnumerable. Convert to ImmutableList to display contents", Display(en))

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
End Namespace
