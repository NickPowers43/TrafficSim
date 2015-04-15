﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Vehicle;

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

    private Thread WaitingThread;
    private float MaxLength;
    private float CurrentLength;

    private Queue<Vehicle> vehicleQueue = new Queue<Vehicle>();

    private Vehicle vehicle = new Vehicle();

    void Start(){
        
    }

    public void Queue(){
        lock (vehicleQueue)
        {
            CurrentLength = vehicleQueue.Sum(length => vehicle.getVehicleLength());
            vehicleQueue.Enqueue(vehicle);
        }
    }
    public void DeQueue()
    {
        lock (vehicleQueue)
        {
            CurrentLength = vehicleQueue.Sum(length => vehicle.getVehicleLength());
            vehicleQueue.Dequeue();
        }
    }

    public void Full()
    {
        if (MaxLength < CurrentLength)
        {
            Console.WriteLine("Queue is Full.");
        }
    }
}

