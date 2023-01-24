using NakedFunctions.Security;

namespace Model.Authorization;

public class FileViewModelAuthorizer : ITypeAuthorizer<FileViewModel>
{
    public bool IsVisible(FileViewModel fvm, string memberName, IContext context) => true;
}