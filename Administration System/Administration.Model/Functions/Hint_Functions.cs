using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Functions
{
    public static class Hint_Functions
    {

        #region Editing
        [Edit]
        public static IContext EditTitle(
            this Hint hint,
            string title,
            IContext context) =>
        context.WithUpdated(hint, new(hint) { Title = title });

        [Edit]
        public static IContext EditHtmlFile(
            this Hint hint,
            string htmlFile,
            IContext context) =>
        context.WithUpdated(hint, new(hint) { HtmlFile = htmlFile });

        [Edit]
        public static IContext EditCostInMarks(
            this Hint hint,
            int costInMarks,
            IContext context) =>
        context.WithUpdated(hint, new(hint) { CostInMarks = costInMarks });

        #endregion
    }
}
