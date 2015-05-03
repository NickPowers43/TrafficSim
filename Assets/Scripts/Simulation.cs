﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

class Simulation
{
    private static double speedMultiplier = 1.0;
    public static double SpeedMultiplier
    {
        get
        {
            return speedMultiplier;
        }
        set
        {
            speedMultiplier = value;
        }
    }

    private static bool running;
    public static bool Running
    {
        get
        {
            return running;
        }
        set
        {
            running = value;
        }
    }

    public static double GetTime()
    {
        return DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
    }

    public static double SimulationTime(double realTime)
    {
        return realTime * speedMultiplier;
    }

    public static void SleepSimSeconds(double seconds)
    {
        Thread.Sleep(new TimeSpan((long)(TimeSpan.TicksPerSecond * seconds * speedMultiplier)));
    }
}
