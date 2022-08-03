static void RunTests()
{
    try
    {
        TestNeighbourCells(30, new List<int> { 9, 10, 11, 29, 31, 49, 50, 51 });
        AllTestsPassed();
    }
    catch (TestFailure) { }
}

private static void TestNeighbourCells(int cell, List<int> expected)
{
    var n = NeighbourCells(cell);
    string fn = nameof(NeighbourCells);
    var msg = fail + $"{fn}({cell}).";
    AssertTrue(fn, cell.ToString(), n.Count == 8, msg + $" Expected: 8 elements Actual: {n.Count}");
    foreach (int val in expected)
    {
        AssertTrue(fn, cell.ToString(), n.Contains(val), msg + $" List does not contain: {val}");
    }
}