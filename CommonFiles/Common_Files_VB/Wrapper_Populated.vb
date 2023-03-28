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
            Return $"!!!Calling {functionName}({ArgString(args)}) Expected: {Display(expected)} Actual: {actual}~~~"
        End Function
#End Region

#Region "<HiddenCode>"
        Public Function IsFactor(ByVal f As Integer, ByVal n As Integer) As Boolean
            Return n Mod f = 0
        End Function
#End Region

    End Module


    <TestClass>
    Public Class Tests

        <TestMethod>
        Public Sub TestIsPrime()
            TestIsPrime(True, 2)
            TestIsPrime(True, 3)
            TestIsPrime(False, 4)
            TestIsPrime(True, 5)
            TestIsPrime(False, 6)
            TestIsPrime(True, 7)
            TestIsPrime(False, 8)
            TestIsPrime(False, 9)
            TestIsPrime(False, 10)
        End Sub

        Private Sub TestIsPrime(ByVal expected As Boolean, ByVal n As Integer)
            Dim actual = IsPrime(n)
            Assert.AreEqual(expected, actual, FailMessage(NameOf(IsPrime), expected, actual, n))
        End Sub

    End Class

End Namespace
