import {  FunctionPlaceholder } from "./language-helpers";

export function wrapCSharpExpression(expression : string) {
    return `
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    class MainWrapper {

        private static string Display(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is string)
            {
                return $"{obj}";
            }

            if (obj is IEnumerable)
            {
                var display = ((IEnumerable)obj).Cast<object>().Select(o => Display(o));
                return $@"{{{string.Join(',', display)}}}";
            }

            return obj.ToString();
        }

        static void Main(string[] args) {
           System.Console.WriteLine(Display(${(expression)}));
        }

       ${FunctionPlaceholder}
    }`;
}

export function wrapCSharpFunctions(functions : string) {
    return `
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    
    class MainWrapper{
        static void Main(string[] args) {}
    }

    static class UserDefinedFunctions {
        ${functions}
    }
    `;
}