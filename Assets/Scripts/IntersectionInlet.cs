
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
}

