using System.Text;
using static csharp.Framework;

public static class BestFit
{
    #region Data definitions and other hidden task-specific code
    #endregion

    #region Student code
    static double SumX(List<(double, double)> l) => l.Sum(p => p.Item1);
    static double SumY(List<(double, double)> l) => l.Sum(p => p.Item2);
    static double SumXsq(List<(double, double)> l) => l.Sum(p => p.Item1 * p.Item1);
    static double SumXY(List<(double, double)> l) => l.Sum(p => p.Item1 * p.Item2);

    static double CalcA(double sumX, double sumX2, double sumY, double sumXY, int n) => (sumY * sumX2 - sumX * sumXY) / (n * sumX2 - sumX * sumX);
    static double CalcB(double sumX, double sumX2, double sumY, double sumXY, int n) => (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);

    static (double, double) BestFitFromSummaryTerms(double sumX, double sumX2, double sumY, double sumXY, int n) =>
        (CalcA(sumX, sumX2, sumY, sumXY, n), CalcB(sumX, sumX2, sumY, sumXY, n));

    //Returns the terms (a,b) for best fit line of form y = a + bx for a list of (x,y) points
    static (double, double) BestFitFromPoints(List<(double, double)> l) =>
        BestFitFromSummaryTerms(SumX(l), SumXsq(l), SumY(l), SumXY(l), l.Count);
    #endregion

    #region App code

    //Assumes all x,y values are in range 0,<20 
    public static string Plot(List<(double, double)> l)
    {
        var grid = new bool[20,20];
        var s = new StringBuilder("20");
        foreach(var p in l)
        {
            grid[(int)p.Item1, (int)p.Item2] = true;
        }
        for (int y=19; y >= 0; y--)
        {
            s.Append($"|");
            for (int x = 0; x < 20; x++)
            {
                s.Append(grid[x, y] ? "■ " : "  ");
            }
            s.Append("\n");
        }
        s.Append("0---------------------------------------20");
        return s.ToString();
    }

    public static List<(double, double)> exampleData = new List<(double, double)>
    {(0,2.673040396),(1,3.550437254),(2,5.063963424),(3,6.445493589),(4,5.959864362),(5,7.826499306),(6,5.628729698),(7,10.65942485),(8,8.010532477),(9,10.67121885),(10,9.000669447),(11,13.50830072),(12,11.43464406),(13,13.58275437),(14,13.10821319),(15,10.28617997),(16,17.86384888),(17,15.20409345),(18,17.00764549),(19,12.11596949) };
    #endregion

    #region Tests
    private static List<(double, double)> empty = new List<(double, double)>();
    public static List<(double, double)> l1 = new List<(double, double)> 
    { (0.71, 1.12), (3.56, 5.36), (7.83, 9.04) };


    public static void RunTests()
    {
        TestTerm(SumX, empty, 0.0);
        TestTerm(SumX, l1, 12.1);

        TestTerm(SumY, empty, 0.0);
        TestTerm(SumY, l1, 15.52);

        TestTerm(SumXsq, empty, 0.0);
        TestTerm(SumXsq, l1, 74.4866);

        TestTerm(SumXY, empty, 0.0);
        TestTerm(SumXY, l1, 90.66);

        TestFunction(nameof(CalcA), 0.7663359541491346, CalcA(SumX(l1), SumXsq(l1), SumY(l1), SumXY(l1), l1.Count), l1);
        TestFunction(nameof(CalcB), 1.0926439783101325, CalcB(SumX(l1), SumXsq(l1), SumY(l1), SumXY(l1), l1.Count), l1);

        TestFunction(nameof(BestFitFromSummaryTerms), (0.7663359541491346, 1.0926439783101325), BestFitFromSummaryTerms(SumX(l1), SumXsq(l1), SumY(l1), SumXY(l1), l1.Count), l1);
        TestFunction(nameof(BestFitFromPoints), (0.7663359541491346, 1.0926439783101325), BestFitFromPoints(l1), l1);

        AllTestsPassed();
    }

    private static void TestTerm(Func<List<(double, double)>, double> f, List<(double, double)> data, double expected)
    {
        TestFunction(nameof(f), expected, f(data), data);
    }
    #endregion
}
