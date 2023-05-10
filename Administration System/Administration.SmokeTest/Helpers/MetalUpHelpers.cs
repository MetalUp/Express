using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;

namespace SmokeTest.Helpers;

public static class MetalUpHelpers {
    private const int LongTimeout = 80;
    private const int DefaultTimeout = 40;


    private static readonly string UserIdAdmin = @"metalup.admin@gmail.com";
    private static readonly string UserIdTeacher = @"metalup.dev@gmail.com";
    private static readonly string UserIdStudent = @"metalup.student@gmail.com";
    public static readonly string UserIdInvitee = @"metalup.invitee@gmail.com";

    private static string PasswordTeacher => GetIConfigurationBase()["password_teacher"];
    private static string PasswordStudent => GetIConfigurationBase()["password_student"];
    private static string PasswordAdmin => GetIConfigurationBase()["password_admin"];
    public static string PasswordInvitee => GetIConfigurationBase()["password_invitee"];
    public static string BaseUrl => GetIConfigurationBase()["base_url"];

    public static Helper LoginAsStudent(this Helper helper)
    {
        helper.StartLogin();
        helper.LoginWithAuth0(PasswordStudent, UserIdStudent);
        helper.WaitForCss(".not-in-progress");
        return helper;
    }

    public static Helper LoginAsTeacher(this Helper helper)
    {
        helper.StartLogin();
        helper.LoginWithAuth0(PasswordTeacher, UserIdTeacher);
        helper.WaitForCss(".not-in-progress");
        return helper;
    }

    public static Helper LoginAsAdmin(this Helper helper)
    {
        helper.StartLogin();
        helper.LoginWithAuth0(PasswordAdmin, UserIdAdmin);
        helper.WaitForCss(".not-in-progress");
        return helper;
    }

    public static IConfigurationRoot GetIConfigurationBase() =>
        new ConfigurationBuilder()
            .AddUserSecrets<InvitationTests>()
            .AddEnvironmentVariables()
            .Build();

    public static Helper GoToLanding(this Helper helper)
    {
        helper.WebDriver.Navigate().GoToUrl(helper.BaseUrl + "/landing");
        helper.WaitForCss(".metalup button");
        return helper;
    }

    public static Helper StartLogin(this Helper helper)
    {
        var loginButton = helper.GoToLanding().WaitForCss(".metalup button");
        helper.Click(loginButton);
        return helper;
    }

    public static Helper Logout(this Helper helper)
    {
        helper.GotoHome();
        helper.ClickLogOffButton();
        var logoffButton = helper.WaitForCss(@"button[value=""Log Off""]");
        helper.Click(logoffButton);
        helper.WaitForCss("app-landing button");
        return helper;
    }

    public static Helper LoginWithAuth0(this Helper helper, string pwd, string userId)
    {
        var userInput = helper.WaitForCss(@"input[type=""email""]");
        var passwordInput = helper.WaitForCss(@"input[type=""password""]");
        Thread.Sleep(2000);
        userInput.SendKeys(userId);
        passwordInput.SendKeys(pwd);
        var login = helper.WaitForCss(@"button[name=""submit""]");
        helper.Click(login);
        return helper;
    }

    public static TaskView GoToTask(this Helper helper, int taskId)
    {
        helper.GotoBaseUrlDirectly($"/task/{taskId}");
        helper.WaitForCss("app-expression-evaluation textarea:enabled");
        helper.WaitForCss("app-code-definition  textarea:enabled");
        helper.WaitForCss("app-result");
        helper.WaitForCss("app-task-description");
        helper.WaitForCss("app-hint");
        helper.WaitForCss("app-testing");
        var view = helper.WaitForCss("app-task-view");
        return new TaskView(view, helper);
    }

    public static int GetActivityCount(this Helper helper)
    {
        var list = helper.GotoHome().OpenMainMenu("Activities").GetActionWithoutDialog("My Activities").ClickToViewList();
        var details = list.Details();
        return details is "No items found" ? 0 : list.TotalCount();
    }

    public static Helper WaitAndAssert(this Helper helper, string selector, string expected)
    {
        helper.Wait.Until(d => helper.WaitForCss(selector).Text.Trim().Length > 0);
        var actual = helper.WaitForCss(selector).Text;
        Assert.AreEqual(expected, actual);
        return helper;
    }

    public static Helper WaitForChange(this Helper helper, string selector, string changedFrom)
    {
        helper.Wait.Until(d => helper.WaitForCss(selector).GetAttribute("value") != changedFrom);
        return helper;
    }

    public static Helper WaitUntilPopulated(this Helper helper, string selector) {
        helper.Wait.Until(d => helper.WaitForCss(selector).GetAttribute("value").Trim().Length > 0);
        return helper;
    }

    public static Helper SetLongTimeout(this Helper helper) {
        helper.Wait.Timeout = new TimeSpan(0, 0, LongTimeout);
        return helper;
    }

    public static Helper SetDefaultTimeout(this Helper helper) {
        helper.Wait.Timeout = new TimeSpan(0, 0, DefaultTimeout);
        return helper;
    }
}