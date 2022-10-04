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
    public class temp {
        public static String fail = "Test failed calling ";

        public static String ArgString(Object... arguments) {
            String display = "";
            for(Object o : arguments) {
                display = display + ", " + Display(o);
            }
            return display;
        } 
    
        public static void TestFunction(String functionName, Object expected, Object actual, Object... args)
        {
            if (Display(actual) != Display(expected))
            {
                System.out.println(fail + functionName + ArgString(args) + " Expected: " + Display(expected) + " Actual: " + Display(actual));
                throw new TestFailure();
            }
        }
    
        public static void AssertTrue(String functionName, String args, Boolean actual, String message)
        {
            if (actual != true)
            {
                System.out.println(fail + functionName + ArgString(args) + " " + message);
                throw new TestFailure();
            }
        }
    
        public static String allTestsPassed = "All tests passed.";
    
        public static void AllTestsPassed()
        {
            System.out.print(allTestsPassed);
        }
    
        public static class TestFailure extends Exception { }

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

        ${ReadyMadeFunctionsPlaceholder}

        ${UserDefinedFunctionPlaceholder}

        ${tests}

        static void main(String[] args) {
            RunTests();
        }
    }
    `;
}