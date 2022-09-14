using static CSharp.Wordle;
using static CSharp.Reference;

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
        Console.WriteLine(attempt);
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


