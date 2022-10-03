namespace CSharp
{
    public static class Wordle2
    {
        public static string Set(string word, int n, char newChar) => word.Substring(0, n) + newChar + word.Substring(n + 1);

        public static bool IsGreen(string attempt, string target, int n) => target[n] == attempt[n];

        public static string SetAttemptIfGreen(string attempt, string target, int n) =>
             IsGreen(attempt, target, n) ? Set(attempt, n, '*') : attempt;

        public static string SetTargetIfGreen(string attempt, string target, int n) =>
            IsGreen(attempt, target, n) ? Set(target, n, '.') : target;

        public static (string, string, int) SetAllGreens(string attempt, string target, int n) =>
            Enumerable.Range(0, 5).Aggregate((attempt,target,n), (a, x) => 
                (SetAttemptIfGreen(a.Item1, a.Item2, a.Item3), SetTargetIfGreen(a.Item1, a.Item2, a.Item3 ), x + 1));

        public static bool IsYellow(string attempt, string target, int n) => target.Contains(attempt[n]);

        public static string SetAttemptIfYellow(string attempt, string target, int n) =>
            IsYellow(attempt, target, n) ? Set(attempt, n, '+') : attempt;

        public static string SetTargetIfYellow(string attempt, string target, int n) =>
            IsYellow(attempt, target, n) ? Set(target, target.IndexOf(attempt[n]), '.') : target;

        public static (string, string, int) MarkAllYellows(string attempt, string target, int n) =>
            Enumerable.Range(0, 5).Aggregate((attempt, target, n), (a, x) => 
                (SetAttemptIfYellow(a.Item1, a.Item2, a.Item3), SetTargetIfYellow(a.Item1, a.Item2, a.Item3), x + 1));

        public static string MarkAttempt(string attempt, string target) =>
            MarkAllYellows(SetAllGreens(attempt, target, 0).Item1, SetAllGreens(attempt, target, 0).Item2, 0).Item1;

        public static List<string> WordsRemainingAfter(List<string> prior, string attempt, string mark) =>
            prior.Where(w => MarkAttempt(attempt, w) == mark).ToList();

        public static int WordCountLeftByWorstOutcome(List<string> possibleWords, string attempt) =>
            possibleWords.GroupBy(w => MarkAttempt(attempt, w)).Max(g => g.Count());

        public static string FindBestAttempt(List<string> possAnswers, List<string> possAttempts) =>
            possAttempts.AsParallel().Select(w => (WordCountLeftByWorstOutcome(possAnswers, w), w)).
                Aggregate((best, x) => (x.Item1 < best.Item1) ||
                (x.Item1 == best.Item1 && possAnswers.Contains(x.Item2)) ? x : best).Item2;
    }
}

