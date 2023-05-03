// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;
using static SmokeTest.MetalUpHelpers;

namespace SmokeTest;

// because the initial timeout needs to be longer if the server is starting up

[TestClass]
public class InvitationTests : BaseTest {
    [TestMethod]
    public virtual void CreateInvitation() {
        helper.LoginAsTeacher();
        var dialog = helper.GotoHome().OpenMainMenu("Invitations").GetActionWithDialog("Invite New Student In My Organisation").Open();
        dialog.GetTextField("Name").Clear().Enter("Test Invitation");
        var user = dialog.ClickOKToViewObject();
        var inviteCode = user.GetProperty("Link To Be Emailed").GetValue();
        helper.Logout();

        Thread.Sleep(2000);

        var code = inviteCode.Split('/').Last();
        helper.GotoBaseUrlDirectly($@"/invitation/{code}");
        var loginButton = helper.WaitForCss(@"app-invitation button");
        helper.Click(loginButton);
        helper.LoginWithAuth0(PasswordStudent, UserIdStudent);
        helper.WaitForCss(".home");
        helper.GotoHome().AssertMainMenusAre("Assignments", "Tasks");
        helper.Logout();
        // delete user

        Thread.Sleep(2000);
        helper.LoginAsAdmin();
        var list = helper.GotoHome().OpenMainMenu("Users").GetActionWithoutDialog("Our Students").ClickToViewList();
        list.AssertNoOfRowsIs(1);
        var student = list.GetRowFromList(0).Click();
        student.AssertTitleIs("Test Invitation");
        var dialog1 = student.OpenActions().GetActionWithDialog("Delete User").Open();
        dialog1.GetTextField("Confirm").Clear().Enter("DELETE");
        dialog1.ClickOKWithNoResultExpected();
        helper.Logout();
    }

    #region Overhead

    protected override string BaseUrl => MetalUpDevelopmentBaseUrl;

    private Helper helper;

    [TestInitialize]
    public virtual void InitializeTest() {
        helper = new Helper(BaseUrl, "dashboard", Driver, Wait);
    }

    [TestCleanup]
    public virtual void CleanupTest() { }

    #endregion
}