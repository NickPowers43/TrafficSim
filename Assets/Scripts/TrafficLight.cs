using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Timer;
using System.Linq;
using System.Text;

public class TrafficLight: Object
{
    private bool red;
    private bool yellow;
    private bool green;

    private object threadLock = new object();
    private object streetlight = new object();

    private List<TrafficLight> traffic = new List<TrafficLight>();
    private Queue<Vehicle> vqueue = new Queue<Vehicle>();
    private Vehicle[] v;
    private Navigator n = new Navigator();

    private LaneQueue[] lanequeue;
    private Thread control;
    private TimerCallback callback;
    private Timer t;

    public bool Red
    {
        get
        {
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
            return green;
        }
        set
        {
            green = value;
        }
    }

    public bool Yellow
    {
        get
        {
            return yellow;
        }
        set
        {
            yellow = value;
        }
    }

    public void OperateTrafficLightsatIntersection(int index)
    {
        for (int i = 0; i < index; i++)
        {
            for (int j = 0; j < i; i++)
            {
                
                lock (n.GetNextHop(0, i, j))
                {
                    vqueue.Dequeue();
                }
                lock (n.GetNextHop(0, j, i))
                {
                    vqueue.Enqueue(v[i]);
                }
            }
        }
    }
}
