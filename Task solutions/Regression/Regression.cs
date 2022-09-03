using static csharp.Framework;

public static class Regression
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

    #endregion

    #region Tests
    private static List<(double, double)> empty = new List<(double, double)>();
    private static List<(double, double)> l1 = new List<(double, double)> 
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

        AllTestsPassed();
    }

    private static void TestTerm(Func<List<(double, double)>, double> f, List<(double, double)> data, double expected)
    {
        TestFunction(nameof(f), expected, f(data), data);
    }

    private static void TestCalcA()
    {

    }

    private static void TestCalcB()
    {

    }

    private static void TestBestFitFromSummaryTerms()
    {

    }

    private static void TestBestFitFromPoints()
    {

    }


    #endregion
}
