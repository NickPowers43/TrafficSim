using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Utility;

public class LaneQueue
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

    public LaneQueue()
    {
        isDestination = true;
        left = true;
        right = true;
        straight = true;
        uturn = true;
    }

    private Queue<Vehicle> vehicleQueue = new Queue<Vehicle>();

    private Vehicle vehicle = new Vehicle();

    void Start(){
        
    }

    public void Queue()
    {
        CurrentLength = vehicleQueue.Sum(length => vehicle.getVehicleLength());
        vehicleQueue.Enqueue(vehicle);
    }
    
    public void DeQueue()
    {
        CurrentLength = vehicleQueue.Sum(length => vehicle.getVehicleLength());
        vehicleQueue.Dequeue();
    }

    public void Full()
    {
        if (MaxLength < CurrentLength)
        {
            Console.WriteLine("Queue is Full.");
        }
    }
}

