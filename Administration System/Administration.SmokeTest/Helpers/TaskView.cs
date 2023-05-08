﻿using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

    private IWebElement GetCodeSubmitButton() => helper.WaitForCssNo("app-code-definition button", 0);

    private IWebElement GetPreviousExpressionButton() => helper.WaitForCssNo("app-expression-evaluation button", 2);

    private IWebElement GetSubmitExpressionButton() => helper.WaitForCssNo("app-expression-evaluation button", 0);

    public TaskView EnterExpression(string expression) {
        var inp = helper.WaitForCss(".expression textarea");
        Thread.Sleep(1000);
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

    public TaskView EnterCode(string code) {
        var inp = helper.WaitForCss(".code-definition textarea");
        Thread.Sleep(1000);
        inp.Clear();
        inp.SendKeys(code);
        helper.Click(GetCodeSubmitButton());
        return this;
    }

    public TaskView AssertResultIs(string result) {
        helper.WaitAndAssert(".result textarea", result);
        return this;
    }

    public TaskView AssertExpressionErrorIs(string result) {
        helper.WaitAndAssert("app-expression-evaluation textarea:read-only", result);
        return this;
    }

    public TaskView AssertCompileResultIs(string result) {
        helper.WaitAndAssert("app-code-definition textarea:read-only", result);
        return this;
    }
}