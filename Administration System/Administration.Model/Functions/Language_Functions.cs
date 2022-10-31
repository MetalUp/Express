namespace Model.Functions
{
    public static class Language_Functions
    {
        #region Editing
        [Edit]
        public static IContext EditWrapperFile(
         this Language language,
         File wrapperFile,
         IContext context) =>
             context.WithUpdated(language, new(language) { WrapperFileId = wrapperFile.Id, WrapperFile = wrapperFile});

        [Edit]
        public static IContext EditHelpersFile(
 this Language language,
 File helpersFile,
 IContext context) =>
     context.WithUpdated(language, new(language) { HelpersFileId = helpersFile.Id, HelpersFile = helpersFile });

        [Edit]
        public static IContext EditRegExRulesFile(
 this Language language,
 File regExRulesFile,
 IContext context) =>
     context.WithUpdated(language, new(language) { RegExRulesFileId = regExRulesFile.Id, RegExRulesFile = regExRulesFile });

        #endregion
    }
}
