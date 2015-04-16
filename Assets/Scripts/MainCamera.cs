﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public enum ToolArrays
{
    Select,
    Build
}

public class ToolsArray
{
    public virtual void Deactivate()
    {

    }
    public virtual void Initialize()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void OnClick()
    {

    }
}

public class Tool
{
    public virtual void Deactivate()
    {

    }
    public virtual void Activate()
    {

    }
    public virtual void Initialize()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void OnClick()
    {

    }
}

public class SelectTools : ToolsArray
{
    private const float MAX_HOVER_HIGHLIGHT_DIST = 32.0f;

    class SelectIntersection : Tool
    {
        private LinkedList<Intersection> intersections;

        public SelectIntersection(LinkedList<Intersection> intersections)
        {
            this.intersections = intersections;
        }

        public void Activate()
        {

        }
        public void Deactivate()
        {
            foreach (Intersection node in intersections)
            {
                node.selectionSprite.SetActive(false);
            }
        }
        public void Initialize()
        {

        }
        public void Update()
        {
            //hilight nearby selectable objects

            float dist = float.PositiveInfinity;

            Intersection closest = Intersection.ClosestToCursor(ref dist);

            if (dist < MAX_HOVER_HIGHLIGHT_DIST)
            {
                closest.selectionSprite.SetActive(true);
            }
        }
        public void OnClick()
        {

        }
    }

    public enum Tools
    {
        Intersection
    }

    private Tool activeTool;
    SelectIntersection selectIntersection;

    public SelectTools(LinkedList<Intersection> intersections)
    {
        selectIntersection = new SelectIntersection(intersections);
    }

    public void Activate(Tools toolToActivate)
    {
        switch (toolToActivate)
        {
            case Tools.Intersection:
                activeTool = selectIntersection;
                break;
            default:
                break;
        }

        activeTool.Activate();
    }

    public void Initialize()
    {
        selectIntersection.Initialize();
    }
    public void Deactivate()
    {
        activeTool.Deactivate();
    }
    public void Update()
    {
        activeTool.Update();
    }
    public void OnClick()
    {
        activeTool.OnClick();
    }
}

public class BuildTools : ToolsArray
{
    class Connect : Tool
    {
        Intersection currentSelected;

        public Connect()
        {
            currentSelected = null;
        }

        public void Activate()
        {

        }
        public void Deactivate()
        {

        }
        public void Initialize()
        {

        }
        public void Update()
        {

        }
        public void OnClick()
        {

        }
    }

    class CreateIntersection : Tool
    {
        private GameObject intersectionPrefab;

        public CreateIntersection(GameObject intersectionPrefab)
        {
            this.intersectionPrefab = intersectionPrefab;
        }

        public void Activate()
        {

        }
        public void Deactivate()
        {

        }
        public void Initialize()
        {

        }
        public void Update()
        {
            //hilight nearby road nodes
        }
        public void OnClick()
        {
            Vector3 screenPoint = MainCamera.Instance.Self.ScreenToWorldPoint(Input.mousePosition);
            GameObject nodeGO = (GameObject)GameObject.Instantiate(intersectionPrefab);
            nodeGO.transform.position = new Vector3(screenPoint.x, screenPoint.y, 0.0f);

            //UpdateRoadNetwork();
        }
    }

    public enum Tools
    {
        CreateIntersection,
        Connect
    }

    private Tool activeTool;
    private GameObject buildToolCursor;
    private GameObject intersectionPrefab;
    //Tool objects
    private Connect connect;
    private CreateIntersection createIntersection;

    public BuildTools(GameObject buildToolCursorPrefab, GameObject intersectionPrefab)
    {
        this.intersectionPrefab = intersectionPrefab;

        connect = new Connect();
        createIntersection = new CreateIntersection(intersectionPrefab);

        buildToolCursor = GameObject.Instantiate(buildToolCursorPrefab);
        this.intersectionPrefab = intersectionPrefab;
    }

    public void Activate(Tools tool)
    {
        switch (tool)
        {
            case Tools.CreateIntersection:
                activeTool = createIntersection;
                break;
            case Tools.Connect:
                activeTool = connect;
                break;
            default:
                break;
        }

        buildToolCursor.SetActive(true);
        activeTool.Activate();
    }

    public void Initialize()
    {
        connect.Initialize();
        createIntersection.Initialize();
    }
    public void Deactivate()
    {
        activeTool.Deactivate();
        buildToolCursor.SetActive(false);
    }
    public void Update()
    {
        //rotate cursor
        buildToolCursor.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), Time.deltaTime * 360.0f);
        //position cursor
        Vector3 cursorPosition = MainCamera.Instance.Self.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0.0f;
        buildToolCursor.transform.position = cursorPosition;

        activeTool.Update();
    }
    public void OnClick()
    {
        activeTool.OnClick();
    }
}

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

    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);
    private ToolArrays activeTool = ToolArrays.Select;
    private LinkedList<Intersection> intersections;
    private BuildTools buildTools;
    private SelectTools selectTools;
    private ToolsArray activeTools;
    LinkedListNode<Intersection> currentSelectedIntersection;
    //select tool
    //Intersection currentSelectedIntersection = null;
    //build Connect Tool


    private Camera self;
    public Camera Self
    {
        get
        {
            return self;
        }
    }

    public static void printHello()
    {
        print("Hello");
    }

	void Start () {
        Instance = this;
        self = GetComponent<Camera>();

        intersections = new LinkedList<Intersection>();

        buildTools = new BuildTools(buildToolCursorPrefab, intersectionPrefab);
        buildTools.Initialize();
        selectTools = new SelectTools(intersections);
        selectTools.Initialize();

        Thread t = new Thread(new ThreadStart(printHello));
        t.Start();

        this.prevMousePos = Input.mousePosition;
	}
	
    public void OnBuildRoadConnectClick()
    {
        activeTools.Deactivate();
        buildTools.Activate(BuildTools.Tools.Connect);
        activeTools = buildTools;
    }
    public void OnBuildRoadCreateIntersectionClick()
    {
        activeTools.Deactivate();
        buildTools.Activate(BuildTools.Tools.CreateIntersection);
        activeTools = (ToolsArray)buildTools;
    }
    public void OnSelectClick()
    {
        activeTools.Deactivate();
        selectTools.Activate(SelectTools.Tools.Intersection);
        activeTools = selectTools;
    }

    public void OnWorldSpaceClick()
    {
        activeTools.OnClick();
    }
    private void UpdateActiveTool()
    {
        activeTools.Update();
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

        UpdateActiveTool();

        prevMousePos = Input.mousePosition;
	}
}
