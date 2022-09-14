using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Model
{
    //A helper class to provide model configuration for use by Startup.cs in the Server project.
    //The implementation here relies on the conventions that:
    //- All domain classes are defined in namespace "Model.Types"
    //- All domain functions are defined on static types in namespace "Model.Functions"
    //- All main menu function are defined on static types that have 'MenuFunctions' in their name
    //This ModelConfig may be re-written to change the conventions, or to remove conventions altogether, and
    //specify the lists of types, functions, and menus explicitly.
    public static class ModelConfig
    {
        public static Type[] DomainTypes() =>
          DomainTypes("Model.Types").ToArray();

        public static Type[] TypesDefiningDomainFunctions() =>
          PublicStaticClasses("Model.Functions").ToArray();

    public static Type[] MainMenus() =>
        PublicStaticClasses("Model.Functions.Menus").ToArray();

        public static Func<IConfiguration, DbContext> EFCoreDbContextCreator =>
            c => {
                var db = new AdminDbContext(c.GetConnectionString("ILEAdmin"));
                //db.Create();
                return db;
            };

        #region Helpers

        private static IEnumerable<Type> PublicStaticClasses(string nameSpace) =>
            PublicClasses(nameSpace).Where(t => t.IsAbstract && t.IsSealed);

        private static IEnumerable<Type> PublicClasses(string namespaceStarting) =>
            typeof(ModelConfig).Assembly.GetTypes().Where(t => t.IsPublic && t.IsClass && t.Namespace.StartsWith(namespaceStarting));

        private static IEnumerable<Type> DomainTypes(string nameSpace) =>
            typeof(ModelConfig).Assembly.GetTypes().Where(t => 
                t.Namespace == nameSpace &&
                t.IsPublic && 
                ((t.IsClass && !t.IsAbstract && !t.IsSealed) || t.IsInterface || t.IsEnum));

        #endregion
    }
}
