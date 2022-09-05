using static CSharp.Wordle;
using static CSharp.Reference;

var rand = new Random();

//var target = "SILLS";
var words = AllWords;
//var attempt = "RAISE";
//while (true)
//{
//    var result = MarkAttempt(attempt, target);
//    Console.Write($"{attempt}: {result}");
//    words = RemainingValidWords(words, attempt, result);
//    foreach (var w in words) Console.Write($"{w} ");
//    Console.WriteLine();
//    var best1 = BestAttempt(words);
//    var worst1 = RemainingWordCountLeftByWorstOutcome(words, best1);
//    var best2 = BestAttemptFromAllWords(words, AllWords);
//    var worst2 = RemainingWordCountLeftByWorstOutcome(words, best2);
//    attempt = worst2 < worst1 ? best2 : best1;
//    Console.ReadKey();
//}
//Select the word at random
//var words = AllWords;
string result = "";
int count = 0;

var attempt = "RAISE";
while (result != "*****")
{
    Console.WriteLine(attempt);
    result = Console.ReadLine();
    words = RemainingValidWords(words, attempt, result);
    attempt = BestAttempt(words, AllWords);
}





//var round1 = RemainingValidWords(AllWords, "RAISE", "__*_*");
//var round2 = RemainingValidWords(round1, "CHILE", "+_*_*");

//var round3 = RemainingValidWords(round2, "VOICE", "__***");
//var m = MarkAttempt("CHILE", "TWICE");

//var remain2 = RemainingWordCountLeftByWorstOutcome(round2, "CHILE");

//var best = BestAttempt(round3);



//Console.WriteLine($" {best}");


