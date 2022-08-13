static void RunTests()
{
        testKeepWithinBounds(0, 0);
        testKeepWithinBounds(399, 399);
        testKeepWithinBounds(400, 0);
        testKeepWithinBounds(401, 1);
        testKeepWithinBounds(419, 19);
        testKeepWithinBounds(-1, 399);
        testKeepWithinBounds(-20, 380);
        AllTestsPassed();
}

private static void testKeepWithinBounds(int p1, int expected)
{
    TestFunction(nameof(KeepWithinBounds), expected, KeepWithinBounds(p1), p1);
}