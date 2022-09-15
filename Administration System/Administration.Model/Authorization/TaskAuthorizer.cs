﻿using NakedFunctions.Security;

namespace Model.Authorization
{
    public class TaskAuthorizer : ITypeAuthorizer<Task>
    {
        public bool IsVisible(Task task, string memberName, IContext context) =>
            task.IsPublic() && Helpers.MemberIsProperty(task, memberName) ? true :
                Users.UserRole(context) switch
                {
                    Role.Root => true,
                    Role.Author => AuthorAuthorization(task, memberName, context),
                    >= Role.Teacher  => TeacherAuthorization(task, memberName, context),
                    Role.Student => StudentAuthorization(task, memberName, context),
                    _ => false
                };

        private bool AuthorAuthorization(Task task, string memberName, IContext context) => 
           task.IsAssignable() || task.AuthorId == Users.Me(context).Id;

        private bool TeacherAuthorization(Task task, string memberName, IContext context) => 
           task.IsAssignable() && (!(memberName.StartsWith("Edit") || memberName.StartsWith("Add")));

        private bool StudentAuthorization(Task task, string memberName, IContext context) =>
            Helpers.MemberIsProperty(task, memberName) && task.IsAssignedToCurrentUser(context);
    }
}
