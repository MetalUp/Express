static void RunTests()
{
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
    AllTestsPassed();
}

private static void testWillLive(bool alive, int neighbours, bool expected)
{
    TestFunction(nameof(WillLive), expected, WillLive(alive, neighbours), alive, neighbours);
}
