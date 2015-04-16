
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class IntersectionInlet : Object
{
    private int inletIndex;

    public int InletIndex
    {
        get
        {
            return inletIndex;
        }
        set
        {
            inletIndex = value;
        }
    }

    public int LanesIn
    {
        get
        {
            return laneQueues.Length;
        }
        set
        {
            lanesIn = value;
        }
    }

    private int lanesOut;

    public int LanesOut
    {
        get
        {
            return outgoingInlet.lanesIn;
        }
        set
        {
            lanesOut = value;
        }
    }

    private int maxLanes;

    public int MaxLanes
    {
        get
        {

        }
        set
        {
            maxLanes = value;
        }
    }

    private IntersectionInlet outgoingInlet;

    public IntersectionInlet OutgoingInlet
    {
        get
        {
            return outgoingInlet;
        }
        set
        {
            outgoingInlet = value;
        }
    }

    private LaneQueue[] laneQueues = new LaneQueue[3];

    public bool CanLaneTurnLeft(int laneIndex)
    {
        //laneQueues[laneIndex]

        return false;
    }

    public bool CanLaneTurnRight(int laneIndex)
    {
        //laneQueues[laneIndex]

        return false;
    }

    public bool CanLaneGoStraight(int laneIndex)
    {
        //laneQueues[laneIndex]

        return false;
    }

    public bool CanLaneUTurn(int laneIndex)
    {
        //laneQueues[laneIndex]

        return false;
    }
}

