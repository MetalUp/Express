import { wrapCSharpExpression, wrapCSharpFunctions } from "./csharp-helpers";
import { wrapPythonExpression, wrapPythonFunctions } from "./python-helpers";

export const FunctionPlaceholder = "<<FunctionsPlaceholder>>";
export const TaskCodePlaceholder = "<<TaskCodePlaceholder>>";

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