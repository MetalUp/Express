using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using OpenQA.Selenium;

namespace SmokeTest;

public class TaskView {
    private readonly IWebElement element;
    private readonly Helper helper;

    public TaskView(IWebElement element, Helper helper) {
        this.element = element;
        this.helper = helper;
    }

    public TaskView EnterExpression(string expression) {
        var inp = helper.WaitForCss(".expression textarea");
        Thread.Sleep(2000);
        inp.SendKeys(expression);
        inp.SendKeys(Keys.Enter);
        return this;
    }

    public TaskView AssertResultIs(string result) {
        helper.Wait.Until(d => this.helper.WaitForCss(".result textarea").Text.Trim().Length > 0);
        Assert.AreEqual<string>(result, this.helper.WaitForCss(".result textarea").Text);
        return this;
    }
}