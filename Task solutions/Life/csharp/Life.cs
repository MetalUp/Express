using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Framework.Framework;


namespace CSharp
{
    static class Life
    {
        #region Data definitions and other hidden task-specific code
        static Random rand = new Random();
        public static List<bool> exampleCells = new List<bool> { false, true, false, true, false, false, true, true, true, false, false, true, true, false, true, true, false, false, true, true, true, true, true, true, false, false, true, false, true, true, true, false, true, true, true, true, false, true, false, true, false, false, true, true, false, false, false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, false, false, true, false, false, true, false, false, true, true, false, false, false, true, true, false, false, true, true, false, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, true, false, false, true, true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, false, false, false, true, false, false, true, true, false, true, false, true, true, false, false, false, false, false, true, false, false, true, false, true, false, true, false, true, false, true, false, false, true, false, true, false, true, false, false, true, false, false, true, true, false, true, false, false, true, false, true, false, true, true, false, false, false, false, true, false, false, true, false, false, false, false, true, false, false, false, true, false, true, false, true, false, false, false, false, false, true, true, false, true, true, false, false, false, false, false, true, false, false, true, true, true, false, false, false, true, false, true, false, false, true, true, false, true, false, true, false, true, false, false, true, false, true, false, true, false, true, false, false, false, true, false, false, false, true, false, true, false, true, false, false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, true, false, true, false, false, true, false, true, false, true, false, false, false, false, false, true, false, true, false, false, false, false, true, true, false, false, true, true, false, true, true, false, true, true, false, true, false, false, true, false, true, false, false, true, false, true, true, false, false, false, false, false, true, true, false, false, false, true, true, true, false, false, false, true, true, false, true, true, true, false, false, false, true, false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, true, true, false, true, false, false, true, true, false, false, true, true, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false };

        public static string AsGrid(List<bool> cells) => Enumerable.Range(0, 400).Aggregate("Grid:\n", (s, n) => s + (n > 0 && n % 20 == 0 ? "\n" : "")+(cells[n] ? "■ " : "  "));
        #endregion

        #region Student code

        static List<int> NeighbourCells(int c) => new List<int> { c - 21, c - 20, c - 19, c - 1, c + 1, c + 19, c + 20, c + 21 };

        // Effectively wraps the grid vertically so that top row and bottom row are neighbours
        //static int KeepWithinBounds(int i) => i >= 400 ? i - 400 : i < 0 ? i + 400 : i;
        static int KeepWithinBounds(int i) => i % 400;


        static List<int> AdjustedNeighbourCells(int c) => NeighbourCells(c).Select(x => KeepWithinBounds(x)).ToList();

        //static int LiveNeighbours(List<bool> cells, int c) => AdjustedNeighbourCells(c).Count(i => cells[i] == true);
        static int LiveNeighbours(List<bool> cells, int c) => AdjustedNeighbourCells(c).Where(i => cells[i] == true).Count();

        static bool WillLive(bool currentlyAlive, int liveNeighbours) => (currentlyAlive ? liveNeighbours > 1 && liveNeighbours < 4 : liveNeighbours == 3);

        //static bool WillLive(bool currentlyAlive, int liveNeighbours) => 
        //    (currentlyAlive && liveNeighbours > 1 && liveNeighbours < 4) | (!currentlyAlive && liveNeighbours == 3);


        static bool NextCellValue(List<bool> cells, int c) => WillLive(cells[c], LiveNeighbours(cells, c));

        static List<bool> NextGeneration(List<bool> cells) => Enumerable.Range(0, 400).Select(n => NextCellValue(cells, n)).ToList();
        #endregion

        #region App
        public static void RunApp()
        {
            //Initialise grid with approx 33% randomly-selected live cells
            var grid = Enumerable.Range(0, 400).Select(n => rand.Next(0, 3) > 1).ToList();

            while (true)
            {
                Console.Clear();
                for (int i = 0; i < 400; i++)
                {
                    Console.Write(grid[i] ? "* " : "  ");
                    if (i % 20 == 0) Console.WriteLine(); //New line whenever a full row has been written
                }
                grid = NextGeneration(grid);
                Thread.Sleep(500); //Delay 100 milliseconds to slow the refresh rate
            }
        }
        #endregion

        #region Tests


        public static void RunTests_NeighbourCells()
        {
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

        public static void RunTests_KeepWithinBounds()
        {
            try
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
            catch (TestFailure) { }
        }

        private static void testKeepWithinBounds(int p1, int expected)
        {
            TestFunction(nameof(KeepWithinBounds), expected, KeepWithinBounds(p1), p1);
        }

        public static void RunTests_AdjustedNeighbourCells()
        {
                string fn = nameof(AdjustedNeighbourCells);
                testAdjustedNeighbourCells(399, new List<int> { 378, 379, 380, 398, 0, 18, 19, 20 });
                testAdjustedNeighbourCells(380, new List<int> { 359, 360, 361, 379, 381, 399, 0, 1 });
                testAdjustedNeighbourCells(0, new List<int> { 379, 380, 381, 399, 1, 19, 20, 21 });
                testAdjustedNeighbourCells(19, new List<int> { 398, 399, 0, 18, 20, 38, 39, 40 });
                AllTestsPassed();
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

        public static void RunTests_LiveNeighbourCount()
        {
            testLiveNeighbourCount(0, 4);
            testLiveNeighbourCount(19, 4);
            testLiveNeighbourCount(30, 2);
            testLiveNeighbourCount(44, 3);
            testLiveNeighbourCount(59, 4);
            testLiveNeighbourCount(60, 3);
            testLiveNeighbourCount(399, 4);
            AllTestsPassed();
        }

        private static void testLiveNeighbourCount(int p1, int expected)
        {
            TestFunction(nameof(LiveNeighbours), expected, LiveNeighbours(exampleCells, p1), "testGrid", p1);
        }

public static void RunTests_WillLive()
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

        public static void RunTests_NextCellValue()
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
            TestFunction(nameof(NextCellValue), expected, NextCellValue(exampleCells, c), "testGrid", c);
        }

        public static void RunTests_NextGeneration()
        {
            string fn = nameof(NextGeneration);
            List<bool> nextGen = new List<bool> { false, true, false, true, true, true, true, false, true, false, true, true, true, false, false, false, true, true, false, false, false, false, false, false, true, false, true, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, true, false, false, true, false, false, false, false, false, false, false, false, true, false, false, false, false, true, false, true, true, true, false, false, true, true, true, true, true, false, false, false, false, false, false, false, true, true, false, false, false, true, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, false, false, false, true, true, false, true, true, false, false, true, true, true, true, false, false, false, true, true, true, true, false, false, true, false, true, false, true, false, true, false, false, false, false, false, false, false, true, true, true, true, true, false, true, false, true, false, true, true, false, true, false, true, false, true, true, false, false, false, true, false, true, false, false, false, true, false, false, false, false, true, false, true, false, false, true, false, false, false, true, false, true, false, true, true, false, true, false, false, true, true, false, true, false, false, false, false, false, false, true, false, true, true, false, false, true, false, false, true, true, false, true, true, false, false, false, true, false, false, true, true, false, false, false, true, false, false, false, false, false, true, false, true, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, true, false, false, true, false, true, false, true, true, false, true, false, true, true, false, false, false, false, true, false, true, true, true, false, false, true, true, false, false, false, true, false, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, true, true, false, false, false, true, false, false, false, true, false, false, false, false, false, false, true, false, true, false, false, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, true, true, false, false, false, true, false, true, false, true, false, false, false, true, true, true, false };
            TestFunction(fn, nextGen, NextGeneration(exampleCells), "testGrid");
            AllTestsPassed();
        }
        #endregion
    }


}
