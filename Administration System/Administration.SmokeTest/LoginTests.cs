// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;

namespace SmokeTest;

// because the initial timeout needs to be longer if the server is starting up

[TestClass]
public class LoginTests : BaseTest {
    private static IConfigurationRoot GetIConfigurationBase() =>
        new ConfigurationBuilder()
            .AddUserSecrets<LoginTests>()
            .AddEnvironmentVariables()
            .Build();

    private void Login() {
        helper.GoToLanding();
        helper.StartLogin();
        helper.LoginWithAuth0(PasswordDev, UserIdDev);
    }

    [TestMethod]
    public virtual void LoginAndLogout() {
        Login();
        helper.Logout();
    }

    [TestMethod]
    public virtual void CreateInvitation() {
        Login();
        Thread.Sleep(2000); // need to wait here until redirected - may be better fix
        var dialog = helper.GotoHome().OpenMainMenu("Invitations").GetActionWithDialog("Invite New Student In My Organisation").Open();
        dialog.GetTextField("Name").Clear().Enter("Test Invitation");
        var user = dialog.ClickOKToViewObject();
        var inviteCode = user.GetProperty("Link To Be Emailed").GetValue();
        helper.Logout();
        Thread.Sleep(2000);
        var code = inviteCode.Split('/').Last();
        helper.GotoBaseUrlDirectly($@"/invitation/{code}");
        Thread.Sleep(2000);
        var loginButton = helper.WaitForCss(@"app-invitation button");
        helper.Click(loginButton);
        helper.LoginWithAuth0(PasswordStudent, UserIdStudent);
        Thread.Sleep(4000);
        helper.GetHomeView().AssertMainMenusAre("Assignments", "Tasks");
        helper.Logout();

        Thread.Sleep(10000);
        // delete user
    }

    #region Overhead

    protected override string BaseUrl => @"https://development.metalup.org/";

    private string PasswordDev => GetIConfigurationBase()["password_dev"];
    private string PasswordStudent => GetIConfigurationBase()["password_student"];

    private Helper helper;
    private readonly string UserIdDev = @"metalup.dev@gmail.com";
    private readonly string UserIdStudent = @"metalup.student@gmail.com";

    [TestInitialize]
    public virtual void InitializeTest() {
        Wait.Timeout = new TimeSpan(0, 0, 40);
        helper = new Helper(BaseUrl, "dashboard", Driver, Wait);
    }

    [TestCleanup]
    public virtual void CleanupTest() {
        Wait.Timeout = new TimeSpan(0, 0, 10);
    }

    #endregion
}