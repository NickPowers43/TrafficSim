using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Navigator
{
    private static Navigator instance;
    public static Navigator Instance
    {
        get
        {
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    private readonly LaneQueue[][][] nextHopMats;

    public LaneQueue GetNextHop(int priority, int src, int dst)
    {
        if (nextHopMats[priority][dst] != null)
        {
            return nextHopMats[priority][dst][src];
        }
        else
        {
            throw new ArgumentException("LaneQueue(" + dst.ToString() + ") is not a destination");
        }
    }

    /// <summary>
    /// <remarks>
    /// Ensure that the following variables are populated:
    /// LaneQueue.LaneQueues,
    /// LaneQueue.LaneQueueEdges,
    /// LaneQueue.NextIndex
    /// </remarks>
    /// </summary>
    public Navigator()
    {
        //clear everything
        LaneQueue.LaneQueueEdges.Clear();
        LaneQueue.NextIndex = 0;

        //build new LaneQueue edges
        foreach (Intersection intersection in Intersection.Intersections)
        {
            intersection.ConnectInlets();
        }

        //Run Bellman Ford algorithm to compute the lowest cost to get from
        //node i to node j.
        float[][] leastCostMat = Utility.BellmanFord.RunAlgorithm(LaneQueue.LaneQueueEdges, LaneQueue.NextIndex);

        //generate next hop matrices
        //the vehicles are assumed to never query most of the columns of this matrix. 
        //The matrices therefore will have nullable entries for the column
        List<int> destinationIndices = GetListOfDestinationIndices();
        nextHopMats = GetNextHopMatrices(2, destinationIndices);

        //Fill the non-null columns of the nextHop matrices
        PopulateNextHopMatrices(leastCostMat, destinationIndices, nextHopMats);

        LaneQueue.LaneQueueEdges.Clear();
    }

    private LaneQueue[][][] GetNextHopMatrices(int priorities, List<int> destinations)
    {
        LaneQueue[][][] output = new LaneQueue[priorities][][];

        for (int i = 0; i < priorities; i++)
        {
            output[i] = new LaneQueue[LaneQueue.NextIndex][];
            for (int j = 0; j < destinations.Count; j++)
            {
                output[i][destinations[j]] = new LaneQueue[LaneQueue.NextIndex];
            }
        }

        return output;
    }
    private List<int> GetListOfDestinationIndices()
    {
        List<int> output = new List<int>(LaneQueue.NextIndex / 4);

        foreach (LaneQueue laneQueue in LaneQueue.LaneQueues)
        {
            if (laneQueue.IsDestination)
            {
                output.Add(laneQueue.Index);
            }
        }

        return output;
    }
    private void PopulateNextHopMatrices(float[][] leastCostMat, List<int> dstIndices, LaneQueue[][][] nextHopMats)
    {
        int start = 0;
        while (start < Intersection.Intersections.Count)
        {
            for (int i = 0; i < dstIndices.Count; i++)
                FindNextHop(
                    ref start,
                    leastCostMat,
                    dstIndices[i],
                    out nextHopMats[0][dstIndices[i]][LaneQueue.LaneQueueEdges[start].start.Index],
                    out nextHopMats[1][dstIndices[i]][LaneQueue.LaneQueueEdges[start].start.Index]);
        }
    }
    private void FindNextHop(ref int start, float[][] leastCostMat, int dest, out LaneQueue first, out LaneQueue second)
    {
        float lowestCost = leastCostMat[LaneQueue.LaneQueueEdges[start].end.Index][dest];
        first = LaneQueue.LaneQueueEdges[start].end;
        second = first;
        LaneQueue startNode = LaneQueue.LaneQueueEdges[start].start;

        //continue until we meet an edge that does not start from startNode
        while (startNode == LaneQueue.LaneQueueEdges[++start].start)
        {
            float cost = leastCostMat[LaneQueue.LaneQueueEdges[start].end.Index][dest];
            if (lowestCost > cost)
            {
                lowestCost = cost;
                second = first;
                first = LaneQueue.LaneQueueEdges[start].end;
            }
        }
    }
}
