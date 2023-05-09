using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;
using SmokeTest.Helpers;
using static SmokeTest.Helpers.MetalUpHelpers;

namespace SmokeTest;

[TestClass]
public class TaskTests : BaseTest {
    #region C#
    [TestMethod]
    public virtual void EvaluateExpressionCSharp() {
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterExpression("1 + 1");
        task.AssertResultIs("2");
    }

    [TestMethod]
    public virtual void DisplayTypes()
    {
        var task = helper.GoToTask(CsEmptyTaskId);

        task.EnterExpression("\"Foo\"");
        task.AssertChangedResultIs("","\"Foo\"");

        task.EnterExpression("true");
        task.AssertChangedResultIs("\"Foo\"","true");

        task.EnterExpression("new List<int> { 1, 2, 3 }");
        task.AssertChangedResultIs("true","{1, 2, 3}");

        task.EnterExpression("new List<string> { \"foo\", \"bar\" }");
        task.AssertChangedResultIs("{1, 2, 3}","{\"foo\", \"bar\"}");

        task.EnterExpression("new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5 } }");
        task.AssertChangedResultIs("{\"foo\", \"bar\"}","{\r\n{1, 2, 3}, \r\n{4, 5}}");

        task.EnterExpression("(\"foo\", 1)");
        task.AssertChangedResultIs("{\r\n{1, 2, 3}, \r\n{4, 5}}", "(\"foo\", 1)");
    }

    [TestMethod]
    public virtual void EvaluateExpressionCSharpFailRegex() {
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterExpression("1");
        task.AssertResultIs("1");
        task.EnterExpression("1;");
        task.AssertExpressionErrorIs("Use of ';' is not permitted");
        task.PreviousExpression();
        task.EnterCurrentExpression();
        task.AssertResultIs("1");
    }

    [TestMethod]
    public virtual void SubmitCodeCSharp()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterCode("static int f() => 1;");
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void EvaluateSubmittedCodeCSharp()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(CsEmptyTaskId);
        var random = new Random().NextInt64();
        task.EnterCode($"static long ReturnRandom() => {random};");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterExpression("ReturnRandom()");
        task.AssertResultIs($"{random}");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void SubmitCodeCSharpRegexFail()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterCode("static int f() => 1;");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterCode("int f() => 1;");
        task.AssertCompileResultIs("All functions should be: static <ReturnType> <NameStartingInUpperCase>(<parametersStartingLowerCase>) => <expression>;");
        task.PreviousCode();
        task.EnterCurrentCode();
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 2, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void SubmitCodeCSharpCompileFail()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterCode("static int f() => 1;");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterCode(@"static int f() => """";");
        task.AssertCompileResultIs("CS0029: Cannot implicitly convert type 'string' to 'int' (1,19)");
        task.PreviousCode();
        task.EnterCurrentCode();
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 3, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void RunTestsCsSuccess()
    {
        var task = helper.GoToTask(CsWithTestTaskId);
        task.EnterCode("static int Square(int x) => x*x; ");
        task.AssertCompileResultIs("Compiled OK");
        task.RunTests();
        task.AssertTestResultIs("All Tests Passed");
    }

    [TestMethod]
    public virtual void RunTestsCsFail()
    {
        var task = helper.GoToTask(CsWithTestTaskId);
        task.EnterCode("static int Square(int x) => x*10;");
        task.AssertCompileResultIs("Compiled OK");
        task.RunTests();
        task.AssertTestResultIs("Test failed calling Square(3) Expected: 9 Actual: 30");
    }
    #endregion

    #region VB
    [TestMethod]
    public virtual void EvaluateExpressionVb() {
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterExpression("1 + 3");
        task.AssertResultIs("4");
    }

    [TestMethod]
    public virtual void DisplayTypesVb()
    {
        var task = helper.GoToTask(VbEmptyTaskId);

        task.EnterExpression("\"Foo\"");
        task.AssertChangedResultIs("", "\"Foo\"");

        task.EnterExpression("true");
        task.AssertChangedResultIs("\"Foo\"", "true");

        task.EnterExpression("New List(Of Integer)({1, 2, 3})");
        task.AssertChangedResultIs("true", "{1, 2, 3}");

        task.EnterExpression("New List(Of String)({\"foo\", \"bar\"})");
        task.AssertChangedResultIs("{1, 2, 3}", "{\"foo\", \"bar\"}");

        task.EnterExpression("New List(Of List(Of Integer))({New List(Of Integer)({ 1, 2, 3 }), New List(Of Integer)({ 4, 5 }) })");
        task.AssertChangedResultIs("{\"foo\", \"bar\"}", "{\r\n{1, 2, 3}, \r\n{4, 5}}");

        task.EnterExpression("(\"foo\", 1)");
        task.AssertChangedResultIs("{\r\n{1, 2, 3}, \r\n{4, 5}}", "(\"foo\", 1)");
    }

    [TestMethod]
    public virtual void EvaluateExpressionVbFailRegex() {
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterExpression("1");
        task.AssertResultIs("1");
        task.EnterExpression("Dim 1");
        task.AssertExpressionErrorIs("Use of the keyword 'Dim' is not permitted");
        task.PreviousExpression();
        task.EnterCurrentExpression();
        task.AssertResultIs("1");
    }

    [TestMethod]
    public virtual void SubmitCodeVb()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterCode(@"
Function F() As Integer
    Return 1
End Function
");
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void EvaluateSubmittedCodeVb()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(VbEmptyTaskId);
        var random = new Random().NextInt64();
        task.EnterCode(@$"
Function ReturnRandom() As Long
    Return {random}
End Function
");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterExpression("ReturnRandom()");
        task.AssertResultIs($"{random}");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void SubmitCodeVbCompileFail()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterCode(@"
Function F() As Integer
    Return 1
End Function
");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterCode(@"
Function F() As Integer
    Return $
End Function
");
        task.AssertCompileResultIs("BC30201: Expression expected. (3,12)");
        task.PreviousCode();
        task.EnterCurrentCode();
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 3, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void SubmitCodeVbRegexFail()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterCode(@"
Function F() As Integer
    Return 1
End Function
");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterCode(@"
F() As Integer
    Return 1
End Function
");
        task.AssertCompileResultIs("All functions must be: Function NameStartingUpperCase(<paramsStartingLowerCase>)<ReturnType\\nReturn <expression>\\nEnd Function");
        task.PreviousCode();
        task.EnterCurrentCode();
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 2, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void RunTestsVBsuccess()
    {
        var task = helper.GoToTask(VbWithTestTaskId);
        task.EnterCode(@"
Function Square(x As Integer) As Integer
    Return x*x
End Function");
        task.AssertCompileResultIs("Compiled OK");
        task.RunTests();
        task.AssertTestResultIs("All Tests Passed");
    }

    [TestMethod]
    public virtual void RunTestsVbFail()
    {
        var task = helper.GoToTask(VbWithTestTaskId);
        task.EnterCode(@"
Function Square(x As Integer) As Integer
    Return x*10
End Function");
        task.AssertCompileResultIs("Compiled OK");
        task.RunTests();
        task.AssertTestResultIs("Test failed calling Square(3) Expected: 9 Actual: 30");
    }
    #endregion

    #region Python
    [TestMethod]
    public virtual void EvaluateExpressionPython()
    {
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterExpression("1 + 2");
        task.AssertResultIs("3");
    }

    [TestMethod]
    public virtual void DisplayTypesPython()
    {
        var task = helper.GoToTask(PyEmptyTaskId);

        task.EnterExpression("'Foo'");
        task.AssertChangedResultIs("", "'Foo'");

        task.EnterExpression("True");
        task.AssertChangedResultIs("'Foo'", "True");

        task.EnterExpression("[1, 2, 3]");
        task.AssertChangedResultIs("True", "[1, 2, 3]");

        task.EnterExpression("['foo','bar']");
        task.AssertChangedResultIs("[1, 2, 3]", "['foo', 'bar']");

        task.EnterExpression("[[1,2,3],[4,5]]");
        task.AssertChangedResultIs("['foo', 'bar']", "[\r\n[1, 2, 3], \r\n[4, 5]]");

        task.EnterExpression("('foo', 1)");
        task.AssertChangedResultIs("[\r\n[1, 2, 3], \r\n[4, 5]]", "('foo', 1)");
    }

    [TestMethod]
    public virtual void EvaluateExpressionPythonFailRegex()
    {
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterExpression("1");
        task.AssertResultIs("1");
        task.EnterExpression("1;");
        task.AssertExpressionErrorIs("Use of ';' is not permitted");
        task.PreviousExpression();
        task.EnterCurrentExpression();
        task.AssertResultIs("1");
    }

    [TestMethod]
    public virtual void SubmitCodePython()
    {
        var before = helper.GetActivityCount();
        Wait.Timeout = new TimeSpan(0, 0, 40); // mypy is SLOW!
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterCode("def f() -> int: return 1");
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void EvaluateSubmittedCodePython()
    {
        var before = helper.GetActivityCount();
        Wait.Timeout = new TimeSpan(0, 0, 40); // mypy is SLOW!
        var task = helper.GoToTask(PyEmptyTaskId);
        var random = new Random().NextInt64();
        task.EnterCode($"def return_random() -> int: return {random}");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterExpression("return_random()");
        task.AssertResultIs($"{random}");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void SubmitCodePythonCompileFail()
    {
        var before = helper.GetActivityCount();
        Wait.Timeout = new TimeSpan(0, 0, 40); // mypy is SLOW!
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterCode("def f() -> int: return 1");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterCode(@"def f() -> int: return """"");
        task.AssertCompileResultIs("error: Incompatible return value type  (1,24)");
        task.PreviousCode();
        task.EnterCurrentCode();
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 3, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void SubmitCodePythonRegexFail()
    {
        var before = helper.GetActivityCount();
        Wait.Timeout = new TimeSpan(0, 0, 40); // mypy is SLOW!
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterCode("def f() -> int: return 1");
        task.AssertCompileResultIs("Compiled OK");
        task.EnterCode("f() -> int: return 1");
        task.AssertCompileResultIs("All functions must follow form: def <lower_case_name>(<params, each with type>) -> <return_type> :  return <expression>");
        task.PreviousCode();
        task.EnterCurrentCode();
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 2, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void RunTestsPySuccess()
    {
        var task = helper.GoToTask(PyWithTestTaskId);
        task.EnterCode("def square(x: int) -> int : return x*x");
        task.AssertCompileResultIs("Compiled OK");
        task.RunTests();
        task.AssertTestResultIs("All Tests Passed");
    }

    [TestMethod]
    public virtual void RunTestsPyFail()
    {
        var task = helper.GoToTask(PyWithTestTaskId);
        task.EnterCode("def square(x: int) -> int : return x*30");
        task.AssertCompileResultIs("Compiled OK");
        task.RunTests();
        task.AssertTestResultIs("Test failed calling Square(3) Expected: 9 Actual: 30");
    }
    #endregion

    #region Overhead

    protected override string BaseUrl => MetalUpHelpers.BaseUrl;

    private static Helper helper;
    private const int CsEmptyTaskId = 107;
    private const int VbEmptyTaskId = 114;
    private const int PyEmptyTaskId = 109;
    private const int CsWithTestTaskId = 118;
    private const int VbWithTestTaskId = 119;
    private const int PyWithTestTaskId = 120;

    [ClassInitialize]
    public static void InitialiseClass(TestContext context) {
        helper = new Helper(MetalUpHelpers.BaseUrl, "dashboard", Driver, Wait);
        helper.LoginAsStudent();
    }

    [ClassCleanup]
    public static void CleanUpClass() {
        helper.Logout();
    }

    [TestInitialize]
    public virtual void InitializeTest() { }

    [TestCleanup]
    public virtual void CleanupTest() {
        //some tests reset timeout
        Wait.Timeout = new TimeSpan(0, 0, 10);
    }

    #endregion
}