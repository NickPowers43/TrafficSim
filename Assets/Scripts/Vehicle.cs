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

    public String getVehicleType()
    {

    }
}
