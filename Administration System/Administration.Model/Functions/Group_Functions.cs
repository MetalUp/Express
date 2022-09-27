namespace Model.Functions
{
    public static class Group_Functions
    {

        public static IContext AddStudent(this Group group, User student, IContext context) =>
            context.WithUpdated(group, 
                new Group(group) { Students = group.Students.Append(student).ToList() });


        public static string ValidateAddStudent(this Group group, User student, IContext context) =>
            !student.HasRole(Role.Student) ? "User is not a Student" :
                group.Students.Contains(student) ? "User is already a member of group" : null;

        public static IContext RemoveStudent(this Group group, User student, IContext context)
        {
            var s2 = new List<User>(group.Students);
            s2.Remove(student);
            return context.WithUpdated(group, new Group(group) { Students = s2 });
        }

        public static List<User> Choices1RemoveStudent(this Group group, User student, IContext context) =>
            group.Students.ToList();

        public static IContext RemoveAllStudents(this Group group, string confirm, IContext context)
        {
            var s2 = new List<User>(group.Students);
            var g2 = new Group(group) { Students = s2 };
            return context.WithUpdated(group, g2);
        }

        public static string ValidateRemoveAllStudents(this Group group, [DescribedAs("type REMOVE ALL")] string confirm, IContext context) =>
            confirm == "REMOVE ALL" ? null : "Must type REMOVE ALL into Confirm field";

           
        public static IContext DeleteGroup(this Group group, string confirm, IContext context) =>
            context.WithDeleted(group);

        public static string ValidateDeleteGroup(this Group group, [DescribedAs("type DELETE GROUP")] string confirm, IContext context) =>
            confirm == "DELETE GROUP" ? null : "Must type DELETE GROUP into Confirm field";
    }
}
