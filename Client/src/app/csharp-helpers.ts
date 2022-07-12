export function wrapCSharp(code : string) {
    return `
    class Hello {
        static void Main(string[] args) {
           System.Console.WriteLine(${code});
       }
    }`;
}

export function wrapJava(code : string) {
    return `
    class prog {
        public static void main(String[] args) {
            System.out.println(${code}); 
        }
    }`;
}

export function wrapPython3(code : string) {
    return `print (${code})`;
}

export function wrapVBNet(code : string) {
    return `
    Module Hello  
    Sub Main()  
        System.Console.WriteLine(${code})
    End Sub  
    End Module`;
}

export function wrap(language : string, code : string) {
    switch (language) {
        case 'csharp' : return wrapCSharp(code);
        case 'java' : return wrapJava(code);
        case 'python3' : return wrapPython3(code);
        case 'vbnet' : return wrapVBNet(code);
        default : return code;
    }
}