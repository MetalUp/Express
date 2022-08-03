import {  FunctionPlaceholder, TaskCodePlaceholder } from "./language-helpers";

export function wrapCSharpExpression(expression : string) {
    return `
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    class MainWrapper {
        ${TaskCodePlaceholder}

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
        ${TaskCodePlaceholder}

        static void Main(string[] args) {}
    }

    static class UserDefinedFunctions {
        ${functions}
    }
    `;
}

export function wrapCSharpTests(tests : string) {
    return `
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    
    class MainWrapper{
        public static string fail = "Test failed calling ";

        public static string ArgString(params object[] arguments) => arguments.Aggregate("", (s, a) => s + Display(a) + ", ").TrimEnd(' ', ',');
    
        public static void TestFunction(string functionName, object expected, object actual, params object[] args)
        {
            if (Display(actual) != Display(expected))
            {
                Console.WriteLine(fail + $"{functionName}({ArgString(args)}) Expected: {Display(expected)}  Actual: {Display(actual)}");
                throw new TestFailure();
            }
        }
    
        public static void AssertTrue(string functionName, string args, bool actual, string message)
        {
            if (actual != true)
            {
                Console.WriteLine(fail +$"{functionName}({ArgString(args)}) {message}");
                throw new TestFailure();
            }
        }
    
        public static string allTestsPassed = "All tests passed.";
    
        public static void AllTestsPassed()
        {
            Console.WriteLine(allTestsPassed);
        }
    
        public class TestFailure : Exception { }

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

        ${FunctionPlaceholder}

        ${tests}

        static void Main(string[] args) {
            RunTests();
        }
    }
    `;
}