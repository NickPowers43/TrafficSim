using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

enum ActiveTool
{
    Select,
    BuildRoadConnect,
    BuildRoadCreateIntersection
}

public class MainCamera : MonoBehaviour {

    private const float MAX_HOVER_HIGHLIGHT_DIST = 32.0f;
    private const float CAMERA_MOVE_SPEED = 5.0f;

    public float roadNodeSelectRange = 0.01f;
    public float uiTopBoundary;
    public GameObject buildToolCursorPrefab;
    public GameObject roadNodePrefab;

    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);
    private ActiveTool activeTool = ActiveTool.Select;
    private GameObject buildToolCursor;
    private Camera self;
    private LinkedList<RoadNode> roadNodes = new LinkedList<RoadNode>();
    //build tool
    LinkedListNode<RoadNode> currentSelectedRoadNode;
    //select tool
    //RoadNode currentSelectedRoadNode = null;

    public static void printHello()
    {
        print("Hello");
    }

	void Start () {

        Thread t = new Thread(new ThreadStart(printHello));
        t.Start();

        //Cursor.visible = false;
        self = GetComponent<Camera>();
        buildToolCursor = GameObject.Instantiate(buildToolCursorPrefab);

        this.prevMousePos = Input.mousePosition;
	}
	
    private void DeselectCurrentTool()
    {
        switch (activeTool)
        {
            case ActiveTool.Select:
                foreach (RoadNode node in roadNodes)
                {
                    node.selectionSprite.SetActive(false);
                }
                return;
            case ActiveTool.BuildRoadConnect:
                buildToolCursor.SetActive(false);
                break;
            case ActiveTool.BuildRoadCreateIntersection:
                buildToolCursor.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void OnBuildRoadConnectClick()
    {
        //cancel what we were previously doing
        DeselectCurrentTool();

        //initialize build road tool
        buildToolCursor.SetActive(true);

        activeTool = ActiveTool.BuildRoadConnect;
    }
    public void OnBuildRoadCreateIntersectionClick()
    {
        //cancel what we were previously doing
        DeselectCurrentTool();

        //initialize build road tool
        buildToolCursor.SetActive(true);

        activeTool = ActiveTool.BuildRoadCreateIntersection;
    }
    public void OnSelectClick()
    {
        //cancel what we were previously doing
        DeselectCurrentTool();

        activeTool = ActiveTool.Select;
    }
    public void OnWorldSpaceClick()
    {
        switch (activeTool)
        {
            case ActiveTool.Select:
                OnSelectWSClick();
                break;
            case ActiveTool.BuildRoadConnect:
                OnBuildRoadConnectWSClick();
                break;
            case ActiveTool.BuildRoadCreateIntersection:
                OnBuildRoadCreateIntersectionWSClick();
                break;
            default:
                break;
        }
    }

    private void OnSelectWSClick()
    {
        //select anything within range
    }
    private void OnBuildRoadConnectWSClick()
    {
        //TODO 

        print("Not Implemented");
    }
    private void OnBuildRoadCreateIntersectionWSClick()
    {
        Vector3 screenPoint = self.ScreenToWorldPoint(Input.mousePosition);
        GameObject nodeGO = (GameObject)GameObject.Instantiate(roadNodePrefab);
        nodeGO.transform.position = new Vector3(screenPoint.x, screenPoint.y, 0.0f);

        roadNodes.AddLast(nodeGO.GetComponent<RoadNode>());

        UpdateRoadNetwork();
    }

    private void UpdateRoadNetwork()
    {
        int i = 0;
        foreach (RoadNode node in roadNodes)
        {
            node.Index = i++;
            print(node.Index.ToString());
        }


    }

    private void UpdateSelectTool()
    {
        //hilight nearby selectable objects

        RoadNode closest = null;
        float dist = float.PositiveInfinity;
        foreach (RoadNode node in roadNodes)
        {
            float currDist = Vector3.Magnitude(self.WorldToScreenPoint(node.transform.position) - Input.mousePosition);
            if (currDist < dist)
            {
                closest = node;
                dist = currDist;
            }

            node.selectionSprite.SetActive(false);
        }

        if (dist < MAX_HOVER_HIGHLIGHT_DIST)
        {
            closest.selectionSprite.SetActive(true);
        }
    }
    private void UpdateBuildRoadConnectTool()
    {
        //TODO 

        print("Not Implemented");

    }
    private void UpdateBuildRoadCreateIntersectionTool()
    {
        //rotate cursor
        buildToolCursor.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), Time.deltaTime * 360.0f);
        //position cursor
        Vector3 cursorPosition = self.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0.0f;
        buildToolCursor.transform.position = cursorPosition;

        //hilight nearby road nodes

    }
    private void UpdateActiveTool()
    {
        switch (activeTool)
        {
            case ActiveTool.Select:
                UpdateSelectTool();
                break;
            case ActiveTool.BuildRoadConnect:
                UpdateBuildRoadConnectTool();
                break;
            case ActiveTool.BuildRoadCreateIntersection:
                UpdateBuildRoadCreateIntersectionTool();
                break;
            default:
                break;
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

        UpdateActiveTool();

        prevMousePos = Input.mousePosition;
	}
}
