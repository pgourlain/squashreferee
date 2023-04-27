

public class SquashPlayer {
    public string Name = "?";
    public object? Image = null;
    public int[] Scores = Array.Empty<int>();
    public int Score=0;
    public int WinSets=0;

    public void PushScore(bool isWin) {
        var l = new List<int>(this.Scores);
        l.Add(this.Score);
        this.Scores = l.ToArray();
        this.Score = 0;
        if (isWin) {
            this.WinSets++;
        }
    }

    public int GetWinSets() {
        return this.WinSets;
    }
} 