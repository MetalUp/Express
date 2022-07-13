import { defaultRegExp, filterCmpinfoWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapPython3Expression(expression : string) {
    return `print (${expression})`;
}

export function validatePython3Expression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}

const cmpInfoRegex = /SyntaxError.*/

export function filterPython3Cmpinfo(cmpinfo : string) {
    return filterCmpinfoWithRegex(cmpinfo, cmpInfoRegex);
}
