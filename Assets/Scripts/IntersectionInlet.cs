
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
    private LaneQueue[] laneQueues = new LaneQueue[1];
    private int lanesOut;
    private int maxLanes;
    private IntersectionInlet outgoingInlet;

    public LaneQueue[] LaneQueues
    {
        get
        {
            return laneQueues;
        }
        set
        {
            laneQueues = value;
        }
    }
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

    public static void ConnectToAll(LaneQueue lq, LaneQueue[] lqs)
    {
        for (int i = 0; i < lqs.Length; i++)
        {
            Utility.WeightedEdge<LaneQueue> edge;
            edge.start = lq;
            edge.end = lqs[i];
            edge.weight = 1.0f;
            LaneQueue.LaneQueueEdges.Add(edge);
        }
    }

    public void ConnectLaneQueues(IntersectionInlet left, IntersectionInlet straight, IntersectionInlet right)
    {
        for (int i = 0; i < laneQueues.Length; i++)
        {
            laneQueues[i].Index = LaneQueue.NextIndex++;

            //TODO: do this differently for cases where it is turn only
            if (true) // Left turn lane
                ConnectToAll(laneQueues[i], left.LaneQueues);
            if (true) // Straight lane
                ConnectToAll(laneQueues[i], straight.LaneQueues);
            if (true) // Right turn lane
                ConnectToAll(laneQueues[i], right.LaneQueues);
            if (true) // U-turn lane
                ConnectToAll(laneQueues[i], outgoingInlet.LaneQueues);
        }
    }
}

