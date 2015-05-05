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

    private readonly LaneQueue[][] transitionMat;

    public LaneQueue GetTransition(int src, int dst)
    {
        return transitionMat[src][dst];
    }

    public Navigator()
    {
        //get LaneQueue graph
        List<LaneQueue> laneQueues = new List<LaneQueue>();
        int nextIndex = 0;

        //get nodes
        foreach (Intersection intersection in Intersection.Intersections) { intersection.IndexLaneQueues(ref nextIndex); }
        //get edges
        List<Utility.WeightedEdge<LaneQueue>>[] lqEdges = new List<Utility.WeightedEdge<LaneQueue>>[nextIndex];
        foreach (Intersection intersection in Intersection.Intersections) { intersection.ConnectLaneQueues(lqEdges); }

        //Run Bellman Ford algorithm to compute the lowest cost to get from
        //node i to node j.
        float[][] leastCostMat = Utility.BellmanFord.RunAlgorithm(lqEdges, nextIndex);

        //generate next hop matrices
        //the vehicles are assumed to never query most of the columns of this matrix. 
        //The matrices therefore will have nullable entries for the column
        transitionMat = GetTransitionMatrix(nextIndex);

        //Fill the non-null columns of the transition matrices
        PopulateTransitionMatrices(leastCostMat, transitionMat, lqEdges, nextIndex);

        //Utility.Utility.PrintLQMatrix(transitionMat);

        return;
    }

    private LaneQueue[][] GetTransitionMatrix(int size)
    {
        LaneQueue[][] output = new LaneQueue[size][];

        for (int j = 0; j < size; j++)
        {
            output[j] = new LaneQueue[size];
        }

        return output;
    }
    private void PopulateTransitionMatrices(float[][] leastCostMat, LaneQueue[][] transitionMat, List<Utility.WeightedEdge<LaneQueue>>[] lqEdges, int size)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float lowestCost = leastCostMat[lqEdges[i][0].end.Index][j];
                LaneQueue first = lqEdges[i][0].end;

                for (int k = 1; k < lqEdges[i].Count; k++)
                {
                    float cost = leastCostMat[lqEdges[i][k].end.Index][j];
                    if (lowestCost > cost)
                    {
                        lowestCost = cost;
                        first = lqEdges[i][k].end;
                    }
                }

                transitionMat[i][j] = first;
            }
        }
    }
}
