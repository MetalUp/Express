using System.Collections.Immutable;

namespace CSharp
{
    public static class Wordle
    {

        public static (string, string, int) MarkGreens(string attempt, string target, int n) =>
         n == 5 ? (attempt, target, n) : //First, test for the 'exit condition'
             target[n] == attempt[n] ? MarkGreens(Set(attempt, n, '*'), Set(target, n, '_'), n + 1) :
                     MarkGreens(attempt, target, n + 1);

        public static string Set(string word, int n, char newChar) => word.Substring(0, n) + newChar + word.Substring(n + 1);

        public static (string, string, int) MarkYellows(string attempt, string target, int n) =>
            n == 5 ? (attempt, target, n) : //First, test for the 'exit condition'
                attempt[n] == '*' ? MarkYellows(attempt, target, n + 1) :
                    target.Contains(attempt[n]) ? MarkYellows(Set(attempt, n, '+'), Set(target, target.IndexOf(attempt[n]), '_'), n + 1) :
                        MarkYellows(Set(attempt, n, '_'), target, n + 1);

        public static string MarkAttempt(string attempt, string target) =>
            MarkYellows(MarkGreens(attempt, target, 0).Item1, MarkGreens(attempt, target, 0).Item2, 0).Item1;

        public static ImmutableList<string> RemainingValidWords(ImmutableList<string> priorPossible, string attempt, string mark) =>
            priorPossible.Where(w => MarkAttempt(attempt, w) == mark).ToImmutableList();

        public static int RemainingWordCountLeftByWorstOutcome(ImmutableList<string> possibleWords, string attempt) =>
            possibleWords.GroupBy(w => MarkAttempt(attempt, w)).Max(g => g.Count());

        public static string BestAttempt(ImmutableList<string> possibleWords, ImmutableList<string> allWords) =>
            allWords.AsParallel().Select(w => new { word = w, count = RemainingWordCountLeftByWorstOutcome(possibleWords, w) }).
                Aggregate((best, x) => (x.count < best.count) || (x.count == best.count && possibleWords.Contains(x.word)) ? x : best).word;
    }
}

