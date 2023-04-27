using System.Timers;

class NSTimer {
    System.Timers.Timer? intervalID = null;
    public void invalidate() {
        if (this.intervalID != null) {
            clearInterval(this.intervalID);
        }
        this.intervalID = null;
    }

    public void scheduledTimerWithTimeInterval(int interval, Action callback) {
        if (this.intervalID != null) {
            clearInterval(this.intervalID);
        }
        this.intervalID = setInterval(callback, interval * 1000);
    }

    System.Timers.Timer setInterval(Action callback, int msTimeOut)
    {
        System.Timers.Timer result = new System.Timers.Timer();
        result.Interval = msTimeOut;
        result.Elapsed += (_,_) => callback();
        result.Enabled = true;
        return result;
    }

    private void clearInterval(System.Timers.Timer timer)
    {
        timer.Stop();
    }

    public bool isStarted() {
        if (this.intervalID != null) return true;
        return false;
    }
}

class NSTimeInterval {
    public void invalidate() {

    }

    public static double timeIntervalSinceReferenceDate() {

        return DateTime.Now.ToOADate();
    }
}