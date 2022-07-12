import { defaultRegExp, validateExpressionWithRegex } from "./language-helpers";

export function wrapPython3(code : string) {
    return `print (${code})`;
}

export function validatePython3Expression(code : string) {
    return validateExpressionWithRegex(code, defaultRegExp);
}
