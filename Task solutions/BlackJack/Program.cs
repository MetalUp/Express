// See https://aka.ms/new-console-template for more information
using Foo;
using static Foo.BlackJack;

FRandom rnd = FRandom.Seed(DateTime.Now.ToFileTime());

//for (int i = 0; i < 100; i++)
//{
//    Console.WriteLine(rnd.InRange0to1().ToString());
//    rnd = rnd.Next();
//}

//for (int i = 0; i < 1000; i++)
//{
//    List<char> hand = new List<char>();
//    (hand, rnd) = DrawOneDealerHand(hand, rnd);
//    foreach (char c in hand)
//    {
//        Console.Write($"{c}, ");
//    }
//    Console.WriteLine($" Outcome: {AdjustedValue(hand)}");


//}

var r = FRandom.Seed(1);

//Enumerable.Range(0, 10).Aggregate(("", 0), (a, n) => (a.Item1 + n.ToString(), a.Item2+n));

//Enumerable.Range(0, 10).Aggregate(("", FRandom.Seed()), (a, n) => (a.Item1 + n.ToString(), a.Item2.Next()));

var x = Enumerable.Range(0, 10).Aggregate((new List<int>(), FRandom.Seed()), (a, n) => (a.Item1.Append(a.Item2.InRange(0,10)).ToList(), a.Item2.Next()));




Console.WriteLine();