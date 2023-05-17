using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;
using SmokeTest.Helpers;
using static SmokeTest.Helpers.MetalUpHelpers;

namespace SmokeTest;

[TestClass]
public class TaskTests : BaseTest
{
    #region C#

    [TestMethod]
    [TestCategory("Production")]
    public virtual void DisplayTypesCSharp()
    {
        var task = helper.GoToTask(CsEmptyTaskId);

        task.EnterExpression("new List<string> { \"foo\", \"bar\" }");
        task.AssertChangedResultIs("", "{\"foo\", \"bar\"}");

        task.EnterExpression("new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5 } }");
        task.AssertChangedResultIs("{\"foo\", \"bar\"}", "{\r\n{1, 2, 3}, \r\n{4, 5}}");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void EvaluateExpressionCSharp()
    {
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterExpression("1 + 1");
        task.AssertResultIs("2");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void EvaluateExpressionCSharpFailRegex()
    {
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
    [TestCategory("Production")]
    public virtual void EvaluateSubmittedCodeCSharp()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(CsEmptyTaskId);
        var random = new Random().NextInt64();
        task.EnterCode($"static long ReturnRandom() => {random};");
        task.AssertCompileResultIs(CompiledOkMsg);
        task.EnterExpression("ReturnRandom()");
        task.AssertResultIs($"{random}");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void SubmitCodeCSharpRegexFail()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterCode("int f1() => 1;");
        const string errorMsg = "All functions should be: static <ReturnType> <NameStartingInUpperCase>(<parametersStartingLowerCase>) => <expression>;";
        task.AssertCompileResultIs(errorMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before, after, "Mismatched activity count");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void SubmitCodeCSharpCompileFail()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterCode(@"static int f1() => """";");
        const string errorMsg = "CS0029: Cannot implicitly convert type 'string' to 'int' (1,20)";
        task.AssertCompileResultIs(errorMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void RunTestsCSharpSuccess()
    {
        var task = helper.GoToTask(CsWithTestTaskId);
        task.EnterCode("static int Square(int x) => x*x; ");
        task.AssertCompileResultIs(CompiledOkMsg);
        task.RunTests();
        task.AssertTestResultIs("All Tests Passed");
    }

    [TestMethod]
    public virtual void RunTestsCSharpFail()
    {
        var task = helper.GoToTask(CsWithTestTaskId);
        task.EnterCode("static int Square(int x) => x*10;");
        task.AssertCompileResultIs(CompiledOkMsg);
        task.RunTests();
        task.AssertTestResultIs("Test failed calling Square(3) Expected: 9 Actual: 30");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void SubmitCodeCSharp()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterCode("static int f1() => 1;");
        task.AssertCompileResultIs(CompiledOkMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    #endregion

    #region VB

    [TestMethod]
    [TestCategory("Production")]
    public virtual void DisplayTypesVb()
    {
        var task = helper.GoToTask(VbEmptyTaskId);

        task.EnterExpression("New List(Of String)({\"foo\", \"bar\"})");
        task.AssertChangedResultIs("", "{\"foo\", \"bar\"}");

        task.EnterExpression("New List(Of List(Of Integer))({New List(Of Integer)({ 1, 2, 3 }), New List(Of Integer)({ 4, 5 }) })");
        task.AssertChangedResultIs("{\"foo\", \"bar\"}", "{\r\n{1, 2, 3}, \r\n{4, 5}}");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void EvaluateExpressionVb()
    {
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterExpression("1 + 3");
        task.AssertResultIs("4");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void EvaluateExpressionVbFailRegex()
    {
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
    [TestCategory("Production")]
    public virtual void SubmitCodeVb()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterCode(@"
Function F1() As Integer
    Return 1
End Function
");
        task.AssertCompileResultIs(CompiledOkMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    [TestCategory("Production")]
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
        task.AssertCompileResultIs(CompiledOkMsg);
        task.EnterExpression("ReturnRandom()");
        task.AssertResultIs($"{random}");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void SubmitCodeVbCompileFail()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterCode(@"
Function F1() As Integer
    Return $
End Function
");
        var errorMsg = "BC30201: Expression expected. (3,12)";
        task.AssertCompileResultIs(errorMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void SubmitCodeVbRegexFail()
    {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(VbEmptyTaskId);
        task.EnterCode(@"
F1() As Integer
    Return 1
End Function
");
        const string errorMsg = "All functions must be: Function NameStartingUpperCase(<paramsStartingLowerCase>)<ReturnType\\nReturn <expression>\\nEnd Function";
        task.AssertCompileResultIs(errorMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void RunTestsVBSuccess()
    {
        var task = helper.GoToTask(VbWithTestTaskId);
        task.EnterCode(@"
Function Square(x As Integer) As Integer
    Return x*x
End Function");
        task.AssertCompileResultIs(CompiledOkMsg);
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
        task.AssertCompileResultIs(CompiledOkMsg);
        task.RunTests();
        task.AssertTestResultIs("Test failed calling Square(3) Expected: 9 Actual: 30");
    }

    #endregion

    #region Python

    [TestMethod]
    [TestCategory("Production")]
    public virtual void DisplayTypesPython()
    {
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterExpression("['foo','bar']");
        task.AssertChangedResultIs("", "['foo', 'bar']");

        task.EnterExpression("[[1,2,3],[4,5]]");
        task.AssertChangedResultIs("['foo', 'bar']", "[\r\n[1, 2, 3], \r\n[4, 5]]");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void EvaluateExpressionPython()
    {
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterExpression("1 + 2");
        task.AssertResultIs("3");
    }

    [TestMethod]
    [TestCategory("Production")]
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
    [TestCategory("Production")]
    public virtual void SubmitCodePython()
    {
        var before = helper.GetActivityCount();
        helper.SetLongTimeout(); // mypy is SLOW!
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterCode("def f1() -> int: return 1");
        task.AssertCompileResultIs(CompiledOkMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void EvaluateSubmittedCodePython()
    {
        var before = helper.GetActivityCount();
        helper.SetLongTimeout(); // mypy is SLOW!
        var task = helper.GoToTask(PyEmptyTaskId);
        var random = new Random().NextInt64();
        task.EnterCode($"def return_random() -> int: return {random}");
        task.AssertCompileResultIs(CompiledOkMsg);
        task.EnterExpression("return_random()");
        task.AssertResultIs($"{random}");
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void SubmitCodePythonCompileFail()
    {
        var before = helper.GetActivityCount();
        helper.SetLongTimeout(); // mypy is SLOW!
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterCode(@"def f() -> int: return """"");
        const string errorMsg = @"error: Incompatible return value type (got ""str"", expected ""int"")  [return-value] (1,24)";
        task.AssertCompileResultIs(errorMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void SubmitCodePythonRegexFail()
    {
        var before = helper.GetActivityCount();
        helper.SetLongTimeout(); // mypy is SLOW!
        var task = helper.GoToTask(PyEmptyTaskId);
        task.EnterCode("f() -> int: return 1");
        const string errorMsg = "All functions must follow form: def <lower_case_name>(<params, each with type>) -> <return_type> :  return <expression>";
        task.AssertCompileResultIs(errorMsg);
        var after = helper.GetActivityCount();
        Assert.AreEqual(before, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void RunTestsPythonSuccess()
    {
        var task = helper.GoToTask(PyWithTestTaskId);
        helper.SetLongTimeout(); // mypy is SLOW!
        task.EnterCode("def square(x: int) -> int : return x*x");
        task.AssertCompileResultIs(CompiledOkMsg);
        task.RunTests();
        task.AssertTestResultIs("All Tests Passed");
    }

    [TestMethod]
    public virtual void RunTestsPythonFail()
    {
        var task = helper.GoToTask(PyWithTestTaskId);
        helper.SetLongTimeout(); // mypy is SLOW!
        task.EnterCode("def square(x: int) -> int : return x*10");
        task.AssertCompileResultIs(CompiledOkMsg);
        task.RunTests();
        task.AssertTestResultIs("Test failed calling square(3) Expected: 9 Actual: 30");
    }

    #endregion

    #region Overhead

    protected override string BaseUrl => MetalUpHelpers.BaseUrl;

    private static Helper helper;

    [ClassInitialize]
    public static void InitialiseClass(TestContext context)
    {
        helper = new Helper(MetalUpHelpers.BaseUrl, "dashboard", Driver, Wait);
        helper.SetDefaultTimeout();
        helper.LoginAsStudent();
    }

    [ClassCleanup]
    public static void CleanUpClass()
    {
        helper.Logout();
    }

    [TestInitialize]
    public virtual void InitializeTest() { }

    [TestCleanup]
    public virtual void CleanupTest()
    {
        helper.SetDefaultTimeout();
    }

    #endregion
}