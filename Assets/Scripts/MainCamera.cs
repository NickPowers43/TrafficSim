using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);

	// Use this for initialization
	void Start () {
        this.prevMousePos = Input.mousePosition;
	}
	
    public void OnBuildRoadClick()
    {

    }

	// Update is called once per frame
	void Update () {
        Camera self = GetComponent<Camera>();

        if (Input.GetMouseButton(0))
        {
            //if left mouse down
        }

        if (Input.GetMouseButton(1))
        {
            //if user is pressing right mouse button
            transform.position -= self.ScreenToWorldPoint(Input.mousePosition) - self.ScreenToWorldPoint(prevMousePos);
        }

        self.orthographicSize *= 1.0f + (0.25f * Input.mouseScrollDelta.y);


        prevMousePos = Input.mousePosition;
	}
}
