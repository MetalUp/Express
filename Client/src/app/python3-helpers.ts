import { defaultRegExp, validateExpressionWithRegex } from "./language-helpers";

export function wrapPython3Expression(expression : string) {
    return `print (${expression})`;
}

export function validatePython3Expression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}
