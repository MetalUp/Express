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
using SmokeTest.Helpers;
using static SmokeTest.Helpers.MetalUpHelpers;

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
        helper.LoginWithAuth0(PasswordInvitee, UserIdInvitee);
        helper.WaitForCss(".home");
        helper.GotoHome().AssertMainMenusAre("Activities", "Assignments", "Tasks");
        helper.Logout();
    }

    #region Overhead

    protected override string BaseUrl => MetalUpHelpers.BaseUrl;

    private Helper helper;

    [TestInitialize]
    public virtual void InitializeTest() {
        helper = new Helper(BaseUrl, "dashboard", Driver, Wait);
    }

    [TestCleanup]
    public virtual void CleanupTest()
    {
        Thread.Sleep(2000);
        helper.LoginAsAdmin();

        while (DeleteTopStudent()) { }

        helper.Logout();
    }

    private bool DeleteTopStudent()
    {
        var list = helper.GotoHome().OpenMainMenu("Users").GetActionWithoutDialog("Our Students").ClickToViewList();

        var noRow = list.RowCount();

        if (noRow >= 2)
        {
            var student = list.GetRowFromList(0).Click();
            student.AssertTitleIs("Test Invitation");
            var dialog1 = student.OpenActions().GetActionWithDialog("Delete User").Open();
            dialog1.GetTextField("Confirm").Clear().Enter("DELETE");
            dialog1.ClickOKWithNoResultExpected();
            return true;
        }

        return false;
    }

    #endregion
}