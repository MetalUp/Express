import { defaultRegExp, filterCmpinfoWithRegex, filterStderrWithRegex, findFunctionsWithRegex, FunctionPlaceholder, validateExpressionWithRegex } from "./language-helpers";

export function wrapPythonExpression(expression : string) {
    return `
from math import *

${FunctionPlaceholder}

def display(x):
  if isinstance(x, str):
    return x
  else:
    try:
         return list(iter(x))
    except TypeError:
         return x

print (display(${expression}))
`;
}

export function wrapPythonFunctions(functions : string) {
    return functions;
}

export function validatePythonExpression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}

export function findPythonFunctions(expression : string) {
    const fMatch = /([A-Za-z]\w*\s*)\(/g;
    return findFunctionsWithRegex(expression, fMatch);
}