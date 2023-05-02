// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Configuration;
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

    [TestMethod]
    public virtual void LoginAndLogout() {
        helper.GoToLanding();
        helper.StartLogin();
        helper.LoginWithAuth0(Password, UserId);
        helper.Logout();
    }

    #region Overhead

    protected override string BaseUrl => @"https://development.metalup.org/";

    private string Password => GetIConfigurationBase()["password"];

    private Helper helper;
    private readonly string UserId = @"metalup.dev@gmail.com";

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