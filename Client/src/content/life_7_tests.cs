static void RunTests()
{
    string fn = nameof(NextGeneration);
    List<bool> nextGen = new List<bool> { false, true, false, true, true, true, true, false, true, false, true, true, true, false, false, false, true, true, false, false, false, false, false, false, true, false, true, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, true, false, false, true, false, false, false, false, false, false, false, false, true, false, false, false, false, true, false, true, true, true, false, false, true, true, true, true, true, false, false, false, false, false, false, false, true, true, false, false, false, true, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, false, false, false, true, true, false, true, true, false, false, true, true, true, true, false, false, false, true, true, true, true, false, false, true, false, true, false, true, false, true, false, false, false, false, false, false, false, true, true, true, true, true, false, true, false, true, false, true, true, false, true, false, true, false, true, true, false, false, false, true, false, true, false, false, false, true, false, false, false, false, true, false, true, false, false, true, false, false, false, true, false, true, false, true, true, false, true, false, false, true, true, false, true, false, false, false, false, false, false, true, false, true, true, false, false, true, false, false, true, true, false, true, true, false, false, false, true, false, false, true, true, false, false, false, true, false, false, false, false, false, true, false, true, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, true, false, false, true, false, true, false, true, true, false, true, false, true, true, false, false, false, false, true, false, true, true, true, false, false, true, true, false, false, false, true, false, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, true, true, false, false, false, true, false, false, false, true, false, false, false, false, false, false, true, false, true, false, false, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, true, true, false, false, false, true, false, true, false, true, false, false, false, true, true, true, false };
    TestFunction(fn, nextGen, NextGeneration(testGrid), " testGrid");

    testNextCellValue(0, false);
    testNextCellValue(19, false);
    testNextCellValue(30, true);
    testNextCellValue(44, true);
    testNextCellValue(59, false);
    testNextCellValue(60, true);
    testNextCellValue(399, false);

    testWillLive(false, 0, false);
    testWillLive(false, 1, false);
    testWillLive(false, 2, false);
    testWillLive(false, 3, true);
    testWillLive(false, 4, false);
    testWillLive(false, 5, false);
    testWillLive(false, 6, false);
    testWillLive(false, 7, false);
    testWillLive(false, 8, false);
    testWillLive(true, 0, false);
    testWillLive(true, 1, false);
    testWillLive(true, 2, true);
    testWillLive(true, 3, true);
    testWillLive(true, 4, false);
    testWillLive(true, 5, false);
    testWillLive(true, 6, false);
    testWillLive(true, 7, false);
    testWillLive(true, 8, false);

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

private static void testWillLive(bool alive, int neighbours, bool expected)
{
    TestFunction(nameof(WillLive), expected, WillLive(alive, neighbours), alive, neighbours);
}

private static void testNextCellValue(int c, bool expected)
{
    TestFunction(nameof(NextCellValue), expected, NextCellValue(exampleCells, c), "exampleCells", c);
}