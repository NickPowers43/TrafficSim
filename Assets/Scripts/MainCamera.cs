using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using SimpleJson;
using System.IO;
using System.Text;

public class MainCamera : MonoBehaviour {

    //Fields:

    //Camera fields
    private const float CAMERA_MOVE_SPEED = 5.0f;
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
    private Camera self;
    public Camera Self
    {
        get
        {
            return self;
        }
    }
    //Unity prefab objects
    public GameObject buildToolCursorPrefab;
    public GameObject intersectionPrefab;
    public GameObject roadPrefab;
    public GameObject stopSimulationButton;
    //UI object references and variables
    public float intersectionSelectRange = 0.01f;
    public float uiTopBoundary;
    private Vector3 prevMousePos = new Vector3(0.0f, 0.0f, 0.0f);
    private Tools.Build.ToolBar buildTools;
    private Tools.Select.ToolBar selectTools;
    private ToolArray activeTools;
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
    
    //Functions:

    //On the start of this Unity component
	void Start(){

        LaneQueue.NextIndex = 0;

        //Initialize UI objects
        this.prevMousePos = Input.mousePosition;
        Tools.Build.BuildTool.Cursor = GameObject.Instantiate(buildToolCursorPrefab);
        Intersection.Prefab = intersectionPrefab;
        Road.Prefab = roadPrefab;
        activeTools = null;
        buildTools = new Tools.Build.ToolBar();
        selectTools = new Tools.Select.ToolBar();


        Instance = this;
        self = GetComponent<Camera>();

        LoadSaveFile();
	}
    //On the update of this Unity component
    void Update()
    {
        //move camera when arrow keys or WASD keys are pressed
        transform.position += new Vector3(0.0f, Input.GetAxis("Vertical") * CAMERA_MOVE_SPEED * Time.deltaTime, 0.0f);
        transform.position += new Vector3(Input.GetAxis("Horizontal") * CAMERA_MOVE_SPEED * Time.deltaTime, 0.0f, 0.0f);

        //if user is pressing right mouse button
        if (Input.GetMouseButton(1))
        {
            //move camera the amount the mouse has moved
            transform.position -= self.ScreenToWorldPoint(Input.mousePosition) - self.ScreenToWorldPoint(prevMousePos);
        }

        //zoom camera
        self.orthographicSize *= 1.0f + (0.25f * Input.mouseScrollDelta.y);

        //update active tool
        if (activeTools != null)
        {
            activeTools.Update();
        }

        //store current mouse position
        prevMousePos = Input.mousePosition;
    }
    //Load street network information from "temp.save"
    private void LoadSaveFile()
    {
        string json = File.ReadAllText("temp.save");

        JsonObject mainObj = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(json);

        JsonArray intersections = (JsonArray)mainObj["intersections"];

        JsonArray roads = (JsonArray)mainObj["roads"];

        //add intersections
        foreach (JsonObject intersection in intersections)
        {
            Vector2 pos = new Vector2();
            pos.x = float.Parse((string)intersection["posX"]);
            pos.y = float.Parse((string)intersection["posY"]);

            Intersection intersectionGO = ((GameObject)GameObject.Instantiate(Intersection.Prefab, new Vector3(pos.x, pos.y, Intersection.Z_POSITION), Quaternion.identity)).GetComponent<Intersection>();

            Intersection.Intersections.Add(intersectionGO);
        }

        //add roads
        foreach (JsonObject road in roads)
        {
            int sourceIndex = int.Parse((string)road["source"]);
            int destinationIndex = int.Parse((string)road["destination"]);

            Road roadGO = GameObject.Instantiate(Road.Prefab).GetComponent<Road>();

            roadGO.SetEndPoints(Intersection.Intersections[sourceIndex].GenerateInlet(), Intersection.Intersections[destinationIndex].GenerateInlet());
        }
    }
    //when the user presses the giant button covering the background of the UI
    public void OnWorldSpaceClick()
    {
        if (activeTools != null)
        {
            activeTools.OnClick();
        }
    }
    //button click handlers
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
    public void OnStartClick()
    {
        if (Simulation.Running)
        {
            OnStopClick();
        }

        Navigator navigator = new Navigator();
        Navigator.Instance = navigator;

        //TODO: start simulation
        stopSimulationButton.SetActive(true);
        Simulation.Running = true;

        //spin up active objects
        foreach (Intersection i in Intersection.Intersections)
        {
            i.Run();
        }
    }
    public void OnStopClick()
    {
        stopSimulationButton.SetActive(false);
        Simulation.Running = false;
    }
    //Save street network information into "temp.save"
    public void OnSaveClick()
    {
        //add intersections
        JsonArray intersections = new JsonArray(Intersection.Intersections.Count);
        int i = 0;
        foreach (var intersection in Intersection.Intersections)
        {
            JsonObject temp = new JsonObject();

            intersection.Index = i++;

            temp["posX"] = intersection.transform.position.x.ToString();
            temp["posY"] = intersection.transform.position.y.ToString();
            temp["poi"] = intersection.POI.ToString();
            temp["index"] = intersection.Index.ToString();

            intersections.Add(temp);
        }

        //add roads
        JsonArray roads = new JsonArray(Road.Roads.Count);
        i = 0;
        foreach (Road road in Road.Roads)
        {
            JsonObject temp = new JsonObject();

            temp["source"] = road.Source.Parent.Index.ToString();
            temp["destination"] = road.Destination.Parent.Index.ToString();

            roads.Add(temp);
        }

        //create final object
        JsonObject mainObj = new JsonObject();
        mainObj["intersections"] = intersections;
        mainObj["roads"] = roads;

        string output = SimpleJson.SimpleJson.SerializeObject(mainObj);
        //Debug.Log(temp);

        File.WriteAllText("temp.save", output);
    }
}
