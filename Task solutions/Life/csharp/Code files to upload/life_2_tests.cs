static void RunTests()
{
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
