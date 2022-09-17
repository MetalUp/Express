using static Life;

//var g = Glider();
//var g = Enumerable.Range(0, 400).Select(n => rand.Next(0, 3) > 1).ToList();

//RunApp(g);

//RunTests();


testNewNeighbourCells(25, new List<int> { 4, 5, 6, 24, 26, 44, 45, 46 });
testNewNeighbourCells(10, new List<int> { 389, 390, 391, 9, 11, 29, 30, 31 });
testNewNeighbourCells(390, new List<int> { 369, 370, 371, 389, 391, 9, 10, 11 });
testNewNeighbourCells(60, new List<int> { 59, 40, 41, 79, 61, 99, 80, 81 });
testNewNeighbourCells(79, new List<int> { 58, 59, 40, 78, 60, 98, 99, 80 });

testNewNeighbourCells(0, new List<int> { 399, 380, 381, 19, 1, 19, 20, 21 });
testNewNeighbourCells(19, new List<int> { 398, 399, 0, 18, 20, 38, 39, 20 });
testNewNeighbourCells(399, new List<int> { 378, 379, 360, 398, 380, 18, 19, 0 });
testNewNeighbourCells(380, new List<int> { 379, 360, 361, 399, 381, 19, 0, 1 });





//Console.WriteLine(AsGrid(Life.exampleCells));