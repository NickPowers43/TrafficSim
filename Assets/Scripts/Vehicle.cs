using System;
using System.Collections;
using System.Collections.Generic;

enum VehicleType
{
    Car, Truck, Bus
}

public class Vehicle
{
    private int speed;
    private double distance;
    private String type;
    private int index;
    private float length;
    private VehicleType vr;
    private double acceleration;

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
    
    public Vehicle()
	{
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

    public double getDistance()
    {
        return distance;
    }

    public double setDistance(double newDistance)
    {
        distance = newDistance;
        return distance;
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
