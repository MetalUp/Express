import {  UserDefinedFunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from "./language-helpers";

export function wrapJavaExpression(expression : string) {
    return `
    public class temp {
        ${ReadyMadeFunctionsPlaceholder}

        ${UserDefinedFunctionPlaceholder}

        private static String Display(Object obj)
        {
            if (obj == null)  return null;
            if (obj instanceof String) return (String)obj;
            if (obj instanceof Boolean) return (Boolean) obj ? "true" : "false";
            if (obj instanceof Iterable)
            {
                String display = "";
                for(Object i : (Iterable)obj ){
                    display = display + "," + Display(i);
                }
                return display;
            }
            return obj.toString();
        }

        public static void main(String[] args) {
           System.out.println(Display(${(expression)}));
        }
    }`;
}

export function wrapJavaFunctions(userDefinedFunction : string) {
    return `
    public class temp {
        ${ReadyMadeFunctionsPlaceholder}

        ${userDefinedFunction}

        public static void main(String[] args) {}
    }
    `;
}

export function wrapJavaTests(tests : string) {
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
            Console.Write(allTestsPassed);
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

        ${ReadyMadeFunctionsPlaceholder}

        ${UserDefinedFunctionPlaceholder}

        ${tests}

        static void Main(string[] args) {
            RunTests();
        }
    }
    `;
}