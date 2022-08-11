static void RunTests()
{
    try
    {
        string fn = nameof(AdjustedNeighbourCells);
        testAdjustedNeighbourCells(399, new List<int> { 378, 379, 380, 398, 0, 18, 19, 20 });
        testAdjustedNeighbourCells(380, new List<int> { 359, 360, 361, 379, 381, 399, 0, 1 });
        testAdjustedNeighbourCells(0, new List<int> { 379, 380, 381, 399, 1, 19, 20, 21 });
        testAdjustedNeighbourCells(19, new List<int> { 398, 399, 0, 18, 20, 38, 39, 40 });
        AllTestsPassed();
    }
    catch (TestFailure) { }
}

private static void testAdjustedNeighbourCells(int cell, List<int> expected)
{
    var n = AdjustedNeighbourCells(cell);
    string fn = nameof(AdjustedNeighbourCells);
    AssertTrue(fn, cell.ToString(), n.Count == 8, $" Expected: 8 elements Actual: {n.Count}");
    foreach (int val in expected)
    {
        AssertTrue(fn, cell.ToString(), n.Contains(val), $"List does not contain: {val}");
    }
}