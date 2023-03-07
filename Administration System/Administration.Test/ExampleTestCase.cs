using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Model;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedFunctions.Reflector.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using Server;
using Task = System.Threading.Tasks.Task;

namespace Test
{
    [TestClass]
    public class ExampleTestCase : ExpressTestCase
    {

        [SetUp]
        public void SetUp() {
            StartTest();
        }

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            InitializeNakedObjectsFramework(this);
            new TestAdminDbContext().Create();
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            new TestAdminDbContext().Delete();
        }


        protected override void ConfigureServices(IServiceCollection services) {
            services.AddControllers()
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddHttpContextAccessor();
            services.AddNakedFramework(frameworkOptions =>
            {
                frameworkOptions.MainMenus = MenuHelper.GenerateMenus(ModelConfig.MainMenus());
                frameworkOptions.AddEFCorePersistor();
                frameworkOptions.AuthorizationConfiguration = AuthorizationHelpers.AdminAuthConfig();
                frameworkOptions.AddNakedFunctions(appOptions =>
                {
                    appOptions.DomainTypes = ModelConfig.DomainTypes();
                    appOptions.DomainFunctions = ModelConfig.TypesDefiningDomainFunctions();
                    appOptions.DomainServices = ModelConfig.DomainServices();
                });
                frameworkOptions.AddRestfulObjects(options => options.AcceptHeaderStrict = false);
            });
           
            services.AddDbContext<DbContext, TestAdminDbContext>();
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddScoped(p => TestPrincipal);
        }


        [Test]
        public async Task Test1()
        {
            LogInAs("Richard");

            var menus = (await GetHome().GetMenus()).AssertMenuOrder(nameof(Assignments), nameof(Groups), nameof(Invitations), nameof(Organisations), nameof(Projects), nameof(Users));
            
            var projects = (await menus.GetMenu(nameof(Projects))).AssertMemberOrder(nameof(Projects.AllAssignableProjects), nameof(Projects.FindProjects));

            var all = (await projects.GetAction(nameof(Projects.AllAssignableProjects)).AssertNumberOfParameters(1)).AssertReturnsList();
            
            var lang = (await all.GetParameter(0)).AssertName("Language").AssertString().AssertOptional().AssertChoice(0, "Python").AssertDefault(null); //Params numbered from 1 ? (TBC)

            var list = await all.Invoke("");

            //var list = (await (await all.AssertValid(0)).Invoke(0)).GetList(); //Here and line above, 'null' indicates that no option has been specified for an optional param. Could be more explicit as e.g. EMPTY 
            
            
            //var lifeCS = list.AssertType<Project>().AssertHasMember("Life (C Sharp)").GetMember("Life (C Sharp)");
            //lifeCS.AssertPropertyOrder(nameof(Project.Link), nameof(Project.Status), nameof(Project.Title), nameof(Project.Language), nameof(Project.CommonHiddenCodeFile),
            //    nameof(Project.Description), nameof(Project.Keywords), nameof(Project.Tasks)).
            //    AssertActionOrder(nameof(Project_Functions.AssignToMe), nameof(Project_Functions.AssignToIndividual), Project_Functions.AssignToGroup);
            //var toMe = lifeCS.GetProperty(nameof(Project_Functions.AssignToMe));
            //toMe.AssertDisabled("Project is already assigned to you");
        }

       
    }
}
