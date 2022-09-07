using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Functions
{
    public static class Assignment_Functions
    {
        #region Activity
        public static IQueryable<Activity> ListActivity(this Assignment assignment, IContext context)
        {
            int assigId = assignment.Id;
            return context.Instances<Activity>().Where(a => a.AssignmentId == assigId).OrderByDescending(a => a.TimeStamp);
        }

        #endregion

        //    public static IContext MarkNotCompleted(this Assignment a, string teacherNote, IContext context) =>
        //        context.WithNew(new Activity() { Assignment = a, TimeStamp = context.Now(), Type = ActivityType.NotCompleted, Details = teacherNote })
        //        .WithUpdated(a, new Assignment(a) { Status = ActivityType.NotCompleted });

        //    public static IContext MarkTasksNotCompleted(this IQueryable<Assignment> assignments, string teacherNote, IContext context) =>
        //      assignments.Aggregate(context, (c, a) => MarkNotCompleted(a, teacherNote, c));

        //    Called when the assignee navigates from the assignment to view of the task itself
        //    public static IContext StartAssigment(this Assignment a, IContext context) =>
        //        context.WithNew(new Activity() { Assignment = a, TimeStamp = context.Now(), Type = ActivityType.Started })
        //        .WithUpdated(a, new Assignment(a) { Status = ActivityType.Started });

    }
}
