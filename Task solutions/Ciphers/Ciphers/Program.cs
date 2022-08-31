// See https://aka.ms/new-console-template for more information
using System.Linq;

//string d = "JGDQOXUSCAMIFRVTPNEWKBLZYH";
//foreach(char c in d)
//{
//    Console.Write(AsInt(c) + ",");
//}

while (true)
{
    string msg = Console.ReadLine();
    Console.WriteLine(EncryptString(msg, d1));
}

int Caesar1(int x) => (x + 1) % 26;
int Caesar1Decrypt(int x) => (x -1) % 26;

int AsInt(char c) => Char.ToUpper(c)-65;
char AsChar(int x) => (char) (x+65);

char EncryptChar(char c, Func<int, int> f) => AsChar(f(AsInt(c)));

string EncryptString(string s, Func<int, int> f) => s.ToCharArray().Select(c => EncryptChar(c,f)).Aggregate("", (c,s) => c+s);



int d1(int x) => (new int[] {9, 6, 3, 16, 14, 23, 20, 18, 2, 0, 12, 8, 5, 17, 21, 19, 15, 13, 4, 22, 10, 1, 11, 25, 24, 7})[x];