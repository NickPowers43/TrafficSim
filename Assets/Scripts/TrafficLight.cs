using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Timer;
using System.Linq;
using System.Text;

public class TrafficLight
{
    private bool red;
    private bool yellow;
    private bool green;

    private object threadLock = new object();

    private LaneQueue lanequeue = new LaneQueue();
    private Thread control;
    private TimerCallback callback;
    private Timer t;

    public bool Red
    {
        get
        {
            PauseThread();
            return red;
        }
        set
        {
            red = value;
        }
    }

    public bool Green
    {
        get
        {
            ResumeThread();
            return green;
        }
        set
        {
            green = value;
        }
    }

    public void SlowDown()
    {
        if (yellow == true)
        {
            callback = new TimerCallback(lanequeue);
        }
    }

    public void PauseThread()
    {
        if (red == true)
        {
            lock (threadLock)
            {
                control = lanequeue.WaitingThread;
                Monitor.Wait(control);
            }
        }
    }

    public void ResumeThread()
    {
        if (green == true)
        {
            lock (threadLock)
            {
                control = lanequeue.WaitingThread;
                Monitor.Pulse(control);
            }
        }
    }
}
