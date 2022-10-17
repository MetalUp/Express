namespace Model.Types;

[ViewModel(typeof(RunResult_Functions))]

public class RunResult {
    public string Cmpinfo { get; init; }
    public int Outcome { get; init; }
    public string RunID { get; init; }
    public string Stderr { get; init; }
    public string Stdout { get; init; }
}