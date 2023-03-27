'Namespace MetalUp.Express

'    Module Program
'        Public Sub Main()
'            Console.WriteLine(Display(""))
'        End Sub
'    End Module

'    Public Module HiddenCode

'        Public Function IsPrime(ByVal n As Integer) As Boolean
'            Return Not Enumerable.Range(2, n \ 2 - 1).Any(Function(f) IsFactor(f, n))
'        End Function

'        Public Function IsFactor(ByVal f As Integer, ByVal n As Integer) As Boolean
'            Return n Mod f = 0
'        End Function

'    End Module

'    Public Module Helpers
'        Public Function Display(ByVal obj As Object) As String
'            If obj Is Nothing Then Return Nothing
'            If TypeOf obj Is String Then Return $"{obj}"
'            If TypeOf obj Is Boolean Then Return If(CType(obj, Boolean), "true", "false")

'            If TypeOf obj Is IEnumerable Then
'                Dim disp = (CType(obj, IEnumerable)).Cast(Of Object)().Select(Function(o) Display(o))
'                Return $"{{{String.Join(","c, disp)}}}"
'            End If

'            Return obj.ToString()
'        End Function

'        Public Function ArgString(ParamArray arguments As Object()) As String
'            Return arguments.Aggregate("", Function(s, a) s & Display(a) & ", ").TrimEnd(" "c, ","c)
'        End Function

'        Public Function FailMessage(functionName As String, ByVal expected As Object, ByVal actual As Object, ParamArray args As Object()) As String
'            Return $"!!!Calling {functionName}({ArgString(args)}) Expected: {Display(expected)} Actual: {actual}~~~"
'        End Function
'    End Module

'    Public Module StudentCode
'        Public Function PrimesUpTo(ByVal n As Integer) As List(Of Integer)
'            Return Enumerable.Range(2, n - 1).Where(Function(x) IsPrime(x)).ToList()
'        End Function
'    End Module

'    <TestClass>
'    Public Class Tests

'        <TestMethod>
'        Public Sub TestPrimesUpTo()
'            TestPrimesUpTo(New List(Of Integer) From {2}, 2)
'            TestPrimesUpTo(New List(Of Integer) From {2, 3, 5, 7, 11, 13, 17}, 18)
'            TestPrimesUpTo(New List(Of Integer) From {2, 3, 5, 7, 11, 13, 17, 19}, 19)
'            TestPrimesUpTo(New List(Of Integer) From {2, 3, 5, 7, 11, 13, 17, 19}, 20)
'            TestPrimesUpTo(New List(Of Integer) From {2, 3, 5, 7, 11, 13, 17, 19}, 21)
'        End Sub

'        Private Sub TestPrimesUpTo(ByVal expected As List(Of Integer), ByVal n As Integer)
'            Dim actual = PrimesUpTo(n)
'            CollectionAssert.AreEqual(expected, actual, FailMessage(NameOf(PrimesUpTo), expected, actual, n))
'        End Sub

'        <TestMethod>
'        Public Sub TestIsPrime()
'            TestIsPrime(True, 2)
'            TestIsPrime(True, 3)
'            TestIsPrime(False, 4)
'            TestIsPrime(True, 5)
'            TestIsPrime(False, 6)
'            TestIsPrime(True, 7)
'            TestIsPrime(False, 8)
'            TestIsPrime(False, 9)
'            TestIsPrime(False, 10)
'        End Sub

'        Private Sub TestIsPrime(ByVal expected As Boolean, ByVal n As Integer)
'            Dim actual = IsPrime(n)
'            Assert.AreEqual(expected, actual, FailMessage(NameOf(IsPrime), expected, actual, n))
'        End Sub


'        <TestMethod>
'        Public Sub TestIsFactor()
'            TestIsFactor(True, 1, 1)
'            TestIsFactor(True, 1, 3)
'            TestIsFactor(True, 3, 3)
'            TestIsFactor(False, 2, 3)
'            TestIsFactor(True, 2, 4)
'        End Sub

'        Private Sub TestIsFactor(ByVal expected As Boolean, ByVal f As Integer, ByVal n As Integer)
'            Dim actual = IsFactor(f, n)
'            Assert.AreEqual(expected, actual, FailMessage(NameOf(IsFactor), expected, actual, f, n))
'        End Sub
'    End Class

'End Namespace
