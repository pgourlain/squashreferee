

class undoItem {
    public void Play(SquashGame game) {
    }
}

class pointUndoItem : undoItem {
    int _scoreP1;
    int[] _setP1;
    int _scoreP2;
    int[] _setP2;
    internal PlayerSideEnum serviceSide;
    internal PlayerEnum server;
    int _winSetsP1;
    int _winSetsP2;

    public pointUndoItem(SquashGame game) {
        this._scoreP1 = game.Player1.Score;
        this._scoreP2 = game.Player2.Score;
        this._setP1 = game.Player1.Scores;
        this._setP2 = game.Player2.Scores;
        this._winSetsP1 = game.Player1.WinSets;
        this._winSetsP2 = game.Player2.WinSets;
        this.serviceSide = game.ServiceSide;
        this.server = game.Server;
    }

    public void Play(SquashGame game) {
        game.Player1.Score = this._scoreP1;
        game.Player1.Scores = this._setP1;
        game.Player2.Score = this._scoreP2;
        game.Player2.Scores = this._setP2;
        game.Player1.WinSets = this._winSetsP1;
        game.Player2.WinSets = this._winSetsP2;
        game.Server = this.server;
        game.ServiceSide = this.serviceSide;
        game.Status = GameStatus.Playing;
        if (game.GameUICallBack != null) {
            game.GameUICallBack.UpdateScore(this._scoreP1, this._scoreP2);
            game.GameUICallBack.UpdateServiceSide();
            game.GameUICallBack.HideChooseService();
        }
    }
}

class startUndoItem : undoItem {

    public void Play(SquashGame game ) {
        game.Reset();
        game.StartGame();
    }
}

class tossUndoItem : undoItem {
    public void Play(SquashGame game) {
        game.timer.invalidate();
        game.Status = GameStatus.TossChoice;
        if (game.GameUICallBack != null) {
            game.GameUICallBack.ChooseToss();
        }
    }
}

class serviceUndoItem : undoItem {
    internal PlayerSideEnum serviceSide;
    internal PlayerEnum server;
    bool isPrefered;
    public serviceUndoItem(PlayerEnum srv, PlayerSideEnum service,bool isPrefered) {
        this.serviceSide = service;
        this.server = srv;
        this.isPrefered = isPrefered;
    }

    public void Play(SquashGame game) {
        game.Status = GameStatus.ServiceChoice;
        if (this.isPrefered) {
            game.Undo();
            //game.SetPlayerServeOn(this.serviceSide, true);
        } else {
            if (game.GameUICallBack != null) {
                game.GameUICallBack.ChooseServeSide();
            }
        }
    }
}

