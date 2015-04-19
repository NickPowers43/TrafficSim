
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class IntersectionInlet : Object
{
    private Intersection parent;
    private int inletIndex;
    private LaneQueue[] laneQueues = new LaneQueue[3];
    private int lanesOut;
    private int maxLanes;
    private IntersectionInlet outgoingInlet;

    public Intersection Parent
    {
        get
        {
            return parent;
        }
    }
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
            LaneQueue[] t = new LaneQueue[value];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = laneQueues[i];
                laneQueues = t;
            }
        }
    }
    public int LanesOut
    {
        get
        {
            return outgoingInlet.LanesIn;
        }
        set
        {
            lanesOut = value;
        }
    }
    public int MaxLanes
    {
        get
        {
            //TODO: replace when more than one LaneQueue array is allowed
            return 1;
        }
        set
        {
            maxLanes = value;
        }
    }
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

    public void ConnectLaneQueues(IntersectionInlet left, IntersectionInlet forward, IntersectionInlet right)
    {
        for (int i = 0; i < 3; i++)
        {
            if (left.CanLaneTurnLeft(i) == true && forward.CanLaneGoStraight(i) == true)
            {
                left = forward;
            }
            if (left.CanLaneTurnLeft(i) == true && right.CanLaneTurnRight(i) == true)
            {
                left = right;
            }
            if (right.CanLaneTurnRight(i) == true && forward.CanLaneGoStraight(i) == true)
            {
                right = forward;
            }
        }
    }

    public IntersectionInlet(Intersection parent)
    {
        this.parent = parent;
    }

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

