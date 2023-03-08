﻿namespace Model.Functions
{
    public static class Language_Functions
    {
        #region Editing
        [Edit]
        public static IContext EditWrapperFile(
         this Language language,
         File wrapperFile,
         IContext context) =>
             context.WithUpdated(language, new(language) { WrapperFileId = wrapperFile.Id, WrapperFile = wrapperFile });

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

        [Edit]
        public static IContext EditName(this Language language, string name, IContext context) =>
            context.WithUpdated(language, new(language) { Name = name });

        [Edit]
        public static IContext EditCSSstyle(this Language language, string cssStyle, IContext context) =>
    context.WithUpdated(language, new(language) { CSSstyle = cssStyle });

        [Edit]
        public static IContext EditVersion(this Language language, string version, IContext context) =>
context.WithUpdated(language, new(language) { Version = version });


        [Edit]
        public static IContext EditFileExtension(this Language language, string fileExtension, IContext context) =>
context.WithUpdated(language, new(language) { FileExtension = fileExtension });

        [Edit]
        public static IContext EditCompileArguments(this Language language, string compileArguments, IContext context) =>
            context.WithUpdated(language, new(language) { CompileArguments = compileArguments });


        #endregion
    }
}
