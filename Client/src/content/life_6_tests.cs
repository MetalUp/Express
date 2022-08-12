static void RunTests()
{
    testNextCellValue(0, false);
    testNextCellValue(19, false);
    testNextCellValue(30, true);
    testNextCellValue(44, true);
    testNextCellValue(59, false);
    testNextCellValue(60, true);
    testNextCellValue(399, false);
    AllTestsPassed();
}

private static void testNextCellValue(int c, bool expected)
{
    TestFunction(nameof(NextCellValue), expected, NextCellValue(exampleCells, c), "exampleCells", c);
}

