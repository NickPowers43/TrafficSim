using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VehicleType
{
    Car, Truck, Bus
}
public enum Intention
{
    Food, Gas, Services, Residential, Work, Goods, School
}

public class Vehicle
{
    private int speed;
    private float position;
    private float timeBehind;
    private String type;
    private int index;
    private float length;
    private VehicleType vr;
    private Intention intention;
    private double acceleration;
    private float takeoffTime = Time.time;
    private int destination;

    //statistical data
    private float creationTime;
    private float currentTime;
    public float Age //Age in real-time
    {
        get
        {
            return Time.time - creationTime;
        }
    }

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
    
    public Vehicle()
	{
        creationTime = Time.time;
	}

    public void Run()
    {
        while (true)
        {

        }
    }

    public int getSpeed()
    {
        return speed;
    }

    public int setSpeed(int newSpeed)
    {
        speed = newSpeed;
        return speed;
    }

    public float Position
    {
        get
        {
            return position;
        }
        set
        {
            position = value;
        }
    }

    public VehicleType getVehicleType()
    {
        return vr;
    }

    public VehicleType setVehicleType(VehicleType v){
        if(v == VehicleType.Car){
            vr = v;
        }
        else if(v == VehicleType.Bus){
            vr = v;
        }
        else if(v == VehicleType.Truck){
            vr = v;
        }
        return v;
    }

    public Intention getVehicleIntention()
    {
        return intention;
    }

    public Intention setVehicleIntention(Intention ir)
    {
        if (ir == null)
        {
            throw new NotImplementedException("Intention not implemented");
        }
        else
        {
            intention = ir;
        }
        return ir;
    }

    public float getVehicleLength()
    {
        return length;
    }

    public float setVehicleLength(float l)
    {
        l = length;
        return l;
    }

    public double getAcceleration()
    {
        return acceleration;
    }

    public double setAcceleration(double a)
    {
        a = acceleration;
        return a;
    }
}
