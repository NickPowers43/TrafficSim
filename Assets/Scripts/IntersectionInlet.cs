
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class IntersectionInlet
{
    //intersection that owns this inlet
    private Intersection parent;
    public Intersection Parent
    {
        get
        {
            return parent;
        }
    }
    //the index of this inlet. this uniquely identifies an inlet and its position at the intersection
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
    //array of laneQueues starting from the road median
    private LaneQueue[] laneQueues;
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
    //The number of incoming lanes for this inlet
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
    //how many lanes go out of the intersection at this inlet
    private int lanesOut;
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
    //maximum number of inlet lanes allowed
    private int maxLanes;
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
    //the opposite intersection inlet (U-turning vehicles would go here)
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

    public IntersectionInlet(Intersection parent, int inletIndex)
    {
        this.inletIndex = inletIndex;
        this.parent = parent;

        laneQueues = new LaneQueue[1];
        for (int i = 0; i < laneQueues.Length; i++)
        {
            laneQueues[i] = new LaneQueue();
        }
    }

    //methods to check if different turning options are available at the given lane
    public bool CanLaneTurnLeft(int laneIndex)
    {
        throw new NotImplementedException();
    }
    public bool CanLaneTurnRight(int laneIndex)
    {
        throw new NotImplementedException();
    }
    public bool CanLaneGoStraight(int laneIndex)
    {
        throw new NotImplementedException();
    }
    public bool CanLaneUTurn(int laneIndex)
    {
        throw new NotImplementedException();
    }

    //create edges originating at lq and terminating at each LaneQueue in lqs
    public static void ConnectToAll(LaneQueue lq, LaneQueue[] lqs)
    {
        for (int i = 0; i < lqs.Length; i++)
        {
            Utility.WeightedEdge<LaneQueue> edge;
            edge.start = lq;
            edge.end = lqs[i];
            edge.weight = lqs[i].MaxLength;
            LaneQueue.LaneQueueEdges.Add(edge);
        }
    }
    //Connect all of the incoming LaneQueues to all the outgoing LaneQueues of every other inlet
    public void ConnectLaneQueues(IntersectionInlet left, IntersectionInlet straight, IntersectionInlet right)
    {
        for (int i = 0; i < laneQueues.Length; i++)
        {
            LaneQueue.LaneQueues.Add(laneQueues[i]);
            laneQueues[i].Index = LaneQueue.NextIndex++;

            //connect laneQueues[i] to all appropriate other lane queues. Connect them in such a way that
            //all edges originating from laneQueues[i] are consecutive in the LaneQueue.LaneQueueEdges List.


            //TODO: do this differently for cases where it is turn only
            if (left != null)
            {
                if (true) // Left turn lane
                    ConnectToAll(laneQueues[i], left.OutgoingInlet.LaneQueues); 
            }
            if (straight != null)
            {
                if (true) // Straight lane
                    ConnectToAll(laneQueues[i], straight.OutgoingInlet.LaneQueues); 
            }
            if (right != null)
            {
                if (true) // Right turn lane
                    ConnectToAll(laneQueues[i], right.OutgoingInlet.LaneQueues); 
            }
            if (true) // U-turn lane
                ConnectToAll(laneQueues[i], outgoingInlet.LaneQueues);
        }
    }
}

