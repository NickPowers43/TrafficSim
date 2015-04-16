using UnityEngine;
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

        public override void Activate()
        {

        }
        public override void Deactivate()
        {
            foreach (Intersection node in intersections)
            {
                node.selectionSprite.SetActive(false);
            }
        }
        public override void Initialize()
        {

        }
        public override void Update()
        {
            //hilight nearby selectable objects

            float dist = float.PositiveInfinity;

            Intersection closest = Intersection.ClosestToCursor(ref dist);

            if (dist < MAX_HOVER_HIGHLIGHT_DIST)
            {
                closest.selectionSprite.SetActive(true);
            }
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
    private const float CURSOR_Z = -0.25f;
    private const float INTERSECTION_Z = 0.0f;

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

    class CreateIntersection : Tool
    {
        private GameObject intersectionPrefab;
        private GameObject cursor;

        public CreateIntersection(GameObject intersectionPrefab, GameObject cursor)
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

    public enum Tools
    {
        CreateIntersection,
        Connect
    }

    private Tool activeTool = null;
    private GameObject buildToolCursor;
    private GameObject intersectionPrefab;
    //Tool objects
    private Connect connect;
    private CreateIntersection createIntersection;

    public BuildTools(GameObject buildToolCursorPrefab, GameObject intersectionPrefab)
    {
        this.intersectionPrefab = intersectionPrefab;

        buildToolCursor = GameObject.Instantiate(buildToolCursorPrefab);

        connect = new Connect();
        createIntersection = new CreateIntersection(intersectionPrefab, buildToolCursor);
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

    public override void Initialize()
    {
        connect.Initialize();
        createIntersection.Initialize();
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

	void Start () {
        Instance = this;
        self = GetComponent<Camera>();

        intersections = new LinkedList<Intersection>();

        buildTools = new BuildTools(buildToolCursorPrefab, intersectionPrefab);
        buildTools.Initialize();
        selectTools = new SelectTools(intersections);
        selectTools.Initialize();

        this.prevMousePos = Input.mousePosition;
	}
	
    public void OnBuildRoadConnectClick()
    {
        if (activeTools != null)
        {
            activeTools.Deactivate(); 
        }
        buildTools.Activate(BuildTools.Tools.Connect);
        activeTools = buildTools;
    }
    public void OnBuildRoadCreateIntersectionClick()
    {
        if (activeTools != null)
        {
            activeTools.Deactivate(); 
        }
        buildTools.Activate(BuildTools.Tools.CreateIntersection);
        activeTools = buildTools;
    }
    public void OnSelectClick()
    {
        if (activeTools != null)
        {
            activeTools.Deactivate(); 
        }
        selectTools.Activate(SelectTools.Tools.Intersection);
        activeTools = selectTools;
    }

    public void OnWorldSpaceClick()
    {
        activeTools.OnClick();
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
