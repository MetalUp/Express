// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

var sw = new Stopwatch();
sw.Start();
var result = Enumerable.Range(1,1000000).Where(x => PrimeNumbers.IsPrime(x)).ToList();
sw.Stop();

Console.WriteLine(sw.ElapsedMilliseconds/1000);

