using System.Collections.Immutable;
using static CSharp.Reference;

static string Set(string word, int n, char newChar) => word.Substring(0, n) + newChar + word.Substring(n + 1);

static bool IsGreen(string attempt, string target, int n) => target[n] == attempt[n];

static string SetAttemptIfGreen(string attempt, string target, int n) =>
     IsGreen(attempt, target, n) ? Set(attempt, n, '*') : attempt;

static string SetTargetIfGreen(string attempt, string target, int n) =>
    IsGreen(attempt, target, n) ? Set(target, n, '.') : target;

static (string, string, int) SetAllGreens(string attempt, string target, int n) =>
    Enumerable.Range(0, 5).Aggregate((attempt, target, n), (a, x) =>
        (SetAttemptIfGreen(a.Item1, a.Item2, a.Item3), SetTargetIfGreen(a.Item1, a.Item2, a.Item3), x + 1));

static bool IsYellow(string attempt, string target, int n) => target.Contains(attempt[n]);

static string SetAttemptIfYellow(string attempt, string target, int n) =>
    IsYellow(attempt, target, n) ? Set(attempt, n, '+') : attempt;

static string SetTargetIfYellow(string attempt, string target, int n) =>
    IsYellow(attempt, target, n) ? Set(target, target.IndexOf(attempt[n]), '.') : target;

static (string, string, int) MarkAllYellows(string attempt, string target, int n) =>
    Enumerable.Range(0, 5).Aggregate((attempt, target, n), (a, x) =>
        (SetAttemptIfYellow(a.Item1, a.Item2, a.Item3), SetTargetIfYellow(a.Item1, a.Item2, a.Item3), x + 1));

static string MarkAttempt(string attempt, string target) =>
    MarkAllYellows(SetAllGreens(attempt, target, 0).Item1, SetAllGreens(attempt, target, 0).Item2, 0).Item1;

static ImmutableList<string> WordsRemainingAfter(ImmutableList<string> prior, string attempt, string mark) =>
    prior.Where(w => MarkAttempt(attempt, w) == mark).ToImmutableList();

static int WordCountLeftByWorstOutcome(ImmutableList<string> possibleWords, string attempt) =>
    possibleWords.GroupBy(w => MarkAttempt(attempt, w)).Max(g => g.Count());

static string FindBestAttempt(ImmutableList<string> possAnswers, ImmutableList<string> possAttempts) =>
    possAttempts.AsParallel().Select(w => (WordCountLeftByWorstOutcome(possAnswers, w), w)).
        Aggregate((best, x) => (x.Item1 < best.Item1) ||
        (x.Item1 == best.Item1 && possAnswers.Contains(x.Item2)) ? x : best).Item2;

//Constant data definitions:
//List<string> AllWords = new List<string> { "ABACK", "...", "ZYMIC" }; //See Appendix I
//List<string> AllPossibleAnswers = new List<string> { "ABACK", "...", "ZYMIC" }; //See Appendix I

//Application:
var possible = AllPossibleAnswers;
var outcome = "";
while (outcome != "*****")
{
    var attempt = FindBestAttempt(possible, AllWords);
    Console.WriteLine(attempt);
    outcome = Console.ReadLine();
    possible = WordsRemainingAfter(possible, attempt, outcome);
    Console.WriteLine($"{possible.Count} remaining");
}

