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
    public GameObject stopSimulationButton;

    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);
    private Tools.Build.ToolBar buildTools;
    private Tools.Select.ToolBar selectTools;
    private ToolArray activeTools;

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

        activeTools = null;
        buildTools = new Tools.Build.ToolBar();
        selectTools = new Tools.Select.ToolBar();

        this.prevMousePos = Input.mousePosition;
	}

    public void OnWorldSpaceClick()
    {
        if (activeTools != null)
        {
            activeTools.OnClick();
        }
    }
    private ToolArray ActiveTools
    {
        set
        {
            if (activeTools != null)
            {
                activeTools.Deactivate();
            }
            activeTools = value;
        }
    }

    public void OnBuildPointOfInterestClick(int arg)
    {
        ActiveTools = buildTools;
        buildTools.Activate(Tools.Build.ToolBar.Tools.BuildPointOfInterest, arg);
    }
    public void OnBuildRoadClick(bool arg)
    {
        ActiveTools = buildTools;
        buildTools.Activate(Tools.Build.ToolBar.Tools.BuildRoad, arg);
    }
    public void OnBuildIntersectionClick(bool arg)
    {
        ActiveTools = buildTools;
        buildTools.Activate(Tools.Build.ToolBar.Tools.BuildIntersection, arg);
    }
    public void OnSelectClick()
    {
        ActiveTools = selectTools;
        selectTools.Activate(Tools.Select.ToolBar.Tools.Intersection);
    }

    bool simulationRunning = false;

    public void OnStartClick()
    {
        if (simulationRunning)
        {
            OnStopClick();
        }

        Navigator navigator = new Navigator();
        Navigator.Instance = navigator;

        //TODO: start simulation
        if (simulationRunning == true)
        {
            stopSimulationButton.SetActive(true);
        }
    }

    public void OnStopClick()
    {
        stopSimulationButton.SetActive(false);

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
