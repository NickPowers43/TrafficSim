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
    private ToolArrays activeTool = ToolArrays.Select;
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
        Instance = this;
        self = GetComponent<Camera>();

        intersections = new LinkedList<Intersection>();

        buildTools = new BuildTools(buildToolCursorPrefab, roadPrefab, intersectionPrefab);
        selectTools = new SelectTools(intersections);

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
        activeTools.OnClick();
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
