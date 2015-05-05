using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

    public Navigator()
    {
        //get LaneQueue graph
        List<LaneQueue> laneQueues = new List<LaneQueue>();
        List<Utility.WeightedEdge<LaneQueue>> lqEdges = new List<Utility.WeightedEdge<LaneQueue>>();
        List<int> destinationIndices = new List<int>();
        int nextIndex = 0;

        //get nodes
        foreach (Intersection intersection in Intersection.Intersections)
        {
            intersection.IndexLaneQueues(ref nextIndex, destinationIndices);
        }
        //get edges
        foreach (Intersection intersection in Intersection.Intersections)
        {
            intersection.ConnectLaneQueues(lqEdges);
        }

        //Run Bellman Ford algorithm to compute the lowest cost to get from
        //node i to node j.
        float[][] leastCostMat = Utility.BellmanFord.RunAlgorithm(lqEdges, nextIndex);

        //generate next hop matrices
        //the vehicles are assumed to never query most of the columns of this matrix. 
        //The matrices therefore will have nullable entries for the column
        nextHopMats = GetNextHopMatrices(2, destinationIndices, nextIndex);

        //Fill the non-null columns of the nextHop matrices
        PopulateNextHopMatrices(leastCostMat, destinationIndices, nextHopMats, lqEdges, nextIndex);

        Debug.Log("Navigator initialized");

    }

    private LaneQueue[][][] GetNextHopMatrices(int priorities, List<int> destinations, int size)
    {
        LaneQueue[][][] output = new LaneQueue[priorities][][];

        for (int i = 0; i < priorities; i++)
        {
            output[i] = new LaneQueue[size][];
            for (int j = 0; j < destinations.Count; j++)
            {
                output[i][destinations[j]] = new LaneQueue[size];
            }
        }

        return output;
    }
    private void PopulateNextHopMatrices(float[][] leastCostMat, List<int> dstIndices, LaneQueue[][][] nextHopMats, List<Utility.WeightedEdge<LaneQueue>> lqEdges, int size)
    {
        int start = 0;
        while (start < size)
        {
            for (int i = 0; i < dstIndices.Count; i++)
                FindNextHop(
                    ref start,
                    leastCostMat,
                    dstIndices[i],
                    out nextHopMats[0][dstIndices[i]][lqEdges[start].start.Index],
                    out nextHopMats[1][dstIndices[i]][lqEdges[start].start.Index],
                    lqEdges,
                    size);
        }
    }
    private void FindNextHop(ref int start, float[][] leastCostMat, int dest, out LaneQueue first, out LaneQueue second, List<Utility.WeightedEdge<LaneQueue>> lqEdges, int size)
    {
        float lowestCost = leastCostMat[lqEdges[start].end.Index][dest];
        first = lqEdges[start].end;
        second = first;
        LaneQueue startNode = lqEdges[start].start;

        //continue until we meet an edge that does not start from startNode
        while (++start < size)
        {
            if (startNode == lqEdges[start].start)
            {
                float cost = leastCostMat[lqEdges[start].end.Index][dest];
                if (lowestCost > cost)
                {
                    lowestCost = cost;
                    second = first;
                    first = lqEdges[start].end;
                }
            }
            else
                break;
        }
    }
}
