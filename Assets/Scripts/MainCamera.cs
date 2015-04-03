using UnityEngine;
using System.Collections;

enum ActiveTool
{
    None,
    BuildRoad
}

public class MainCamera : MonoBehaviour {

    private const float CAMERA_MOVE_SPEED = 5.0f;

    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);
    private ActiveTool activeTool = ActiveTool.None;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;

        this.prevMousePos = Input.mousePosition;
	}
	
    private void DeselectCurrentTool()
    {
        switch (activeTool)
        {
            case ActiveTool.None:
                return;
            case ActiveTool.BuildRoad:
                break;
            default:
                break;
        }
    }

    public void OnBuildRoadClick()
    {
        //cancel what we were previously doing
        DeselectCurrentTool();


    }

	// Update is called once per frame
	void Update () {
        Camera self = GetComponent<Camera>();

        if (Input.GetMouseButton(0))
        {
            //if left mouse down
        }

        transform.position += new Vector3(0.0f, Input.GetAxis("Vertical") * CAMERA_MOVE_SPEED * Time.deltaTime, 0.0f);
        transform.position += new Vector3(Input.GetAxis("Horizontal") * CAMERA_MOVE_SPEED * Time.deltaTime, 0.0f, 0.0f);

        if (Input.GetMouseButton(1))
        {
            //if user is pressing right mouse button
            transform.position -= self.ScreenToWorldPoint(Input.mousePosition) - self.ScreenToWorldPoint(prevMousePos);
        }

        self.orthographicSize *= 1.0f + (0.25f * Input.mouseScrollDelta.y);


        prevMousePos = Input.mousePosition;
	}
}
