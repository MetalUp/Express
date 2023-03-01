namespace Test
{
    [TestClass]
    public class ExampleTestCase : ExpressTestCase
    {
        [TestMethod]
        public void Test1()
        {
            //LogInAs("Richard Teacher (LinkedIn)");
            //var menus = GetHome().GetMenus().AssertMenuOrder(nameof(Activities), nameof(Groups), nameof(Invitations), nameof(Organisations), nameof(Projects), nameof(Users));
            //var projects = menus.GetMenu(nameof(Projects)).AssertMemberOrder(nameof(Projects.AllAssignableProjects), nameof(Projects.FindProjects));
            //var all = projects.GetAction(nameof(Projects.AllAssignableProjects)).AssertNumberOfParameters(1).AssertReturnsList();
            //var lang = all.GetParameter(1).AssertName("Language").AssertType<Language>().AssertOptional().AssertChoice(0, "Python").AssertValue(null); //Params numbered from 1 ? (TBC)
            //var list = all.AssertValid(null).Invoke(null).GetList(); //Here and line above, 'null' indicates that no option has been specified for an optional param. Could be more explicit as e.g. EMPTY 
            //var lifeCS = list.AssertType<Project>().AssertHasMember("Life (C Sharp)").GetMember("Life (C Sharp)");
            //lifeCS.AssertPropertyOrder(nameof(Project.Link), nameof(Project.Status), nameof(Project.Title), nameof(Project.Language), nameof(Project.CommonHiddenCodeFile),
            //    nameof(Project.Description), nameof(Project.Keywords), nameof(Project.Tasks)).
            //    AssertActionOrder(nameof(Project_Functions.AssignToMe), nameof(Project_Functions.AssignToIndividual), Project_Functions.AssignToGroup);
            //var toMe = lifeCS.GetProperty(nameof(Project_Functions.AssignToMe));
            //toMe.AssertDisabled("Project is already assigned to you");
        }
    }
}
