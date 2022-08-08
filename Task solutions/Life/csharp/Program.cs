using CSharp;

//Life.RunTests_NeighbourCells();
//Life.RunTests_KeepWithinBounds();
//Life.RunTests_AdjustedNeighbourCells();
//Life.RunTests_LiveNeighbourCount();
//Life.RunTests_WillLive();
//Life.RunTests_NextCellValue();
//Life.RunTests_NextGeneration();


var a = Enumerable.Range(1,2);
var b = Enumerable.Range(10,2);
var c = a.Sum(x => b.Sum(y => y*x));

Console.WriteLine(b.Sum(y => y * 1));
Console.WriteLine(b.Sum(y => y * 2));
Console.WriteLine(b.Sum(y => y * 1) + b.Sum(y => y * 2));
Console.WriteLine(c);