import { defaultRegExp, filterCmpinfoWithRegex, findFunctionsWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapCSharpExpression(expression : string) {
    return `
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    
    class Hello {

        private static string Display(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is string)
            {
                return $@"""{obj}""";
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
    }`;
}

export function validateCSharpExpression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}

const cmpInfoRegex = /CS.*/

export function filterCSharpCmpinfo(cmpinfo : string) {
    return filterCmpinfoWithRegex(cmpinfo, cmpInfoRegex);
}

export function findCSharpFunctions(expression : string) {
    const fMatch = /([A-Z]\w*\s*)\(/g;
    return findFunctionsWithRegex(expression, fMatch);
}