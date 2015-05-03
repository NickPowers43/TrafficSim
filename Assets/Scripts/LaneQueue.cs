using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Utility;

public class LaneQueue : Queue<Vehicle>
{
    private static List<LaneQueue> laneQueues = new List<LaneQueue>();
    public static List<LaneQueue> LaneQueues
    {
        get
        {
            return laneQueues;
        }
    }

    private static List<WeightedEdge<LaneQueue>> laneQueueEdges = new List<WeightedEdge<LaneQueue>>();
    public static List<WeightedEdge<LaneQueue>> LaneQueueEdges
    {
        get
        {
            return laneQueueEdges;
        }
    }

    private static int nextIndex;
    public static int NextIndex
    {
        get
        {
            return nextIndex;
        }
        set
        {
            nextIndex = value;
        }
    }

    private int index;
    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }
    //thead that is waiting for first vehicle to stop
    private Thread waitingThread;
    public Thread WaitingThread
    {
        get
        {
            return waitingThread;
        }
        set
        {
            waitingThread = value;
        }
    }
    //maximum physical length of this section of road
    private float maxLength;
    public float MaxLength
    {
        get
        {
            return maxLength;
        }
        set
        {
            maxLength = value;
        }
    }
    //sum of contained vehicles
    private float currentLength;
    public float CurrentLength
    {
        get
        {
            return currentLength;
        }
        set
        {
            currentLength = value;
        }
    }

    private bool turningSymbol;
    public bool TurningSymbol
    {
        get
        {
            return turningSymbol;
        }
        set
        {
            turningSymbol = value;
        }
    }

    private bool straight;
    public bool Straight
    {
        get
        {
            return straight;
        }
        set
        {
            straight = value;
        }
    }

    private bool left;
    public bool Left
    {
        get
        {
            return left;
        }
        set
        {
            left = value;
        }
    }

    private bool right;
    public bool Right
    {
        get
        {
            return right;
        }
        set
        {
            right = value;
        }
    }

    private bool uturn;
    public bool Uturn
    {
        get
        {
            return uturn;
        }
        set
        {
            uturn = value;
        }
    }

    private bool isDestination;
    public bool IsDestination
    {
        get
        {
            return isDestination;
        }
        set
        {
            isDestination = value;
        }
    }

    private float speedLimit;
    public float SpeedLimit
    {
        get
        {
            return speedLimit;
        }
        set
        {
            speedLimit = value;
        }
    }

    public LaneQueue(float speedLimit, float maxLength, bool isDestination)
    {
        this.speedLimit = speedLimit;
        this.maxLength = maxLength;
        this.isDestination = isDestination;

        left = true;
        right = true;
        straight = true;
        uturn = true;
    }

    public void Enqueue(Vehicle vehicle)
    {
        base.Enqueue(vehicle);

        CurrentLength += vehicle.Length + TimeToDistance(vehicle.TimeBehind);
    }
    public Vehicle DeQueue()
    {
        Vehicle v = base.Dequeue();

        CurrentLength -= v.Length;

        return v;
    }

    public bool Available(float length)
    {
        return MaxLength > CurrentLength + length;
    }

    public void SimulateTime(float seconds)
    {
        foreach (Vehicle v in this)
        {
            if (v.TimeBehind != 0.0f)
            {
                if (seconds != 0.0f)
                {
                    if (seconds > v.TimeBehind)
                    {
                        seconds -= v.TimeBehind;
                        v.TimeBehind = 0.0f;
                        //awaken thread that may be waiting for this vehicle
                        waitingThread.Interrupt();
                    }
                    else
                    {
                        v.TimeBehind -= seconds;
                        seconds = 0.0f;
                    } 
                }
                else
                {
                    break;
                }
            }
        }
    }
    //the amount of time a vehicle takes to travel a given distance
    public float DistanceToTime(float distance)
    {
        return distance / speedLimit;
    }
    public float TimeToDistance(float time)
    {
        return time / speedLimit;
    }
}

