using NakedFunctions.Security;

namespace Model.Authorization
{
    public class HintAuthorizer : ITypeAuthorizer<Hint>
    {
        //TODO: This is a possible approach, but needs careful testing
        public bool IsVisible(Hint hint, string memberName, IContext context) =>
            (new TaskAuthorizer()).IsVisible(hint.Task, memberName, context);

    }
}
