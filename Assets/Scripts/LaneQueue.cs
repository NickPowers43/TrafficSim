using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Vehicle;

public class LaneQueue : MonoBehaviour
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
        CurrentLength = vehicleQueue.Sum(length => vehicle.getVehicleLength());
        lock (vehicleQueue)
        {
            vehicleQueue.Enqueue(vehicle);
        }
    }
    public void DeQueue()
    {
        CurrentLength = vehicleQueue.Sum(length => vehicle.getVehicleLength());
        lock (vehicleQueue)
        {
            vehicleQueue.Dequeue();
        }
    }

    public void Full()
    {
        if (MaxLength < CurrentLength)
        {
            print("Queue is Full.");
        }
    }
}

