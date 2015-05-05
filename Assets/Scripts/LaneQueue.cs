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
    public static double DEFAULT_SPEED_LIMIT = 0.1f;

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
    private double maxLength;
    public double MaxLength
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
    private double currentLength;
    public double CurrentLength
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

    private double speedLimit;
    public double SpeedLimit
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

    public LaneQueue(double speedLimit, double maxLength, bool isDestination)
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

        vehicle.TimeQueued = Simulation.GetTime();
        CurrentLength += vehicle.Length + TimeToDistance(vehicle.TimeQueued - Peek().TimeQueued);
    }
    public Vehicle DeQueue()
    {
        Vehicle v = base.Dequeue();

        CurrentLength -= v.Length;

        return v;
    }

    public bool Available(double length)
    {
        return MaxLength > CurrentLength + length;
    }

    //the amount of time a vehicle takes to travel a given distance
    public double DistanceToTime(double distance)
    {
        return distance / speedLimit;
    }
    //the distance a vehicle travels within the given time
    public double TimeToDistance(double time)
    {
        return time / speedLimit;
    }
}

