using static CSharp.Wordle;
using static CSharp.Reference;


//Generate list of possible outcomes
//var symbols = "_+*".ToCharArray();
//foreach (var s0 in symbols)
//{
//    foreach (var s1 in symbols)
//    {
//        foreach (var s2 in symbols)
//        {
//            foreach (var s3 in symbols)
//            {
//                foreach (var s4 in symbols)
//                {
//                    var outcome = $"{s0}{s1}{s2}{s3}{s4}";
//                    Console.Write($"\"{outcome}\",");
//                }
//            }
//        }
//    }
//}
//Console.ReadLine();

//var x =PossibleAnswers.Where(p => !AllWords.Contains(p)).ToList();
//foreach(var w in x)
//{
//    Console.Write($"\"{w}\",");
//}
//Console.ReadLine();

//var result = "";

//var analysis = new double[10];
//foreach (var target in PossibleAnswers)
//{
//    result = "";
//    var attempt = "ARISE";
//    var possible = PossibleAnswers;
//    int count = 0;
//    while (result != "*****")
//    {
//        Console.Write($"{attempt} ");
//        result = MarkAttempt(attempt, target);
//        count++;
//        possible = RemainingValidWords(possible, attempt, result);
//        attempt = BestAttempt(possible, AllWords);
//    }
//    analysis[count]++;
//    Console.WriteLine();
//}
//int words = PossibleAnswers.Count();
//double weighted = 0;
//for (int i = 0; i < analysis.Length; i++)
//{
//    weighted += i * analysis[i];
//    Console.WriteLine($"{i} - {analysis[i]} - {analysis[i] / words}");
//}
//Console.WriteLine($"{weighted / words}");

var result = "";

while (true)
{
    var possible = PossibleAnswers;
    var attempt = "RAISE";
    while (result != "*****")
    {
        int after = RemainingWordCountLeftByWorstOutcome(possible, attempt);
        Console.WriteLine($"{attempt} ({possible.Count} -> {after})");
        result = Console.ReadLine();

        possible = RemainingValidWords(possible, attempt, result);
        attempt = BestAttempt(possible, AllWords);

    }
    Console.Write("Press any key for new game");
}




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

//Console.WriteLine("First attempt: RAISE");
////foreach (var outcome1 in Outcomes)
////{
//    var possible1 = RemainingValidWords(PossibleAnswers, "RAISE", "_____");
//foreach (var outcome1 in Outcomes) {
//    var attempt2 = BestAttempt(possible1, AllWords);
//    var remain2 = RemainingValidWords(possible1, attempt2, outcome1);
//    Console.WriteLine($"{outcome1} ({remain2.Count})");

//}

//    //foreach (var outcome2 in Outcomes)
//    //{
//    //    var possible2 = RemainingValidWords(PossibleAnswers, attempt2, outcome2);
//    //    Console.Write($"{outcome1} ({possible.Count}), ");
//    //}
//    Console.WriteLine();
//    Console.ReadKey();


//
//while (true)
//{
//    
//    var attempt = "RAISE";
//    while (result != "*****")
//    {

//        attempt = BestAttempt(possible, AllWords);
//        int remain = RemainingWordCountLeftByWorstOutcome(possible, attempt);
//        Console.WriteLine($"{attempt} ({remain})");
//        result = Console.ReadLine();
//        possible = RemainingValidWords(possible, attempt, result);
//    }
//        Console.Write("Press any key for new game");
//}