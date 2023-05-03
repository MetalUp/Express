using System.Threading;
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

    public TaskView EnterExpression(string expression) {
        var inp = helper.WaitForCss(".expression textarea");
        Thread.Sleep(1000);
        inp.SendKeys(expression);
        inp.SendKeys(Keys.Enter);
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
        helper.Wait.Until(d => helper.WaitForCss(".result textarea").Text.Trim().Length > 0);
        Assert.AreEqual(result, helper.WaitForCss(".result textarea").Text);
        return this;
    }

    public TaskView AssertCompileResultIs(string result) {
        helper.Wait.Until(d => helper.WaitForCssNo("app-code-definition textarea", 1).Text.Trim().Length > 0);
        Assert.AreEqual(result, helper.WaitForCssNo("app-code-definition textarea", 1).Text);
        return this;
    }
}