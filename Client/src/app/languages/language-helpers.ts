import { filterCSharpCmpinfo, filterCSharpStderr, findCSharpFunctions, validateCSharpExpression, wrapCSharpExpression, wrapCSharpFunctions } from "./csharp-helpers";
import { filterPythonCmpinfo, filterPythonStderr, findPythonFunctions, validatePythonExpression, wrapPythonExpression, wrapPythonFunctions } from "./python-helpers";

export const FunctionPlaceholder = "<<FunctionsPlaceholder>>";

export function wrapExpression(language: string, expression: string) {
    switch (language) {
        case 'csharp': return wrapCSharpExpression(expression);
        case 'python': return wrapPythonExpression(expression);
        default: return expression;
    }
}

export function wrapFunctions(language: string, expression: string) {
    switch (language) {
        case 'csharp': return wrapCSharpFunctions(expression);
        case 'python': return wrapPythonFunctions(expression);
        default: return expression;
    }
}

export function validateExpression(language: string, expression: string, whiteListFunctions: string[]) {

    // validate all the functions
    // const invalidFunctions = findFunctions(language, expression).filter(f => !whiteListFunctions.includes(f));
    // if (invalidFunctions.length > 0) {
    //     return "may not use function(s): " + invalidFunctions.join(", ");
    // }

    switch (language) {
        case 'csharp': return validateCSharpExpression(expression);
        case 'python': return validatePythonExpression(expression);
        default: return 'unknown language';
    }
}

export function filterCmpinfo(language: string, cmpinfo: string) {
    switch (language) {
        case 'csharp': return filterCSharpCmpinfo(cmpinfo);
        case 'python': return filterPythonCmpinfo(cmpinfo);
        default: return cmpinfo;
    }
}

export function filterStderr(language: string, stderr: string) {
    switch (language) {
        case 'csharp': return filterCSharpStderr(stderr);
        case 'python': return filterPythonStderr(stderr);
        default: return stderr;
    }
}

function findFunctions(language: string, expression: string) {
    switch (language) {
        case 'csharp': return findCSharpFunctions(expression);
        case 'python': return findPythonFunctions(expression);
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

export function filterStderrWithRegex(stderr: string, re: RegExp) {
    var m = re.exec(stderr);
    return m ? m[0] : stderr;
}

export function findFunctionsWithRegex(expression : string, re: RegExp) {
    var m = expression.match(re) || [];
    return m.map(f => f.replace("(", '').trim());
}