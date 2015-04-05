using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum ActiveTool
{
    Select,
    BuildRoad
}

public struct RoadNode
{
    public GameObject go;

}

public class MainCamera : MonoBehaviour {

    private const float CAMERA_MOVE_SPEED = 5.0f;

    public float roadNodeSelectRange = 0.01f;
    public float uiTopBoundary;
    public GameObject buildToolCursorPrefab;

    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);
    private ActiveTool activeTool = ActiveTool.Select;
    private GameObject buildToolCursor;
    private Camera self;

    private LinkedList<RoadNode> roadNodes;
    //build tool
    LinkedListNode<RoadNode> currentSelectedRoadNode;



	// Use this for initialization
	void Start () {
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
                return;
            case ActiveTool.BuildRoad:
                buildToolCursor.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void OnBuildRoadClick()
    {
        //cancel what we were previously doing
        DeselectCurrentTool();

        //initialize build road tool
        buildToolCursor.SetActive(true);

        activeTool = ActiveTool.BuildRoad;
    }

    public void OnSelectClick()
    {
        //cancel what we were previously doing
        DeselectCurrentTool();

        activeTool = ActiveTool.Select;
    }

    private void OnSelectMouseDown()
    {
        //select anything within range
    }

    private void OnBuildRoadMouseDown()
    {

    }

    private void OnWorldSpaceMouseDown()
    {
        switch (activeTool)
        {
            case ActiveTool.Select:
                OnSelectMouseDown();
                break;
            case ActiveTool.BuildRoad:
                OnBuildRoadMouseDown();
                break;
            default:
                break;
        }
    }

    private void UpdateSelectTool()
    {
        //hilight nearby selectable objects
    }

    private void UpdateBuildRoadTool()
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
            case ActiveTool.BuildRoad:
                UpdateBuildRoadTool();
                break;
            default:
                break;
        }
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0) && Input.mousePosition.y > uiTopBoundary)
        {
            //if left mouse down
            OnWorldSpaceMouseDown();
        }

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
