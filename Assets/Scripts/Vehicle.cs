using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VehicleType
{
    Car
}
public enum Intention
{
    Food, Gas, Residential, Work
}

public class Vehicle
{
    //Fields
    //time vehicle was last queued
    private double timeQueued;
    public double TimeQueued
    {
        get
        {
            return timeQueued;
        }
        set
        {
            timeQueued = value;
        }
    }
    //index of destination LaneQueue
    private int destination;
    public int Destination
    {
        get
        {
            return destination;
        }
        set
        {
            destination = value;
        }
    }
    //vehicle physical length
    private float length;
    public float Length
    {
        get
        {
            return length;
        }
    }
    //vehicle type
    private VehicleType type;
    public VehicleType Type
    {
        get
        {
            return type;
        }
    }
    //classification of destination LaneQueue's intersection node
    private Intention intention;
    public Intention Intention
    {
        get
        {
            return intention;
        }
    }

    //statistical data
    private double instantiateTime;
    private double destroyTime;
    public double Age //Age in real-time
    {
        get
        {
            return destroyTime - instantiateTime;
        }
    }

    public Vehicle(double instantiateTime, float length, int destination)
	{
        this.instantiateTime = instantiateTime;
        this.length = length;
        this.destination = destination;
	}
}
