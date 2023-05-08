using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;
using static SmokeTest.Helpers.MetalUpHelpers;

namespace SmokeTest;

[TestClass]
public class TaskTests : BaseTest {
    [TestMethod]
    public virtual void EvaluateExpressionCSharp() {
        var task = helper.GoToTask(107);
        task.EnterExpression("1 + 1");
        task.AssertResultIs("2");
    }

    [TestMethod]
    public virtual void EvaluateExpressionCSharpFailRegex() {
        var task = helper.GoToTask(107);
        task.EnterExpression("1");
        task.AssertResultIs("1");
        task.EnterExpression("1;");
        task.AssertExpressionErrorIs("Use of ';' is not permitted");
        task.PreviousExpression();
        task.EnterCurrentExpression();
        task.AssertResultIs("1");
    }

    [TestMethod]
    public virtual void EvaluateExpressionPython() {
        var task = helper.GoToTask(109);
        task.EnterExpression("1 + 2");
        task.AssertResultIs("3");
    }

    [TestMethod]
    public virtual void EvaluateExpressionPythonFailRegex() {
        var task = helper.GoToTask(109);
        task.EnterExpression("1");
        task.AssertResultIs("1");
        task.EnterExpression("1;");
        task.AssertExpressionErrorIs("Use of ';' is not permitted");
        task.PreviousExpression();
        task.EnterCurrentExpression();
        task.AssertResultIs("1");
    }

    [TestMethod]
    public virtual void EvaluateExpressionVb() {
        var task = helper.GoToTask(114);
        task.EnterExpression("1 + 3");
        task.AssertResultIs("4");
    }

    [TestMethod]
    public virtual void EvaluateExpressionVbFailRegex() {
        var task = helper.GoToTask(114);
        task.EnterExpression("1");
        task.AssertResultIs("1");
        task.EnterExpression("Dim 1");
        task.AssertExpressionErrorIs("Use of the keyword 'Dim' is not permitted");
        task.PreviousExpression();
        task.EnterCurrentExpression();
        task.AssertResultIs("1");
    }

    [TestMethod]
    public virtual void SubmitCodeCSharp() {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(107);
        task.EnterCode("static int f() => 1;");
        task.AssertCompileResultIs("Compiled OK");
        var after = helper.GetActivityCount();

        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void EvaluateSubmittedCodeCSharp() {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(107);
        var random = new Random().NextInt64();
        task.EnterCode($"static long ReturnRandom() => {random};");
        task.AssertCompileResultIs("Compiled OK");

        task.EnterExpression("ReturnRandom()");
        task.AssertResultIs($"{random}");

        var after = helper.GetActivityCount();

        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void SubmitCodePython() {
        var before = helper.GetActivityCount();
        Wait.Timeout = new TimeSpan(0, 0, 40); // mypy is SLOW!
        var task = helper.GoToTask(109);
        task.EnterCode("def f() -> int: return 1");
        task.AssertCompileResultIs("Compiled OK");

        var after = helper.GetActivityCount();

        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void EvaluateSubmittedCodePython() {
        var before = helper.GetActivityCount();
        Wait.Timeout = new TimeSpan(0, 0, 40); // mypy is SLOW!
        var task = helper.GoToTask(109);
        var random = new Random().NextInt64();
        task.EnterCode($"def return_random() -> int: return {random}");
        task.AssertCompileResultIs("Compiled OK");

        task.EnterExpression("return_random()");
        task.AssertResultIs($"{random}");

        var after = helper.GetActivityCount();

        Assert.AreEqual(before + 1, after, "Mismatched activity count");
    }

    [TestMethod]
    public virtual void SubmitCodeVb() {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(114);
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
    public virtual void EvaluateSubmittedCodeVb() {
        var before = helper.GetActivityCount();
        var task = helper.GoToTask(114);
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

    #region Overhead

    protected override string BaseUrl => MetalUpDevelopmentBaseUrl;

    private static Helper helper;

    [ClassInitialize]
    public static void InitialiseClass(TestContext context) {
        helper = new Helper(MetalUpDevelopmentBaseUrl, "dashboard", Driver, Wait);
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