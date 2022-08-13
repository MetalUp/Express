using CSharp;
using static CSharp.Life;

var g = Glider();

while (true)
{
    Console.Clear();
    Console.Write(AsGrid(g));
    g = NextGeneration(g);
    Thread.Sleep(10);
}

Life.RunTests_NeighbourCells();
Life.RunTests_KeepWithinBounds();
Life.RunTests_AdjustedNeighbourCells();
Life.RunTests_LiveNeighbourCount();
Life.RunTests_WillLive();
Life.RunTests_NextCellValue();
Life.RunTests_NextGeneration();





//Console.WriteLine(AsGrid(Life.exampleCells));