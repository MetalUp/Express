import {  UserDefinedFunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from "./language-helpers";

export function wrapCSharpExpression(expression : string) {
    return `
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    class MainWrapper {
        ${ReadyMadeFunctionsPlaceholder}

        ${UserDefinedFunctionPlaceholder}

        private static string Display(object obj)
        {
            if (obj == null)  return null;
            if (obj is string) return $"{obj}";
            if (obj is Boolean) return (Boolean) obj ? "true" : "false";
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

export function wrapCSharpFunctions(userDefinedFunction : string) {
    return `
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using static MainWrapper.HiddenFunctions;

    
    class MainWrapper{
        ${ReadyMadeFunctionsPlaceholder}

        ${userDefinedFunction}

        static void Main(string[] args) {}
    }
    `;
}

export function wrapCSharpTests(tests : string) {
    return `
        using System;
        using System.Linq;
        using System.Collections;
        using System.Collections.Generic;
        using static HiddenFunctions;
        using static UserFunctions;
        using static TestFunctions;
        using Microsoft.VisualStudio.TestTools.UnitTesting;

        ${ReadyMadeFunctionsPlaceholder}
        
        public static class UserFunctions {
          public ${UserDefinedFunctionPlaceholder}
        }

        public static class TestFunctions {
            public static string Display(object obj)
            {
                if (obj == null) return null;
                if (obj is string) return $"{obj}";
                if (obj is Boolean) return (Boolean)obj ? "true" : "false";
                if (obj is IEnumerable)
                {
                    var display = ((IEnumerable)obj).Cast<object>().Select(o => Display(o));
                    return $@"{{{string.Join(',', display)}}}";
                }
                return obj.ToString();
            }

            public static string ArgString(params object[] arguments) => arguments.Aggregate("", (s, a) => s + Display(a) + ", ").TrimEnd(' ', ',');

            public static void TestFunction(string functionName, object expected, object actual, params object[] args)
            {
                Assert.AreEqual(Display(expected), Display(actual),
                    $" Calling {functionName}({ArgString(args)})");
            }
        }

        ${tests}
    `;
}