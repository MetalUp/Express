public static void RunTests_WillLive()
{
    string fn = "WillLive";
    void test(bool currentlyAlive, int liveNeighbours, bool expected)
    {
        TestFunction(fn, expected, WillLive(currentlyAlive, liveNeighbours), currentlyAlive, liveNeighbours);
    }
    try
    {
        test(false, 0, false);
        test(false, 1, false);
        test(false, 2, false);
        test(false, 3, true);
        test(false, 4, false);
        test(false, 5, false);
        test(false, 6, false);
        test(false, 7, false);
        test(false, 8, false);
        test(true, 0, false);
        test(true, 1, false);
        test(true, 2, true);
        test(true, 3, true);
        test(true, 4, false);
        test(true, 5, false);
        test(true, 6, false);
        test(true, 7, false);
        test(true, 8, false);

    }
    catch (TestException) { } //Any other exception thrown at runtime will be uncaught and handled by server
    AllTestsPassed(fn);
}
