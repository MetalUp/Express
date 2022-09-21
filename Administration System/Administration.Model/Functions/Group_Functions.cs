namespace Model.Functions
{
    public static class Group_Functions
    {
        public static List<User> Students(this Group group, IContext context)
        {
            var id = group.Id;
            return context.Instances<StudentGroup>().Where(sg => sg.GroupId == id).Select(sg => sg.Student).ToList();
        }

        //TODO: validation that user is
        public static IContext AddStudent(this Group group, User student, IContext context)
        {
            var sg = new StudentGroup() { GroupId = group.Id, Group = group, StudentId = student.Id, Student = student };
            return context.WithNew(sg);
        }

        public static string ValidateAddStudent(this Group group, User student, IContext context) =>
            !student.HasRole(Role.Student) ? "User is not a Student" :
                group.Students(context).Contains(student) ? "User is already a member of group" : null;

        public static IContext RemoveStudent(this Group group, User student, IContext context)
        {
            var gid = group.Id;
            var sid = student.Id;
            var sg = context.Instances<StudentGroup>().Single(sg => sg.GroupId == gid && sg.StudentId == sid);
            return context.WithDeleted(sg);
        }

        public static List<User> Choices1RemoveStudent(this Group group, User student, IContext context) =>
            group.Students(context);

        public static IContext RemoveAllStudents(this Group group, string confirm, IContext context)
        {
            var gid = group.Id;
            var sgs = context.Instances<StudentGroup>().Where(sg => sg.GroupId == gid);
            return sgs.Aggregate(context, (context, sg) => context.WithDeleted(sg));
        }

        public static string ValidateRemoveAllStudents(this Group group, [DescribedAs("type REMOVE ALL")] string confirm, IContext context) =>
            confirm == "REMOVE ALL" ? null : "Must type REMOVE ALL into Confirm field";

           
        public static IContext DeleteGroup(this Group group, string confirm, IContext context) =>
            context.WithDeleted(group);

        public static string ValidateDeleteGroup(this Group group, [DescribedAs("type DELETE GROUP")] string confirm, IContext context) =>
            confirm == "DELETE GROUP" ? null : "Must type DELETE GROUP into Confirm field";
    }
}
