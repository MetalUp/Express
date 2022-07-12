import { defaultRegExp, validateExpressionWithRegex } from "./language-helpers";

export function wrapJava(code : string) {
    return `
    class prog {
        public static void main(String[] args) {
            System.out.println(${code}); 
        }
    }`;
}

export function validateJavaExpression(code : string) {
    return validateExpressionWithRegex(code, defaultRegExp);
}