using System.ComponentModel;

public interface ISquashGameUI {
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
}

class SquashGameUIWrapper : ISquashGameUI
{
    public event EventHandler OnChooseToss;
    public event EventHandler OnChooseServeSide;
    public event EventHandler OnPlayerWinSet;
    public event EventHandler OnUpdateServiceSide;
    public SquashGameUIWrapper()
    {
    }

    public void ChooseServeSide()
    {
        if (OnChooseServeSide != null)
        {
            OnChooseServeSide(this, EventArgs.Empty);
        }
    }

    public void ChooseToss()
    {        
        if (OnChooseToss != null)
        {
            OnChooseToss(this, EventArgs.Empty);
        }
    }

    public void HideChooseService()
    {

    }

    public void PauseBegin()
    {
    }

    public void PauseEnd()
    {
    }

    public void PlayerWins(PlayerEnum player)
    {
    }

    public void PlayerWinSet(PlayerEnum player)
    {
    }

    public void UpdateScore(int Score1, int Score2)
    {
    }

    public void UpdateServiceSide()
    {
        if (OnUpdateServiceSide != null)
        {
            OnUpdateServiceSide(this, EventArgs.Empty);
        }
    }

    public void UpdateTime()
    {
    }
}