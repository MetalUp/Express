﻿namespace Model.Functions
{
    public static class Language_Functions
    {
        #region Editing
        [Edit]
        public static IContext EditWrapperFile(
         this Language language,
         [Optionally] File wrapperFile,
         IContext context) =>
             context.WithUpdated(language, new(language) { WrapperFileId = wrapperFile.Id, WrapperFile = wrapperFile });

        [Edit]
        public static IContext EditRegExRulesFile(
         this Language language,
         [Optionally] File regExRulesFile,
         IContext context) =>
             context.WithUpdated(language, new(language) { RegExRulesFileId = regExRulesFile.Id, RegExRulesFile = regExRulesFile });

        [Edit]
        public static IContext EditName(this Language language, string name, IContext context) =>
            context.WithUpdated(language, new(language) { Name = name });

        [Edit]
        public static IContext EditCSSstyle(this Language language, string cssStyle, IContext context) =>
    context.WithUpdated(language, new(language) { CSSstyle = cssStyle });

        [Edit]
        public static IContext EditCompilerLanguageId(this Language language, string compilerLanguageId, IContext context) =>
context.WithUpdated(language, new(language) { CompilerLanguageId = compilerLanguageId });

       [Edit]
        public static IContext EditCompileArguments(this Language language, string compileArguments, IContext context) =>
            context.WithUpdated(language, new(language) { CompileArguments = compileArguments });


        #endregion
    }
}
