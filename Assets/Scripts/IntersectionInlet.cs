
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

    private int lanesIn;

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
        if (laneIndex == null)
        {
            throw new NotImplementedException("Lane not implemented");
        }
        else
        {
            return true;
        }
    }

    public bool CanLaneTurnRight(int laneIndex)
    {
        if (laneIndex == null)
        {
            throw new NotImplementedException("Lane not implemented");
        }
        else
        {
            return true;
        }
    }

    public bool CanLaneGoStraight(int laneIndex)
    {
        if (laneIndex == null)
        {
            throw new NotImplementedException("Lane not implemented");
        }
        else
        {
            return true;
        }
    }

    public bool CanLaneUTurn(int laneIndex)
    {
        if (laneIndex == null)
        {
            throw new NotImplementedException("Lane not implemented");
        }
        else
        {
            return true;
        }
    }
}

