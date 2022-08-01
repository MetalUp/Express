using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Framework.Framework;

namespace ConsoleApp3
{
    static class Life
    {
        #region Data definitions and other hidden task-specific code
        const int size = 400; //Total number of cells in grid
        const int w = 20; //Number of cells horizontally

        static Random rand = new Random();
        static List<bool> testGrid = new List<bool> { false, true, false, true, false, false, true, true, true, false, false, true, true, false, true, true, false, false, true, true, true, true, true, true, false, false, true, false, true, true, true, false, true, true, true, true, false, true, false, true, false, false, true, true, false, false, false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, false, false, true, false, false, true, false, false, true, true, false, false, false, true, true, false, false, true, true, false, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, true, false, false, true, true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, false, false, false, true, false, false, true, true, false, true, false, true, true, false, false, false, false, false, true, false, false, true, false, true, false, true, false, true, false, true, false, false, true, false, true, false, true, false, false, true, false, false, true, true, false, true, false, false, true, false, true, false, true, true, false, false, false, false, true, false, false, true, false, false, false, false, true, false, false, false, true, false, true, false, true, false, false, false, false, false, true, true, false, true, true, false, false, false, false, false, true, false, false, true, true, true, false, false, false, true, false, true, false, false, true, true, false, true, false, true, false, true, false, false, true, false, true, false, true, false, true, false, false, false, true, false, false, false, true, false, true, false, true, false, false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, true, false, true, false, false, true, false, true, false, true, false, false, false, false, false, true, false, true, false, false, false, false, true, true, false, false, true, true, false, true, true, false, true, true, false, true, false, false, true, false, true, false, false, true, false, true, true, false, false, false, false, false, true, true, false, false, false, true, true, true, false, false, false, true, true, false, true, true, true, false, false, false, true, false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, true, true, false, true, false, false, true, true, false, false, true, true, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false };

        #endregion

        #region Student code

        // Defines the *relative* positions of each of the eight neighbours to any given cell
        static List<int> RelativeNeighbourPositions() => new List<int> { -w - 1, -w, w + 1, -1, +1, w - 1, w, w + 1 };

        // Effectively wraps the grid vertically so that top row and bottom row are neighbours
        static int KeepWithinGrid(int i) => i >= size ? i - size : i < 0 ? i + size : i;

        static int LiveNeighbourCount(List<bool> grid, int cellNo) => RelativeNeighbourPositions().Count(relPos => grid[KeepWithinGrid(cellNo + relPos)] is true);

        static bool WillLive(bool currentlyAlive, int liveNeighbours) => (currentlyAlive ? liveNeighbours > 1 && liveNeighbours < 4 : liveNeighbours == 3);

        //static bool WillLive(bool currentlyAlive, int liveNeighbours) => 
        //    (currentlyAlive && liveNeighbours > 1 && liveNeighbours < 4) | (!currentlyAlive && liveNeighbours == 3);


        static bool NextCellValue(List<bool> grid, int c) => WillLive(grid[c], LiveNeighbourCount(grid, c));

        static List<bool> NextGeneration(List<bool> grid) => Enumerable.Range(0, size).Select(n => NextCellValue(grid, n)).ToList();
        #endregion

        #region Testing

        public static void RunTests_KeepWithinGrid()
        {
            string fn = "KeepWithinGrid";
            void test(int p1, int expected)
            {
                TestFunction(fn, expected, KeepWithinGrid(p1), p1);
            }
            try
            {
                test(399, 399);
                test(400, 0);
                test(401, 1);
                test(419, 19);
                test(0, 0);
                test(-1, 399);
                test(-20, 380);
            }
            catch (TestException) {} //Any other exception thrown at runtime will be uncaught and handled by server
            AllTestsPassed(fn);
        }

        public static void RunTests_RelativeNeighbourPositions()
        {
            string fn = "RelativeNeighbourPositions";
            void test(bool result, string message) {
                AssertTrue(fn, result, message);
            }
            try
            {
                var n = RelativeNeighbourPositions();
                test(n.Count == 8, $"Returned list should contain 8 elements, currently has {n.Count}");
                foreach (int val in new List<int> { -w - 1, -w, w + 1, -1, +1, w - 1, w, w + 1 })
                {
                    test(n.Contains(val), $"Returned list is missing the value: {val}");
                }
            }
            catch (TestException) { }
            AllTestsPassed(fn);
        }

        public static void RunTests_LiveNeighbourCount()
        {
            string fn = "LiveNeighbourCount";
            void test(int p1, int expected)
            {
                TestFunction(fn, expected, LiveNeighbourCount(testGrid, p1), p1);
            }
            try
            {
                test(0, 5);
                test(19, 4);
                test(30, 1);
                test(44, 3);
                test(59, 4);
                test(60, 4);
                test(399, 5);
            }
            catch (TestException) { } //Any other exception thrown at runtime will be uncaught and handled by server
            AllTestsPassed(fn);
        }

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

        public static void RunTests_NextCellValue()
        {
            string fn = "NextCellValue";
            void test(int c, bool expected)
            {
                TestFunction(fn, expected, NextCellValue(testGrid, c), "testGrid", c);
            }
            try
            {
                test(0, false);
                test(19, false);
                test(30, false);
                test(44, true);
                test(59, false);
                test(60, false);
                test(399, false);
            }
            catch (TestException) { } //Any other exception thrown at runtime will be uncaught and handled by server
            AllTestsPassed(fn);
        }

        public static void RunTests_NextGeneration()
        {
            string fn = "NextGeneration";
            List<bool> nextGen = new List<bool> { false, false, false, true, true, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, true, false, false, true, true, false, false, true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, true, false, true, false, true, true, true, false, false, true, true, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, true, false, true, true, true, true, false, false, false, true, false, true, false, false, false, true, false, false, false, false, false, false, false, true, false, false, true, false, false, false, true, false, false, true, true, true, false, false, true, false, true, true, true, true, true, false, true, false, false, false, false, false, false, true, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, false, false, true, false, true, false, true, false, false, false, false, false, false, true, false, false, true, true, false, false, false, false, true, false, true, false, false, true, false, false, true, true, false, true, false, true, false, true, false, false, true, false, false, true, true, false, false, false, false, true, false, true, false, true, false, false, false, true, false, true, true, true, false, false, false, true, false, false, true, false, false, true, false, false, false, false, false, false, false, false, true, false, true, false, false, false, true, true, true, false, false, false, false, false, false, false, false, false, true, false, true, true, false, false, false, false, true, true, true, false, true, false, true, true, true, true, false, false, true, false, false, true, true, false, true, false, true, false, false, false, true, true, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, true, false, false, false, true, true, false, false, false, true, false, false, false, true, false, true, true, true, false, false, true, true, true, true, true, false, false, false, false, false, true, false, false, false, false, false, true, true, true, false, false, false, false, false, false, true, true, true, false, true, false, false, true, false, true, false, true, false, false, false, true, false, true, false, true, false, false, false, true, false, true, false };
            try
            {
                TestFunction(fn, nextGen, NextGeneration(testGrid), "testGrid");
            }
            catch (TestException) { } //Any other exception thrown at runtime will be uncaught and handled by server
            AllTestsPassed(fn);
        }
        #endregion

        #region App
        public static  void RunApp()
        {
            //Initialise grid with approx 33% randomly-selected live cells
            var grid = Enumerable.Range(0, size).Select(n => rand.Next(0, 3) > 1).ToList();

            while (true)
            {
                Console.Clear();
                for (int i = 0; i < size; i++)
                {
                    Console.Write(grid[i] ? "* " : "  ");
                    if (i % w == 0) Console.WriteLine(); //New line whenever a full row has been written
                }
                grid = NextGeneration(grid);
                Thread.Sleep(500); //Delay 100 milliseconds to slow the refresh rate
            }
        }
        #endregion
    }


}
