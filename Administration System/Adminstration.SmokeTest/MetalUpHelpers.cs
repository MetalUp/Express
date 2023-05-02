using System.Threading;
using NakedFrameworkClient.TestFramework;
using OpenQA.Selenium;

namespace SmokeTest;

public static class MetalUpHelpers {
    
    public static Helper GoToLanding(this Helper helper) {
        helper.WebDriver.Navigate().GoToUrl(helper.BaseUrl + "/landing");
        helper.WaitForCss(".metalup button");
        return helper;
    }

    public static Helper StartLogin(this Helper helper) {
        var loginButton = helper.GoToLanding().WaitForCss(".metalup button");
        helper.Click(loginButton);
        return helper;
    }

    public static Helper Logout(this Helper helper) {
        helper.GetHomeView();
        var logoffIcon = helper.WaitForCss(@".logoff");
        helper.Click(logoffIcon);
        var logoffButton = helper.WaitForCss(@"button[value=""Log Off""]");
        helper.Click(logoffButton);
        return helper;
    }

    public static Helper LoginWithAuth0(this Helper helper, string pwd, string userId) {
        var userInput = helper.WaitForCss(@"input[type=""email""]");
        var passwordInput = helper.WaitForCss(@"input[type=""password""]");
        Thread.Sleep(2000);
        userInput.SendKeys(userId);
        passwordInput.SendKeys(pwd);
        var login = helper.WaitForCss(@"button[name=""submit""]");
        helper.Click(login);
        return helper;
    }
}