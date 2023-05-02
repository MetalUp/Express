namespace Model.Types;

[ViewModel(typeof(RunResult_Functions))]
public class RunResult {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public RunResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string Cmpinfo { get; init; }
    public int Outcome { get; init; }
    public string RunID { get; init; }
    public string Stderr { get; init; }
    public string Stdout { get; init; }
    public int LineNo { get; set; }
    public int ColNo { get; set; }
}