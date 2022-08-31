﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp
{
    public static class Wordle
    {
        public static (string, string, int) MarkGreens(string attempt, string target, int n) =>
         n == 5 ? (attempt, target, n) : //First, test for the 'exit condition'
             target[n] == attempt[n] ? MarkGreens(Set(attempt, n, '*'), Set(target, n, '_'), n + 1) :
                     MarkGreens(attempt, target, n + 1);

        public static (string, string) MarkGreens2(string attempt, string target) =>
            Enumerable.Range(0, 5).Aggregate((attempt, target),
                (at, n) => at.Item1[n] == at.Item2[n] ? (Set(at.Item1, n, '*'), Set(at.Item2, n, '_')): at);

        public static string Set(string word, int n, char newChar) => word.Substring(0,n) + newChar + word.Substring(n + 1);

        public static (string, string, int) MarkYellows(string attempt, string target, int n) =>
            n == 5 ? (attempt, target, n) : //First, test for the 'exit condition'
                    target.Contains(attempt[n]) ? MarkYellows(Set(attempt, n, '+'), Set(target, target.IndexOf(attempt[n]), '_'), n + 1) :
                        MarkYellows( Set(attempt, n, '_'), target, n + 1);

        public static (string, string) MarkYellows2(string attempt, string target) =>
            Enumerable.Range(0, 5).Aggregate((attempt, target),
              (at, n) => at.Item2.Contains(at.Item1[n]) ? (Set(at.Item1, n, '+'), Set(at.Item2, at.Item2.IndexOf(at.Item1[n]), '_')) :
                 (Set(at.Item1, n, '_'), at.Item2));

        public static string MarkAttempt(string attempt, string target) =>
            MarkYellows(MarkGreens(attempt, target, 0).Item1, MarkGreens(attempt,target, 0).Item2, 0).Item1;

        public static List<string> RemainingValidWords(List<string> priorPossible, string attempt, string mark) =>
            priorPossible.Where(w => MarkAttempt(attempt, w) == mark).ToList();

        public static int RemainingWordCountLeftByWorstOutcome(List<string> possibleWords, string attempt) =>
            possibleWords.GroupBy(w => MarkAttempt(attempt, w)).Max(g => g.Count());

        public static string BestAttempt(List<string> possibleWords, List<string> allWords) =>
            allWords.Select(w => new { word = w, count = RemainingWordCountLeftByWorstOutcome(possibleWords, w) }).
                Aggregate((best, x) => (x.count < best.count) ||(x.count == best.count && possibleWords.Contains(x.word)) ? x : best).word;
    }
}