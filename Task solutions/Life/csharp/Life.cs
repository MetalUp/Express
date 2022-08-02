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
        static List<bool> testGrid = new List<bool> { false, true, false, true, false, false, true, true, true, false, false, true, true, false, true, true, false, false, true, true, true, true, true, true, false, false, true, false, true, true, true, false, true, true, true, true, false, true, false, true, false, false, true, true, false, false, false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, false, false, true, false, false, true, false, false, true, true, false, false, false, true, true, false, false, true, true, false, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, true, false, false, true, true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, false, false, false, true, false, false, true, true, false, true, false, true, true, false, false, false, false, false, true, false, false, true, false, true, false, true, false, true, false, true, false, false, true, false, true, false, true, false, false, true, false, false, true, true, false, true, false, false, true, false, true, false, true, true, false, false, false, false, true, false, false, true, false, false, false, false, true, false, false, false, true, false, true, false, true, false, false, false, false, false, true, true, false, true, true, false, false, false, false, false, true, false, false, true, true, true, false, false, false, true, false, true, false, false, true, true, false, true, false, true, false, true, false, false, true, false, true, false, true, false, true, false, false, false, true, false, false, false, true, false, true, false, true, false, false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, true, false, true, false, false, true, false, true, false, true, false, false, false, false, false, true, false, true, false, false, false, false, true, true, false, false, true, true, false, true, true, false, true, true, false, true, false, false, true, false, true, false, false, true, false, true, true, false, false, false, false, false, true, true, false, false, false, true, true, true, false, false, false, true, true, false, true, true, true, false, false, false, true, false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, true, true, false, true, false, false, true, true, false, false, true, true, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false };



        #endregion

        #region Student code

        static List<int> NeighbourCells(int c) => new List<int> { c-21, c-20, c-19, c-1, c+1, c+19, c+20, c+21 };
        static List<int> AdjustedNeighbourCells(int c) =>NeighbourCells(c).Select(x => KeepWithinBounds(x)).ToList();
        
        // Effectively wraps the grid vertically so that top row and bottom row are neighbours
        static int KeepWithinBounds(int i) => i >= 400 ? i - 400 : i < 0 ? i + 400 : i;

        static int LiveNeighbours(List<bool> grid, int c) =>AdjustedNeighbourCells(c).Count(i => grid[i] is true);

        static bool WillLive(bool currentlyAlive, int liveNeighbours) => (currentlyAlive ? liveNeighbours > 1 && liveNeighbours < 4 : liveNeighbours == 3);

        //static bool WillLive(bool currentlyAlive, int liveNeighbours) => 
        //    (currentlyAlive && liveNeighbours > 1 && liveNeighbours < 4) | (!currentlyAlive && liveNeighbours == 3);


        static bool NextCellValue(List<bool> grid, int c) => WillLive(grid[c], LiveNeighbours(grid, c));

        static List<bool> NextGeneration(List<bool> grid) => Enumerable.Range(0, 400).Select(n => NextCellValue(grid, n)).ToList();
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
            string fn = nameof(NeighbourCells);
            void test(int cell, List<int> expected)
            {
                var n = NeighbourCells(cell);
                var msg = fail +$"{fn}({cell}).";
                AssertTrue(fn, cell.ToString(), n.Count == 8,   msg + $" Expected: 8 elements Actual: {n.Count}");
                foreach (int val in expected)
                {
                    AssertTrue(fn, cell.ToString(), n.Contains(val), msg + $" List does not contain: {val}");
                }
            }
            test(30, new List<int> { 9, 10, 11, 29, 31, 49, 50, 51 });
            AllTestsPassed(fn);
        }

        public static void RunTests_KeepWithinBounds()
        {
            string fn = nameof(KeepWithinBounds);
            void test(int p1, int expected)
            {
                TestFunction(fn, expected, KeepWithinBounds(p1), p1);
            }
            test(399, 399);
            test(400, 0);
            test(401, 1);
            test(419, 19);
            test(0, 0);
            test(-1, 399);
            test(-20, 380);
            AllTestsPassed(fn);
        }

        public static void RunTests_AdjustedNeighbourCells()
        {
            string fn = nameof(AdjustedNeighbourCells);
            void test(int cell, List<int> expected)
            {
                var n = AdjustedNeighbourCells(cell);
                AssertTrue(fn, cell.ToString(), n.Count == 8, $" Expected: 8 elements Actual: {n.Count}");
                foreach (int val in expected)
                {
                    AssertTrue(fn, cell.ToString(), n.Contains(val),$"List does not contain: {val}");
                }
            }
            test(399, new List<int> { 378, 379, 380, 398, 0, 18, 19, 20 });
            test(380, new List<int> { 359, 360, 361, 379, 381, 399, 0,1 });
            test(0, new List<int> { 379, 380, 381, 399, 1, 19, 20, 21 });
            test(19, new List<int> { 398, 399, 0, 18, 20, 38, 39, 40 });
            AllTestsPassed(fn);
        }

        public static void RunTests_LiveNeighbourCount()
        {
            string fn = nameof(LiveNeighbours);
            void test(int p1, int expected)
            {
                TestFunction(fn, expected, LiveNeighbours(testGrid, p1), "testGrid", p1);
            }
            test(0, 4);
            test(19, 4);
            test(30, 2);
            test(44, 3);
            test(59, 4);
            test(60, 3);
            test(399, 4);
            AllTestsPassed(fn);
        }

        public static void RunTests_WillLive()
        {
            string fn = nameof(WillLive);
            void test(bool alive, int neighbours, bool expected)
            {
                TestFunction(fn, expected, WillLive(alive, neighbours), alive, neighbours);
            }
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
            AllTestsPassed(fn);
        }

        public static void RunTests_NextCellValue()
        {
            string fn = nameof(NextCellValue);
            void test(int c, bool expected)
            {
                TestFunction(fn, expected, NextCellValue(testGrid, c), "testGrid", c);
            }
            test(0, false);
            test(19, false);
            test(30, true);
            test(44, true);
            test(59, false);
            test(60, true);
            test(399, false);
            AllTestsPassed(fn);
        }

        public static void RunTests_NextGeneration()
        {
            string fn = nameof(NextGeneration);
            List<bool> nextGen = new List<bool> { false, true, false, true, true, true, true, false, true, false, true, true, true, false, false, false, true, true, false, false, false, false, false, false, true, false, true, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, true, false, false, true, false, false, false, false, false, false, false, false, true, false, false, false, false, true, false, true, true, true, false, false, true, true, true, true, true, false, false, false, false, false, false, false, true, true, false, false, false, true, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, false, false, false, true, true, false, true, true, false, false, true, true, true, true, false, false, false, true, true, true, true, false, false, true, false, true, false, true, false, true, false, false, false, false, false, false, false, true, true, true, true, true, false, true, false, true, false, true, true, false, true, false, true, false, true, true, false, false, false, true, false, true, false, false, false, true, false, false, false, false, true, false, true, false, false, true, false, false, false, true, false, true, false, true, true, false, true, false, false, true, true, false, true, false, false, false, false, false, false, true, false, true, true, false, false, true, false, false, true, true, false, true, true, false, false, false, true, false, false, true, true, false, false, false, true, false, false, false, false, false, true, false, true, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, true, false, false, true, false, true, false, true, true, false, true, false, true, true, false, false, false, false, true, false, true, true, true, false, false, true, true, false, false, false, true, false, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, true, true, false, false, false, true, false, false, false, true, false, false, false, false, false, false, true, false, true, false, false, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, true, true, false, false, false, true, false, true, false, true, false, false, false, true, true, true, false };
            TestFunction(fn, nextGen, NextGeneration(testGrid), "testGrid");
            AllTestsPassed(fn);
        }
        #endregion
    }


}
