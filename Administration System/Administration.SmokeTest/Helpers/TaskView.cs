﻿using System.Threading;
using NakedFrameworkClient.TestFramework;
using OpenQA.Selenium;

namespace SmokeTest.Helpers;

public class TaskView {
    private readonly IWebElement element;
    private readonly Helper helper;

    public TaskView(IWebElement element, Helper helper) {
        this.element = element;
        this.helper = helper;
    }

    private IWebElement GetRunTestsButton() => helper.WaitForCssNo("app-testing button", 0);

    private IWebElement GetSubmitCodeButton() => helper.WaitForCssNo("app-code-definition button", 0);

    private IWebElement GetPreviousCodeButton() => helper.WaitForCssNo("app-code-definition button", 2);

    private IWebElement GetPreviousExpressionButton() => helper.WaitForCssNo("app-expression-evaluation button", 2);

    private IWebElement GetSubmitExpressionButton() => helper.WaitForCssNo("app-expression-evaluation button", 0);

    public TaskView EnterExpression(string expression) {
        var inp = helper.WaitForCss(".expression textarea:enabled");
        inp.Clear();
        inp.SendKeys(expression);
        inp.SendKeys(Keys.Enter);
        return this;
    }

    public TaskView EnterCurrentExpression() {
        helper.Click(GetSubmitExpressionButton());
        return this;
    }

    public TaskView PreviousExpression() {
        helper.Click(GetPreviousExpressionButton());
        return this;
    }

    public TaskView PreviousCode() {
        helper.Click(GetPreviousCodeButton());
        return this;
    }

    public TaskView RunTests()
    {
        helper.Click(GetRunTestsButton());
        helper.WaitForChange("app-testing textarea", "Tests not yet run on current function definition(s).");
        return this;
    }

    public TaskView AssertTestResultIs(string expected)
    {
        helper.WaitAndAssert("app-testing textarea", expected);
        return this;
    }

    public TaskView EnterCode(string code) {
        helper.WaitUntilPopulated(".code-definition textarea");
        var inp = helper.WaitForCss(".code-definition textarea:enabled");
        inp.Clear();
        inp.SendKeys(code);
        helper.Click(GetSubmitCodeButton());
        return this;
    }

    public TaskView AssertResultIs(string result) {
        helper.WaitAndAssert(".result textarea", result);
        return this;
    }

    public TaskView AssertChangedResultIs(string oldValue, string newValue)
    {
        helper.Wait.Until(d => helper.WaitForCss(".result textarea").Text != oldValue);
        return AssertResultIs(newValue);
    }

    public TaskView EnterCurrentCode() {
        helper.Click(GetSubmitCodeButton());
        return this;
    }

    public TaskView AssertExpressionErrorIs(string result) {
        helper.WaitAndAssert("app-expression-evaluation textarea:read-only", result);
        return this;
    }

    public TaskView AssertCompileResultIs(string result) {
        helper.WaitAndAssert("app-code-definition textarea.current-status", result);
        return this;
    }

    public TaskView AssertChangedCompileResultIs(string result, string from) {
        helper.WaitForChange("app-code-definition textarea.current-status", from);
        helper.WaitAndAssert("app-code-definition textarea.current-status", result);
        return this;
    }
}