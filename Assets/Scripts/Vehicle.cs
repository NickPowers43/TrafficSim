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
    //time that this vehicle is behind whatever is in front
    private float timeBehind;
    public float TimeBehind
    {
        get
        {
            return timeBehind;
        }
        set
        {
            timeBehind = value;
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
    private float instantiateTime;
    private float destroyTime;
    public float Age //Age in real-time
    {
        get
        {
            return destroyTime - instantiateTime;
        }
    }

    public Vehicle(float instantiateTime, float length, int destination)
	{
        this.instantiateTime = instantiateTime;
        this.length = length;
        this.destination = destination;
	}
}
