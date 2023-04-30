using System.ComponentModel;

public interface ISquashGameUI
{
    void ChooseToss();
    void ChooseServeSide();
    void PlayerWins(PlayerEnum player);
    void PlayerWinSet(PlayerEnum player);
    void UpdateScore(int Score1, int Score2);
    void UpdateServiceSide();
    void UpdateTime();
    void PauseBegin();
    void PauseEnd();
    void HideChooseService();
    void UpdatePointDetails(int ptLeft, int ptRight);
}

class SquashGameUIWrapper : ISquashGameUI
{
    public event EventHandler OnChooseToss;
    public event EventHandler OnChooseServeSide;
    public event EventHandler<PlayerEnum> OnPlayerWinSet;
    public event EventHandler<PlayerEnum> OnPlayerWins;
    public event EventHandler OnUpdateServiceSide;
    public event EventHandler OnUpdateTime;
    public event EventHandler OnPauseBegin;
    public event EventHandler OnPauseEnd;
    public event EventHandler<Tuple<int, int>> OnUpdateScore;
    public event EventHandler<Tuple<int, int>> OnUpdatePointDetails;

    public SquashGameUIWrapper()
    {
    }

    public void ChooseServeSide()
    {
        OnChooseServeSide(this, EventArgs.Empty);
    }

    public void ChooseToss()
    {
        OnChooseToss(this, EventArgs.Empty);
    }

    public void HideChooseService()
    {
    }

    public void PauseBegin()
    {
        OnPauseBegin?.Invoke(this, EventArgs.Empty);
    }

    public void PauseEnd()
    {
        OnPauseEnd?.Invoke(this, EventArgs.Empty);
    }

    public void PlayerWins(PlayerEnum player)
    {
        OnPlayerWins(this, player);
    }

    public void PlayerWinSet(PlayerEnum player)
    {
        OnPlayerWinSet(this, player);
    }

    public void UpdateScore(int Score1, int Score2)
    {
        OnUpdateScore?.Invoke(this, new Tuple<int, int>(Score1, Score2));
    }

    public void UpdateServiceSide()
    {
        OnUpdateServiceSide?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateTime()
    {
        OnUpdateTime?.Invoke(this, EventArgs.Empty);
    }

    public void UpdatePointDetails(int ptLeft, int ptRight)
    {
        OnUpdatePointDetails?.Invoke(this, new Tuple<int, int>(ptLeft, ptRight));
    }
}