using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public enum PointsOfInterest
{
    House = 0,
    Food = 1,
    Fuel = 2,
    Services = 3,
    Work = 4
}

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
    protected Intersection GetHoveredIntersection()
    {
        Vector3 cursorPos = MainCamera.Instance.Self.ScreenToWorldPoint(Input.mousePosition);
        cursorPos.z = BuildTools.INTERSECTION_Z;
        return Intersection.ClosestToPositionAndInRadius(cursorPos);
    }
    protected void ToggleHighlight(Intersection intersection)
    {
        intersection.selectionSprite.SetActive(!intersection.selectionSprite.activeSelf);
    }



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
        private Intersection currentHighlighted;

        public SelectIntersection()
        {
            currentHighlighted = null;
        }

        public override void Activate()
        {

        }
        public override void Deactivate()
        {
            if (currentHighlighted != null)
            {
                currentHighlighted.selectionSprite.SetActive(false);
                currentHighlighted = null;
            }
        }
        public override void Initialize()
        {

        }
        public override void Update()
        {

        }
        public override void OnClick()
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
        selectIntersection = new SelectIntersection();
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

    public override void Initialize()
    {
        selectIntersection.Initialize();
    }
    public override void Deactivate()
    {
        if (activeTool != null)
        {
            activeTool.Deactivate();
            activeTool = null; 
        }
    }
    public override void Update()
    {
        if (activeTool != null)
        {
            activeTool.Update(); 
        }
    }
    public override void OnClick()
    {
        if (activeTool != null)
        {
            activeTool.OnClick(); 
        }
    }
}

public class BuildTools : ToolsArray
{
    public const float CURSOR_Z = -0.25f;
    public const float INTERSECTION_Z = 0.0f;

    class BuildRoad : Tool
    {
        private GameObject roadPrefab;
        private Intersection startNode;

        public BuildRoad(GameObject roadPrefab)
        {
            this.roadPrefab = roadPrefab;
            startNode = null;
        }

        public void Activate()
        {

        }

        public override void Deactivate()
        {
            if (startNode != null)
            {
                startNode.selectionSprite.SetActive(false);
                startNode = null;
            }
        }
        public override void Initialize()
        {

        }
        public override void Update()
        {

        }
        public override void OnClick()
        {
            Intersection hovered = GetHoveredIntersection();

            if (hovered != null)
            {
                if (startNode != null)
                {
                    if (startNode.Degree < 4 && hovered.Degree < 4)
                    {
                        Road road = GameObject.Instantiate(roadPrefab).GetComponent<Road>();
                        road.gameObject.transform.position = (startNode.transform.position + hovered.transform.position) * 0.5f;

                        road.SetEndPoints(startNode.GenerateInlet(), hovered.GenerateInlet());
                    }

                    ToggleHighlight(startNode);
                    startNode = null;
                }
                else
                {
                    startNode = hovered;
                    ToggleHighlight(startNode);
                }
            }

        }
    }

    class BuildIntersection : Tool
    {
        private GameObject intersectionPrefab;
        private GameObject cursor;

        public BuildIntersection(GameObject intersectionPrefab, GameObject cursor)
        {
            this.intersectionPrefab = intersectionPrefab;
            this.cursor = cursor;
        }

        public void Activate()
        {

        }

        public override void Deactivate()
        {

        }
        public override void Initialize()
        {

        }
        public override void Update()
        {
            //color cursor appropriately
            Vector3 cursorPos = MainCamera.Instance.Self.ScreenToWorldPoint(Input.mousePosition);
            cursorPos.z = INTERSECTION_Z;
            if (!Intersection.CheckOverlap(cursorPos, Intersection.INITIAL_RADIUS))
            {
                //highlight cursor green
                cursor.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f);
            }
            else
            {
                //highlight cursor red
                cursor.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f);
            }
        }
        public override void OnClick()
        {
            Vector3 screenPoint = MainCamera.Instance.Self.ScreenToWorldPoint(Input.mousePosition);
            GameObject nodeGO = (GameObject)GameObject.Instantiate(intersectionPrefab);
            nodeGO.transform.position = new Vector3(screenPoint.x, screenPoint.y, INTERSECTION_Z);

            //UpdateRoadNetwork();
        }
    }

    class BuildPointOfInterest : Tool
    {

        public BuildPointOfInterest()
        {

        }

        public void Activate()
        {

        }

        public override void Deactivate()
        {

        }
        public override void Initialize()
        {

        }
        public override void Update()
        {

        }
        public override void OnClick()
        {

        }
    }

    public enum Tools
    {
        BuildPointOfInterest,
        BuildIntersection,
        BuildRoad
    }

    private Tool activeTool = null;
    private GameObject buildToolCursor;
    private GameObject intersectionPrefab;
    //Tool objects
    private BuildRoad buildRoad;
    private BuildIntersection buildIntersection;
    private BuildPointOfInterest buildPointOfInterest;

    public BuildTools(GameObject buildToolCursorPrefab, GameObject roadPrefab, GameObject intersectionPrefab)
    {
        this.intersectionPrefab = intersectionPrefab;

        buildToolCursor = GameObject.Instantiate(buildToolCursorPrefab);

        buildRoad = new BuildRoad(roadPrefab);
        buildIntersection = new BuildIntersection(intersectionPrefab, buildToolCursor);
        buildPointOfInterest = new BuildPointOfInterest();
    }

    public void Activate(Tools tool)
    {
        switch (tool)
        {
            case Tools.BuildPointOfInterest:
                activeTool = buildPointOfInterest;
                break;
            case Tools.BuildIntersection:
                activeTool = buildIntersection;
                break;
            case Tools.BuildRoad:
                activeTool = buildRoad;
                break;
            default:
                break;
        }

        buildToolCursor.SetActive(true);
        activeTool.Activate();
    }

    public override void Initialize()
    {
        buildRoad.Initialize();
        buildIntersection.Initialize();
    }
    public override void Deactivate()
    {
        if (activeTool != null)
        {
            activeTool.Deactivate();
            activeTool = null;
        }
        buildToolCursor.SetActive(false);
    }
    public override void Update()
    {
        if (activeTool != null)
        {
            activeTool.Update();

            //rotate cursor
            buildToolCursor.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), Time.deltaTime * 360.0f);
            //position cursor
            Vector3 cursorPosition = MainCamera.Instance.Self.ScreenToWorldPoint(Input.mousePosition);
            cursorPosition.z = CURSOR_Z;
            buildToolCursor.transform.position = cursorPosition;
        }
    }
    public override void OnClick()
    {
        if (activeTool != null)
        {
            activeTool.OnClick(); 
        }
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
    public GameObject roadPrefab;

    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);
    private ToolArrays activeTool = ToolArrays.Select;
    private LinkedList<Intersection> intersections;
    private BuildTools buildTools;
    private SelectTools selectTools;
    private ToolsArray activeTools;
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
        buildTools.Initialize();
        selectTools = new SelectTools(intersections);
        selectTools.Initialize();

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
	
    public void OnBuildPointOfInterestClick(int poi)
    {

    }

    public void OnBuildRoadClick()
    {
        DeactivateCurrentTool();
        buildTools.Activate(BuildTools.Tools.BuildRoad);
        activeTools = buildTools;
    }
    public void OnBuildIntersectionClick()
    {
        DeactivateCurrentTool();
        buildTools.Activate(BuildTools.Tools.BuildIntersection);
        activeTools = buildTools;
    }
    public void OnSelectClick()
    {
        DeactivateCurrentTool();
        selectTools.Activate(SelectTools.Tools.Intersection);
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
