import { defaultRegExp, filterCmpinfoWithRegex, findFunctionsWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapPython3Expression(expression : string) {
    return `
    from math import *
    
    print (${expression})`;
}

export function validatePython3Expression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}

const cmpInfoRegex = /SyntaxError.*/

export function filterPython3Cmpinfo(cmpinfo : string) {
    return filterCmpinfoWithRegex(cmpinfo, cmpInfoRegex);
}

export function findPython3Functions(expression : string) {
    const fMatch = /([A-Za-z]\w*\s*)\(/g;
    return findFunctionsWithRegex(expression, fMatch);
}