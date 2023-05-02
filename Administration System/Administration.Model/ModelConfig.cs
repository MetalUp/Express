namespace Model
{
    //A helper class to provide model configuration for use by Startup.cs in the Server project.
    //The implementation here relies on the conventions that:
    //- All domain classes are defined in namespace "Model.Types"
    //- All domain functions are defined on static types in namespace "Model.Functions"
    //- All main menu functionS are defined on static types that are in the namespace Model.Menus
    //This ModelConfig may be re-written to change the conventions, or to remove conventions altogether, and
    //specify the lists of types, functions, and menus explicitly.
    public static class ModelConfig
    {
        public static Type[] DomainTypes() =>
            DomainTypes("Model.Types").ToArray();

        public static Type[] TypesDefiningDomainFunctions() =>
            ImmutableList.Create<Type>().AddRange(Functions()).AddRange(DomainServices()).AddRange(MainMenus()).ToArray();    

        public static Type[] Functions() =>
            PublicStaticClasses("Model.Functions").ToArray();

        public static Type[] DomainServices() =>
            PublicStaticClasses("Model.Services").ToArray();

        public static Type[] MainMenus() =>
            PublicStaticClasses("Model.Menus").ToArray();

        #region Helpers

        private static IEnumerable<Type> PublicStaticClasses(params string[] namespaces) =>
            PublicClasses(namespaces).Where(t => t.IsAbstract && t.IsSealed);

        private static IEnumerable<Type> PublicClasses(params string[] namespaces) =>
            typeof(ModelConfig).Assembly.GetTypes().Where(t => t.IsPublic && t.IsClass && namespaces.Any(n =>t.Namespace != null && t.Namespace.StartsWith(n)));

        private static IEnumerable<Type> DomainTypes(string nameSpace) =>
            typeof(ModelConfig).Assembly.GetTypes().Where(t => 
                t.Namespace == nameSpace &&
                t.IsPublic && 
                ((t.IsClass && !t.IsAbstract && !t.IsSealed) || t.IsInterface || t.IsEnum));

        #endregion
    }
}
