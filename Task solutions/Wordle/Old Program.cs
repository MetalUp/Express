using static CSharp.Wordle;
using static CSharp.Reference;
using static CSharp.MyData;
using CSharp;
using System.Diagnostics;


////PLAY GAME
//var outcome = "";
//var possible = AllPossibleAnswers;
//var attempt = "RAISE";
//while (outcome != "*****")
//{
//    Console.WriteLine(attempt);
//    outcome = Console.ReadLine();
//    possible = RemainingValidWords(possible, attempt, outcome);
//    Console.WriteLine($"{possible.Count} remaining");
//    attempt = BestAttempt(possible, AllWords); 
//}


//PLAY GAME - MINIMAL
var possible = AllPossibleAnswers;
var outcome = "";
while (outcome != "*****")
{
    var attempt = BestAttempt(possible, AllWords);
    Console.WriteLine(attempt);
    outcome = Console.ReadLine();
    possible = RemainingValidWords(possible, attempt, outcome);
    Console.WriteLine($"{possible.Count} remaining");
}

//FIND THE WORDS THAT TAKE 5
//foreach (var target in PossibleAnswers)
//{
//    var result = "";
//    var history = "";
//    var attempt = "RAISE";
//    var possible = PossibleAnswers;
//    int count = 0;
//    while (result != "*****")
//    {
//        history += attempt + ",";
//        result = MarkAttempt(attempt, target);
//        count++;
//        possible = RemainingValidWords(possible, attempt, result);
//        attempt = BestAttempt(possible, AllWords);
//    }
//    if (count > 4)
//    Console.WriteLine(history);
//}

//Sort 5 attempt words
//var sorted = FiveAttemptWords.OrderBy(w => w.Substring(6, 5)).ThenBy(w => w.Substring(12, 5)).ThenBy(w => w.Substring(18, 5));
//var grouped = sorted.GroupBy(w => w.Substring(6, 5)).ToList();
//var ordered = grouped.OrderByDescending(g => g.Count()).ToList();
//foreach(var g in ordered)
//{
//    Console.WriteLine();
//    //Console.WriteLine($"{g.Key} {g.Count()}");
//    foreach(var w in g)
//    {
//        Console.WriteLine(w);
//    }
//}





//var round1 = RemainingValidWords(AllWords, "RAISE", "__*_*");
//var round2 = RemainingValidWords(round1, "CHILE", "+_*_*");

//var round3 = RemainingValidWords(round2, "VOICE", "__***");
//var m = MarkAttempt("CHILE", "TWICE");

//var remain2 = RemainingWordCountLeftByWorstOutcome(round2, "CHILE");

//var best = BestAttempt(round3);

//Console.WriteLine($" {best}");



//NEW INVESTIGATION - 26th October

//Starting with RAISE ...
//Foreach (of 238) possible outcomes...
//Determine the best next attempt & calculate remaining possible wordcount from the worst outcome from that attempt
//Look for - does the least worst outcome after 2 attempts follow on from the worst outcome from the first event? Or not?
//If so, the first attempt is optimal. In which case apply the next level down but from worst outcome from attempt 1.
//If not, it MIGHT not be - worth re-evaluating the first attempt to get the least worst after 2.
//var result = "";
//var possible = PossibleAnswers;
//var attempt = "";
//var dict = new Dictionary<string, int>();

////var possible2 = RemainingValidWords(PossibleAnswers, "RAISE", "__*__");
////var attempt2 = BestAttempt(possible2, AllWords);

//foreach (var outcome in Outcomes)
//{
//    int count = RemainingValidWords(PossibleAnswers, "RAISE", outcome).Count;
//    dict[outcome] = count;
//}

//var sorted = dict.OrderByDescending(kvp => kvp.Value);
//var total = dict.Sum(kvp => kvp.Value);
//Console.WriteLine($"Total words left after round 1 {total}");
//foreach (var kvp in sorted)
//{
//    Console.WriteLine($"{kvp.Key} {kvp.Value}");
//}
//Console.ReadKey();

//WORST OUTCOMES FROM RAISE as first attempt
//Outcome, remaining words, bestattempt2, worst outcome after 2
//"_____",167, BLUDY, 13
//"____+",120, DENET, 9
//"__+__",107, UNTIL, 11
//"+____",103, COLON, 10
//"+___+",102, OUTER, 16
//"_+___",91,  CLOAK, 7
//"_*___",91,  BUNTY, 11
//"___+_",80,  MUTON, 7
//"++___",77,  TRONC, 9
//"_+__+",69,  METAL, 5
//"____*",61,  CLOUD, 5
//"__*__",51,  CLOUT, 5

