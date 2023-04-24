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

    public static Helper Login(this Helper helper) {
        var loginButton = helper.WaitForCss(".metalup button");
        helper.Click(loginButton);
        return helper;
    }

    public static Helper LoginWithGithub(this Helper helper) {
        var loginGithub = helper.WaitForCss(@"a[data-provider=""github""]");
        Thread.Sleep(2000);
        helper.Click(loginGithub);
        var user = helper.WaitForCss(@"input#login_field");
        var userId = "";
        var pwd = "";
        user.SendKeys(userId);
        helper.Wait.Until(dr => user.GetAttribute("value") == userId);
        var password = helper.WaitForCss("input#password");
        password.SendKeys(pwd);
        helper.Wait.Until(dr => password.GetAttribute("value") == pwd);
       
        var signIn = helper.WaitForCss(@"input[type=""submit""]");
        helper.Click(signIn);
        return helper;
    }
}