import { defaultRegExp, filterCmpinfoWithRegex, findFunctionsWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapJavaEXpression(expression : string) {
    return `
    class prog {
        public static void main(String[] args) {
            System.out.println(${expression}); 
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