interface SquashGameUI {
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