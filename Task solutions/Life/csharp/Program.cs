using CSharp;
using static CSharp.Life;

//var g = Glider();
var g = Enumerable.Range(0, 400).Select(n => rand.Next(0, 3) > 1).ToList();

RunApp(g);

Life.RunTests_NeighbourCells();
Life.RunTests_KeepWithinBounds();
Life.RunTests_AdjustedNeighbourCells();
Life.RunTests_LiveNeighbourCount();
Life.RunTests_WillLive();
Life.RunTests_NextCellValue();
Life.RunTests_NextGeneration();





//Console.WriteLine(AsGrid(Life.exampleCells));