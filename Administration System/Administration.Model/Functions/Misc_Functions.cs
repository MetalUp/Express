
namespace Model.Functions
{
    public static class Misc_Functions
    {
        //public static (Student, Group, IContext) AddStudentToGroup(Student student, Group group, IContext context) => throw new Exception();

        //AssignTaskToStudent
        //AssignTaskToGroup
        //ShowActiveAssignments(Group group, [Optionally] DateTime dueBy)
        //ShowActiveAssignments(Student student, [Optionally] DateTime dueBy)
        //ShowAssignments(Group group, DateTime fromDate, [Optionally], DateTime toDate, [Optionally] byTeacher)

        //ShowActiveTasks(Student student)
        //ListTasks(Student student, 
        //ListActivity(Task task)
        //MarkAsNotCompleted(Assignment assignment) //Only available to teachers - just to take off active list

        //Invitation functions
        //Invite Student - made by user in a Teacher role on behalf of their organisation
        //View outstanding invitations
        //Send reminder (to one or many)
        //Rescind outstanding (one or many)
        //Accept invitation - made by invitee, changes User role and adds to organisation

        //Group functions
        //Create New Group (for an organisation)
        //Assign Students (or Teachers) associated from the organisation, or from another group 
        //De-assign students (or teachers) from group
        //Edit group name / description

        //Task assignment
        //Assign task to individual user or to all students in a group
        //Find tasks assigned to group, with filtering by status and/or dates, ordered by s
        //Find all tasks due by group
        //Find all tasks overdue, optionally by group

    }
}
