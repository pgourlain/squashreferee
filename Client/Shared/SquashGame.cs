
public class SquashGame
{
    public SquashPlayer Player1 = new SquashPlayer();
    public SquashPlayer Player2 = new SquashPlayer();
    public int PointsCount = 11;
    public int SetsCount = 5;

    private GameStatus _status = GameStatus.NotStarted;
    private SquashGameUI? _gameUICallBack = null;
    /**current storage slot name */
    private string currentSlot = string.Empty;
    public string Sex = "";

    public PlayerEnum Server = PlayerEnum.PlayerOne;
    public PlayerSideEnum ServiceSide = PlayerSideEnum.Left;
    public Stack<undoItem> actions = new();

    String sStart = "00:00:00";
    String sDuration = "00:00:00";
    internal NSTimer timer = new NSTimer();
    double? startTime;
    NSTimer pauseTimer = new NSTimer();
    double pauseTimeStart;
    StorageService? _storage;

    public SquashGame(StorageService? storage)
    {
        _storage = storage;
    }

    public GameStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public SquashGameUI? GameUICallBack
    {
        get => _gameUICallBack;
        set => _gameUICallBack = value;
    }

    public void Reset()
    {
        this.timer.invalidate();
        this.pauseTimer.invalidate();
        this._status = GameStatus.NotStarted;
        this.Player1.Score = 0;
        this.Player2.Score = 0;
        this.Player1.Scores = Array.Empty<int>();
        this.Player2.Scores = Array.Empty<int>();
        this.Player1.WinSets = 0;
        this.Player2.WinSets = 0;
        this.Player1.Image = null;
        this.Player2.Image = null;
        this.sStart = "00:00:00";
        this.actions = new ();
    }

    public void StartGame()
    {
        if (this.Status == GameStatus.NotStarted)
        {
            this.currentSlot = _storage?.newSlot() ?? string.Empty;
            this.actions.Push(new startUndoItem());
            this._status = GameStatus.TossChoice;
            if (this.GameUICallBack != null)
            {
                this.GameUICallBack.ChooseToss();
            }
            this.saveCurrentState();
        }
    }
    public void AddPoint(PlayerEnum which)
    {
        if (this.Status == GameStatus.Playing)
        {
            this.pauseTimer.invalidate();
            this.actions.Push(new pointUndoItem(this));
            switch (which)
            {
                case PlayerEnum.PlayerOne:
                    this.Player1.Score++;
                    if (!this.CheckWinner())
                    {
                        this.CheckServiceChange(which);
                    }
                    break;
                case PlayerEnum.PlayerTwo:
                    this.Player2.Score++;
                    if (!this.CheckWinner())
                    {
                        this.CheckServiceChange(which);
                    }
                    break;
            }
            this.saveCurrentState();
        }
    }
    private void TryServiceChoice(PlayerEnum which)
    {
        var preferedSide = this.GetPreferedSide(which);
        if (preferedSide != PlayerSideEnum.Uknown)
        {
            this._status = GameStatus.ServiceChoice;
            this.SetPlayerServeOn(preferedSide, true);
        }
        else
        {
            this._status = GameStatus.ServiceChoice;
            if (this.GameUICallBack != null)
            {
                this.GameUICallBack.ChooseServeSide();
            }
        }
    }
    private void CheckServiceChange(PlayerEnum which)
    {
        if (which != this.Server)
        {
            this.Server = which;
            this.TryServiceChoice(which);
        }
        else
        {
            if (this.ServiceSide == PlayerSideEnum.Left)
            {
                this.ServiceSide = PlayerSideEnum.Right;
            }
            else
            {
                this.ServiceSide = PlayerSideEnum.Left;
            }
            if (this.GameUICallBack != null)
            {
                this.GameUICallBack.UpdateServiceSide();
            }
        }
    }
    private PlayerSideEnum GetPreferedSide(PlayerEnum which)
    {
        bool hasPreference = false;
        PlayerSideEnum preference = PlayerSideEnum.Right;
        if (this.actions.Count > 0)
        {
            var acts = this.actions.ToArray();
            for (var index = 0; index < acts.Length; index++)
            {
                var element = acts[index];
                if (element is serviceUndoItem)
                {
                    if (((serviceUndoItem)element).server == which)
                    {
                        preference = ((serviceUndoItem)element).serviceSide;
                        hasPreference = true;
                    }
                }
            }
        }
        if (hasPreference)
        {
            return preference;
        }
        return PlayerSideEnum.Uknown;

    }
    void PauseStarted()
    {
        if (this.GameUICallBack != null)
        {
            this.GameUICallBack.PauseBegin();
        }
        this.pauseTimeStart = DateTime.Now.ToOADate();
        this.pauseTimer.scheduledTimerWithTimeInterval(90, () => { });
        this.TryServiceChoice(this.Server);
        this.saveCurrentState();
    }

    /// when user click to force the end of time between sets
    public void ForcePauseEnd()
    {
        this.PauseEnd();
    }

    private void PauseEnd()
    {
        this.pauseTimer.invalidate();
        if (this.Status != GameStatus.Playing)
        {
            if (this.GameUICallBack != null)
            {
                this.GameUICallBack.PauseEnd();
            }
        }
        this.saveCurrentState();
    }

    public double IntTimeLeft()
    {
        double currentTime = DateTime.Now.ToOADate();
        var dt = 90 * 1000 - (currentTime - this.pauseTimeStart);
        var seconds = Math.Floor(dt / 1000.0);
        return seconds;
    }
    public string TimeLeft()
    {
        return this.TwoDigitsString(this.TwoDigitsString(this.IntTimeLeft().ToString()));
    }

    private void StartTime()
    {
        this.startTime = NSTimeInterval.timeIntervalSinceReferenceDate();
        this.sStart = DateTime.Now.ToLocalTime().ToString();
        this.timer.scheduledTimerWithTimeInterval(1, this.updateTime);
    }

    private void updateTime()
    {
        if (this.GameUICallBack != null)
        {
            this.GameUICallBack.UpdateTime();
        }
    }

    public void Undo()
    {
        if (this.actions.Count > 0)
        {
            var last = this.actions.Pop();
            last.Play(this);
            this.saveCurrentState();
        }
    }

    private void MatchEnd(PlayerEnum player)
    {
        this.timer.invalidate();
        this.pauseTimer.invalidate();
        this.startTime = null;
        this._status = GameStatus.MatchEnd;
        if (this.GameUICallBack != null)
        {
            this.GameUICallBack.PlayerWins(player);
        }
        this.saveCurrentState();
    }

    private bool CheckWinner()
    {
        bool res = false;
        if (this.Player1.Score > this.Player2.Score)
        {
            if (this.Player1.Score >= this.PointsCount &&
                Math.Abs(this.Player2.Score - this.Player1.Score) >= 2)
            {
                res = true;
                this.Player2.PushScore(false);
                this.Player1.PushScore(true);
                if (this.GameUICallBack != null)
                {
                    this.GameUICallBack.PlayerWinSet(PlayerEnum.PlayerOne);
                }
                if (this.Player1.WinSets * 2 > this.SetsCount)
                {
                    this.MatchEnd(PlayerEnum.PlayerOne);
                }
                else
                {
                    this.Server = PlayerEnum.PlayerOne;
                    this.PauseStarted();
                }
            }
        }
        else
        {
            if (this.Player2.Score >= this.PointsCount &&
                Math.Abs(this.Player2.Score - this.Player1.Score) >= 2)
            {
                res = true;
                this.Player2.PushScore(true);
                this.Player1.PushScore(false);
                if (this.GameUICallBack != null)
                {
                    this.GameUICallBack.PlayerWinSet(PlayerEnum.PlayerTwo);
                }
                if (this.Player2.WinSets * 2 > this.SetsCount)
                {
                    this.MatchEnd(PlayerEnum.PlayerTwo);
                }
                else
                {
                    this.Server = PlayerEnum.PlayerTwo;
                    this.PauseStarted();
                }
            }
        }
        if (this.GameUICallBack != null)
        {
            this.GameUICallBack.UpdateScore(this.Player1.Score, this.Player2.Score);
        }
        return res;
    }

    public void SetPlayerWinToss(PlayerEnum which)
    {
        if (this.Status == GameStatus.TossChoice)
        {
            this.actions.Push(new tossUndoItem());
            this.Server = which;
            this.StartTime();
            this._status = GameStatus.ServiceChoice;
            if (this.GameUICallBack != null)
            {
                this.GameUICallBack.ChooseServeSide();
            }
            this.saveCurrentState();
        }
    }

    public void SetPlayerServeOn(PlayerSideEnum whichSide, bool isPrefered)
    {
        //this.pauseTimer.invalidate();
        if (this.Status == GameStatus.ServiceChoice)
        {
            this.actions.Push(new serviceUndoItem(this.Server, whichSide, isPrefered));
            this.ServiceSide = whichSide;
            this._status = GameStatus.Playing;
            if (this.GameUICallBack != null)
            {
                this.GameUICallBack.UpdateServiceSide();
            }
            this.saveCurrentState();
        }
    }

    private string TwoDigitsString(string s)
    {
        while (s.Length < 2)
        {
            s = "0" + s;
        }
        return s;
    }

    public String DurationAsString()
    {
        if (this.startTime is null)
        {
            return this.sDuration;
        }
        double currentTime = NSTimeInterval.timeIntervalSinceReferenceDate();
        var diff = currentTime - this.startTime;

        var dt = DateTime.FromOADate(diff ?? currentTime);
        var hours = dt.Hour;
        var minutes = dt.Minute;
        var seconds = dt.Second;

        this.sDuration = this.TwoDigitsString(hours.ToString()) + ":" + this.TwoDigitsString(minutes.ToString())
            + ":" + this.TwoDigitsString(seconds.ToString());
        return this.sDuration;

    }

    String StartTimeAsString()
    {

        return this.sStart;
    }

    string GlobalScore()
    {
        int nb = this.Player1.Scores.Length - 1;
        var ret = "";
        if (nb >= 0)
        {
            for (var index = 0; index <= nb; index++)
            {
                ret += (this.Player1.Scores[index] + "/" + this.Player2.Scores[index]);
                ret += "  ";
            }
        }
        return ret;
    }

    public string Winner()
    {
        if (this.Player1.WinSets > this.Player2.WinSets)
        {
            return this.Player1.Name;
        }
        return this.Player2.Name;
    }

    public String WinnerSets()
    {
        if (this.Player1.WinSets > this.Player2.WinSets)
        {
            return this.Player1.WinSets.ToString() + "/" + this.Player2.WinSets.ToString();
        }
        return this.Player2.WinSets.ToString() + "/" + this.Player1.WinSets.ToString();
    }

    public bool canSwicthService(PlayerEnum which)
    {
        if (this.actions.Count > 0)
        {
            var lastAction = this.actions.Peek();
            if (lastAction is serviceUndoItem)
            {
                if (((serviceUndoItem)lastAction).server == which)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void switchServiceSide()
    {
        if (this.actions.Count > 0)
        {
            var lastAction = this.actions.Peek();
            if (lastAction is serviceUndoItem)
            {
                var currentSide = ((serviceUndoItem)lastAction).serviceSide;
                if (currentSide == PlayerSideEnum.Left)
                {
                    ((serviceUndoItem)lastAction).serviceSide = PlayerSideEnum.Right;
                }
                else
                {
                    ((serviceUndoItem)lastAction).serviceSide = PlayerSideEnum.Left;
                }
                this.ServiceSide = ((serviceUndoItem)lastAction).serviceSide;
                this._status = GameStatus.Playing;
                if (this.GameUICallBack != null)
                {
                    this.GameUICallBack.UpdateServiceSide();
                }
            }
            this.saveCurrentState();
        }
    }

    private void loadFromSlot(string slot)
    {
        if (this._storage != null)
        {
            var sjson = this._storage.load(slot);
            this.currentSlot = slot;
            var newState = System.Text.Json.JsonSerializer.Deserialize<SquashGame>(sjson);
            //TODO: set all member with state from json
        }
    }

    //private string stringifyReplacer(string key, string value)
    //{
    //    switch (key)
    //    {
    //        case "timer":
    //        case "pauseTimer":
    //        case "_storage":
    //        case "Image":
    //            return null;
    //        default:
    //            return value;
    //    }
    //}

    private void saveCurrentState()
    {
        var that = this;
        if (this._storage != null)
        {
            try
            {
                var jsonAsString = System.Text.Json.JsonSerializer.Serialize(that);
                this._storage.save(this.currentSlot, jsonAsString);
            }
            catch (Exception)
            {
                //alert("save exception :" + e);
                //on iphone exception is thrown                
            }
        }
    }
}