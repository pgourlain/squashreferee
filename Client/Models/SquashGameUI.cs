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
        throw new NotImplementedException();
    }

    public void PauseBegin()
    {
        throw new NotImplementedException();
    }

    public void PauseEnd()
    {
        throw new NotImplementedException();
    }

    public void PlayerWins(PlayerEnum player)
    {
        throw new NotImplementedException();
    }

    public void PlayerWinSet(PlayerEnum player)
    {
        throw new NotImplementedException();
    }

    public void UpdateScore(int Score1, int Score2)
    {
        throw new NotImplementedException();
    }

    public void UpdateServiceSide()
    {
        throw new NotImplementedException();
    }

    public void UpdateTime()
    {
        throw new NotImplementedException();
    }
}