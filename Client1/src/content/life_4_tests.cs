static void RunTests()
{
    testLiveNeighbourCount(0, 4);
    testLiveNeighbourCount(19, 4);
    testLiveNeighbourCount(30, 2);
    testLiveNeighbourCount(44, 3);
    testLiveNeighbourCount(59, 4);
    testLiveNeighbourCount(60, 3);
    testLiveNeighbourCount(399, 4);

    testAdjustedNeighbourCells(399, new List<int> { 378, 379, 380, 398, 0, 18, 19, 20 });
    testAdjustedNeighbourCells(380, new List<int> { 359, 360, 361, 379, 381, 399, 0, 1 });
    testAdjustedNeighbourCells(0, new List<int> { 379, 380, 381, 399, 1, 19, 20, 21 });
    testAdjustedNeighbourCells(19, new List<int> { 398, 399, 0, 18, 20, 38, 39, 40 });

    testKeepWithinBounds(0, 0);
    testKeepWithinBounds(399, 399);
    testKeepWithinBounds(400, 0);
    testKeepWithinBounds(401, 1);
    testKeepWithinBounds(419, 19);
    testKeepWithinBounds(-1, 399);
    testKeepWithinBounds(-20, 380);

    TestNeighbourCells(30, new List<int> { 9, 10, 11, 29, 31, 49, 50, 51 });
    AllTestsPassed();
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

private static void testKeepWithinBounds(int p1, int expected)
{
    TestFunction(nameof(KeepWithinBounds), expected, KeepWithinBounds(p1), p1);
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

private static void testLiveNeighbourCount(int p1, int expected)
{
    TestFunction(nameof(LiveNeighbours), expected, LiveNeighbours(testGrid, p1), "testGrid", p1);
}

private static List<bool> testGrid = new List<bool> { false, true, false, true, false, false, true, true, true, false, false, true, true, false, true, true, false, false, true, true, true, true, true, true, false, false, true, false, true, true, true, false, true, true, true, true, false, true, false, true, false, false, true, true, false, false, false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, false, false, true, false, false, true, false, false, true, true, false, false, false, true, true, false, false, true, true, false, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, true, false, false, true, true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, false, false, false, true, false, false, true, true, false, true, false, true, true, false, false, false, false, false, true, false, false, true, false, true, false, true, false, true, false, true, false, false, true, false, true, false, true, false, false, true, false, false, true, true, false, true, false, false, true, false, true, false, true, true, false, false, false, false, true, false, false, true, false, false, false, false, true, false, false, false, true, false, true, false, true, false, false, false, false, false, true, true, false, true, true, false, false, false, false, false, true, false, false, true, true, true, false, false, false, true, false, true, false, false, true, true, false, true, false, true, false, true, false, false, true, false, true, false, true, false, true, false, false, false, true, false, false, false, true, false, true, false, true, false, false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, true, false, true, false, false, true, false, true, false, true, false, false, false, false, false, true, false, true, false, false, false, false, true, true, false, false, true, true, false, true, true, false, true, true, false, true, false, false, true, false, true, false, false, true, false, true, true, false, false, false, false, false, true, true, false, false, false, true, true, true, false, false, false, true, true, false, true, true, true, false, false, false, true, false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, true, true, false, true, false, false, true, true, false, false, true, true, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false };
