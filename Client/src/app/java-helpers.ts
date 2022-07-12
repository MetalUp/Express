import { defaultRegExp, validateExpressionWithRegex } from "./language-helpers";

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