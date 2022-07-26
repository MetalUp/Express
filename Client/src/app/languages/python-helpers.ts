import { FunctionPlaceholder } from "./language-helpers";

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
