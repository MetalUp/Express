using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;
using SmokeTest.Helpers;
using System;
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
    public virtual void EvaluateExpressionPython() {
        var task = helper.GoToTask(109);
        task.EnterExpression("1 + 2");
        task.AssertResultIs("3");
    }

    [TestMethod]
    public virtual void EvaluateExpressionVb() {
        var task = helper.GoToTask(114);
        task.EnterExpression("1 + 3");
        task.AssertResultIs("4");
    }

    [TestMethod]
    public virtual void SubmitCodeCSharp() {
        var task = helper.GoToTask(107);
        task.EnterCode("static int f() => 1;");
        task.AssertCompileResultIs("Compiled OK");
    }

    [TestMethod]
    public virtual void SubmitCodePython() {
        Wait.Timeout = new TimeSpan(0, 0, 40); // mypy is SLOW!
        var task = helper.GoToTask(109);
        task.EnterCode("def f() -> int: return 1");
        task.AssertCompileResultIs("Compiled OK");
        Wait.Timeout = new TimeSpan(0, 0, 10);
    }

    [TestMethod]
    public virtual void SubmitCodeVb() {
        var task = helper.GoToTask(114);
        task.EnterCode(@"
Function F() As Integer
    Return 1
End Function
");
        task.AssertCompileResultIs("Compiled OK");
        Wait.Timeout = new TimeSpan(0, 0, 10);
    }


    #region Overhead

    protected override string BaseUrl => MetalUpDevelopmentBaseUrl;

    private static Helper helper;

    [ClassInitialize]
    public static void InitialiseClass(TestContext context) {
        helper = new Helper(MetalUpDevelopmentBaseUrl, "dashboard", Driver, Wait);
        helper.LoginAsTeacher();
    }

    [ClassCleanup]
    public static void CleanUpClass() {
        helper.Logout();
    }

    [TestInitialize]
    public virtual void InitializeTest() { }

    [TestCleanup]
    public virtual void CleanupTest() { }

    #endregion
}