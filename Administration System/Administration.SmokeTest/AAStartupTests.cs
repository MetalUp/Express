﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;
using SmokeTest.Helpers;
using static SmokeTest.Helpers.MetalUpHelpers;

namespace SmokeTest;

// because the initial timeout needs to be longer if the server is starting up

[TestClass]
public class AAStartupTests : BaseTest {
    // first tests have longer timeouts to ensure servers woken up

    [TestMethod]
    [TestCategory("Production")]
    public virtual void LoginAndLogout() {
        helper.LoginAsTeacher();
        helper.Logout();
    }

    [TestMethod]
    [TestCategory("Production")]
    public virtual void WakeUpCompileServer() {
        helper.LoginAsStudent();
        var task = helper.GoToTask(CsEmptyTaskId);
        task.EnterExpression("1 + 1");
        task.AssertResultIs("2");
        task.EnterCode("static int f1() => 1;");
        task.AssertCompileResultIs(CompiledOkMsg);
        helper.Logout();
    }

    #region Overhead

    protected override string BaseUrl => MetalUpHelpers.BaseUrl;

    [AssemblyInitialize]
    public static void InitialiseAssembly(TestContext context) {
        FilePath(@"drivers.chromedriver.exe");
        InitChromeDriver();
    }

    [AssemblyCleanup]
    public static void CleanUpAssembly() {
        CleanupChromeDriver();
    }

    private Helper helper;

    [TestInitialize]
    public virtual void InitializeTest() {
        helper = new Helper(BaseUrl, "dashboard", Driver, Wait);
        helper.SetLongTimeout();
    }

    [TestCleanup]
    public virtual void CleanupTest() {
        helper.SetDefaultTimeout();
        helper.WebDriver.Manage().Cookies.DeleteAllCookies();
    }

    #endregion
}