import { filterCSharpCmpinfo, findCSharpFunctions, validateCSharpExpression, wrapCSharpEXpression as wrapCSharpExpression } from "./csharp-helpers";
import { filterJavaCmpinfo, findJavaFunctions, validateJavaExpression, wrapJavaEXpression as wrapJavaExpression } from "./java-helpers";
import { filterPython3Cmpinfo, findPython3Functions, validatePython3Expression, wrapPython3Expression } from "./python3-helpers";
import { filterVBNetCmpinfo, findVBNetFunctions, validateVBNetExpression, wrapVBNetExpression } from "./vbnet-helpers";


export function wrapExpression(language: string, expression: string) {
    switch (language) {
        case 'csharp': return wrapCSharpExpression(expression);
        case 'java': return wrapJavaExpression(expression);
        case 'python3': return wrapPython3Expression(expression);
        case 'vbnet': return wrapVBNetExpression(expression);
        default: return expression;
    }
}

export function validateExpression(language: string, expression: string, whiteListFunctions: string[]) {

    // validate all the functions
    const invalidFunctions = findFunctions(language, expression).filter(f => !whiteListFunctions.includes(f));
    if (invalidFunctions.length > 0) {
        return "may not use function(s): " + invalidFunctions.join(", ");
    }

    switch (language) {
        case 'csharp': return validateCSharpExpression(expression);
        case 'java': return validateJavaExpression(expression);
        case 'python3': return validatePython3Expression(expression);
        case 'vbnet': return validateVBNetExpression(expression);
        default: return 'unknown language';
    }
}

export function filterCmpinfo(language: string, cmpinfo: string) {
    switch (language) {
        case 'csharp': return filterCSharpCmpinfo(cmpinfo);
        case 'java': return filterJavaCmpinfo(cmpinfo);
        case 'python3': return filterPython3Cmpinfo(cmpinfo);
        case 'vbnet': return filterVBNetCmpinfo(cmpinfo);
        default: return cmpinfo;
    }
}

function findFunctions(language: string, expression: string) {
    switch (language) {
        case 'csharp': return findCSharpFunctions(expression);
        case 'java': return findJavaFunctions(expression);
        case 'python3': return findPython3Functions(expression);
        case 'vbnet': return findVBNetFunctions(expression);
        default: return [];
    }
}



export const defaultRegExp = /^.*$/;

export function validateExpressionWithRegex(expression: string, re: RegExp) {
    return re.test(expression) ? '' : `${expression} is not an expression`;
}

export function filterCmpinfoWithRegex(cmpinfo: string, re: RegExp) {
    var m = re.exec(cmpinfo);
    return m ? m[0] : cmpinfo;
}

export function findFunctionsWithRegex(expression : string, re: RegExp) {
    var m = expression.match(re) || [];
    return m.map(f => f.replace("(", "").trim());
}