
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class IntersectionInlet : Object
{
    private int inletIndex;
    private LaneQueue[] laneQueues = new LaneQueue[3];
    private int lanesOut;
    private int maxLanes;

    private IntersectionInlet outgoingInlet;

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

