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

    private readonly LaneQueue[][][] transitionMats;

    public LaneQueue GetTransition(int priority, int src, int dst)
    {
        if (transitionMats[priority][dst] != null)
        {
            return transitionMats[priority][dst][src];
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
        foreach (Intersection intersection in Intersection.Intersections) { intersection.IndexLaneQueues(ref nextIndex, destinationIndices); }
        //get edges
        foreach (Intersection intersection in Intersection.Intersections) { intersection.ConnectLaneQueues(lqEdges); }

        //Run Bellman Ford algorithm to compute the lowest cost to get from
        //node i to node j.
        float[][] leastCostMat = Utility.BellmanFord.RunAlgorithm(lqEdges, nextIndex);

        //generate next hop matrices
        //the vehicles are assumed to never query most of the columns of this matrix. 
        //The matrices therefore will have nullable entries for the column
        transitionMats = GetTransitionMatrices(2, destinationIndices, nextIndex);

        //Fill the non-null columns of the transition matrices
        PopulateTransitionMatrices(leastCostMat, destinationIndices, transitionMats, lqEdges, nextIndex);

        return;
    }

    private LaneQueue[][][] GetTransitionMatrices(int priorities, List<int> destinations, int size)
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
    private void PopulateTransitionMatrices(float[][] leastCostMat, List<int> dstIndices, LaneQueue[][][] transitionMats, List<Utility.WeightedEdge<LaneQueue>> lqEdges, int size)
    {
        int start = 0;
        while (start < size)
        {
            int sortStart = start;
            while (lqEdges[++start].start == lqEdges[sortStart].start && start < size) { }

            for (int i = 0; i < dstIndices.Count; i++)
                FindTransition(
                    sortStart,
                    leastCostMat,
                    dstIndices[i],
                    out transitionMats[0][dstIndices[i]][lqEdges[start].start.Index],
                    out transitionMats[1][dstIndices[i]][lqEdges[start].start.Index],
                    lqEdges,
                    size);

        }
    }
    private void FindTransition(int sortStart, float[][] leastCostMat, int dest, out LaneQueue first, out LaneQueue second, List<Utility.WeightedEdge<LaneQueue>> lqEdges, int size)
    {
        float lowestCost = leastCostMat[lqEdges[sortStart].end.Index][dest];
        first = lqEdges[sortStart].end;
        second = first;
        LaneQueue startNode = lqEdges[sortStart].start;

        //continue until we meet an edge that does not start from startNode
        while (sortStart < size && startNode == lqEdges[sortStart].start)
        {
            float cost = leastCostMat[lqEdges[sortStart].end.Index][dest];
            if (lowestCost > cost)
            {
                lowestCost = cost;
                second = first;
                first = lqEdges[sortStart].end;
            }

            sortStart++;
        }

    }
}
