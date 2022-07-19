import { defaultRegExp, filterCmpinfoWithRegex, findFunctionsWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapJavaExpression(expression : string) {
    return `
    import java.util.*;
    import java.lang.*;
    
    class prog {

        private static String display(Object obj) {
            if ((obj == null)) {
                return null;
            }
            
            if ((obj instanceof String)) {
                return '"' + obj.toString() + '"';
            }
            
            if ((obj instanceof Iterable)) {
                List<String> display = new ArrayList<String>();

                for(Object elem : (Iterable)obj){
                    display.add(display(elem));
                }

                return "{" +  String.join(",", display) + "}";
            }
            
            return obj.toString();
        }

        public static void main(String[] args) {
            System.out.println(display(${expression})); 
        }
    }`;
}

export function validateJavaExpression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}

const cmpInfoRegex = /error.*/

export function filterJavaCmpinfo(cmpinfo : string) {
    return filterCmpinfoWithRegex(cmpinfo, cmpInfoRegex);
}

export function findJavaFunctions(expression : string) {
    const fMatch = /([a-z]\w*\s*)\(/g;
    return findFunctionsWithRegex(expression, fMatch);
}