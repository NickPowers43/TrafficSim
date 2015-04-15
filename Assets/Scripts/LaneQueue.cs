using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

public class LaneQueue
{
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

