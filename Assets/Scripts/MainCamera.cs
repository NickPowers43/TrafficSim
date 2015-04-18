using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Tools;

public class MainCamera : MonoBehaviour {

    private static MainCamera instance;
    public static MainCamera Instance
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

    private const float CAMERA_MOVE_SPEED = 5.0f;

    public float intersectionSelectRange = 0.01f;
    public float uiTopBoundary;
    public GameObject buildToolCursorPrefab;
    public GameObject intersectionPrefab;
    public GameObject roadPrefab;

    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);
    private LinkedList<Intersection> intersections;
    private Tools.Build.ToolBar buildTools;
    private Tools.Select.ToolBar selectTools;
    private ToolArray activeTools;
    LinkedListNode<Intersection> currentSelectedIntersection;

    private Camera self;
    public Camera Self
    {
        get
        {
            return self;
        }
    }

	void Start () {

        LaneQueue.NextIndex = 0;

        Tools.Build.BuildTool.Cursor = GameObject.Instantiate(buildToolCursorPrefab);
        Tools.Build.BuildTool.IntersectionPrefab = intersectionPrefab;
        Tools.Build.BuildTool.RoadPrefab = roadPrefab;

        Instance = this;
        self = GetComponent<Camera>();

        intersections = new LinkedList<Intersection>();

        buildTools = new Tools.Build.ToolBar();
        selectTools = new Tools.Select.ToolBar();

        this.prevMousePos = Input.mousePosition;
	}

    private void DeactivateCurrentTool()
    {
        if (activeTools != null)
        {
            activeTools.Deactivate();
        }
    }
    public void OnWorldSpaceClick()
    {
        if (activeTools != null)
        {
            activeTools.OnClick();
        }
    }

    public void OnBuildPointOfInterestClick(int arg)
    {
        DeactivateCurrentTool();
        buildTools.Activate(Tools.Build.ToolBar.Tools.BuildPointOfInterest, arg);
        activeTools = buildTools;
    }
    public void OnBuildRoadClick()
    {
        DeactivateCurrentTool();
        buildTools.Activate(Tools.Build.ToolBar.Tools.BuildRoad);
        activeTools = buildTools;
    }
    public void OnBuildIntersectionClick(bool arg)
    {
        DeactivateCurrentTool();
        buildTools.Activate(Tools.Build.ToolBar.Tools.BuildIntersection, arg);
        activeTools = buildTools;
    }
    public void OnSelectClick()
    {
        DeactivateCurrentTool();
        selectTools.Activate(Tools.Select.ToolBar.Tools.Intersection);
        activeTools = selectTools;
    }

    public void OnStartClick()
    {
        //clear everything
        LaneQueue.LaneQueueEdges.Clear();

        foreach (Intersection intersection in Intersection.Intersections)
        {
            intersection.ConnectInlets();
        }


        List<int> destinationIndices = new List<int>(LaneQueue.NextIndex / 4);
        foreach (LaneQueue laneQueue in LaneQueue.LaneQueues)
        {
            if (laneQueue.IsDestination)
            {
                destinationIndices.Add(laneQueue.Index); 
            }
        }

        float[][] leastCostMat = Utility.BellmanFord.RunAlgorithm(LaneQueue.LaneQueueEdges, LaneQueue.NextIndex);


        //node i is a destination create a column
        LaneQueue[][] nextHopFirst = new LaneQueue[LaneQueue.NextIndex][];
        LaneQueue[][] nextHopSecond = new LaneQueue[LaneQueue.NextIndex][];
        for (int i = 0; i < destinationIndices.Count; i++)
            nextHopFirst[destinationIndices[i]] = new LaneQueue[LaneQueue.NextIndex];
        for (int i = 0; i < destinationIndices.Count; i++)
            nextHopSecond[destinationIndices[i]] = new LaneQueue[LaneQueue.NextIndex];

        int start = 0;
        while (start < Intersection.Intersections.Count)
        {
            for (int i = 0; i < destinationIndices.Count; i++)
                FindNextHop(
                    ref start,
                    leastCostMat,
                    destinationIndices[i],
                    out nextHopFirst[destinationIndices[i]][LaneQueue.LaneQueueEdges[start].start.Index],
                    out nextHopSecond[destinationIndices[i]][LaneQueue.LaneQueueEdges[start].start.Index]);
        }
    }

    private void FindNextHop(ref int start, float[][] leastCostMat, int dest, out LaneQueue first, out LaneQueue second)
    {
        float lowestCost = leastCostMat[LaneQueue.LaneQueueEdges[start].end.Index][dest];
        first = LaneQueue.LaneQueueEdges[start].end;
        second = null;
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

	void Update () {

        transform.position += new Vector3(0.0f, Input.GetAxis("Vertical") * CAMERA_MOVE_SPEED * Time.deltaTime, 0.0f);
        transform.position += new Vector3(Input.GetAxis("Horizontal") * CAMERA_MOVE_SPEED * Time.deltaTime, 0.0f, 0.0f);

        if (Input.GetMouseButton(1))//if user is pressing right mouse button
        {
            //move camera the amount the mouse has moved
            transform.position -= self.ScreenToWorldPoint(Input.mousePosition) - self.ScreenToWorldPoint(prevMousePos);
        }

        self.orthographicSize *= 1.0f + (0.25f * Input.mouseScrollDelta.y);//zoom camera

        if (activeTools != null)
        {
            activeTools.Update();
        }

        prevMousePos = Input.mousePosition;
	}
}
