using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using SimpleJson;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Threading;

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
    public GameObject pathLinePrefab;
    //
    public InputField intersectionName;
    public InputField speedInputField;
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

        this.prevMousePos = Input.mousePosition;

        //Initialize UI objects
        Tools.Build.BuildTool.Cursor = GameObject.Instantiate(buildToolCursorPrefab);
        Intersection.Prefab = intersectionPrefab;
        Intersection.PathLineprefab = pathLinePrefab;
        Road.Prefab = roadPrefab;
        //initialize tools
        activeTools = null;
        buildTools = new Tools.Build.ToolBar();
        selectTools = new Tools.Select.ToolBar();


        Instance = this;
        self = GetComponent<Camera>();

        Simulation.SpeedMultiplier = 3.0;

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

        Thread.Sleep(30);

        //store current mouse position
        prevMousePos = Input.mousePosition;
    }
    //Load street network information from "temp.save"
    private void LoadSaveFile()
    {
        string json = File.ReadAllText("save.json");

        JsonObject mainObj = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(json);

        JsonObject intersections = (JsonObject)mainObj["intersections"];

        JsonArray roads = (JsonArray)mainObj["roads"];

        //add intersections
        foreach (string intersectionObjName in intersections.Keys)
        {
            JsonObject intersection = (JsonObject)intersections[intersectionObjName];

            Vector2 pos = new Vector2();
            pos.x = float.Parse((string)intersection["posX"]);
            pos.y = float.Parse((string)intersection["posY"]);
            PointsOfInterest poi = (PointsOfInterest)int.Parse((string)intersection["poi"]);

            intersectionName.text = (string)intersection["name"];
            Intersection intersectionGO = Intersection.CreateIntersection(new Vector3(pos.x, pos.y, Intersection.Z_POSITION), poi);
        }

        //add roads
        foreach (JsonObject road in roads)
        {
            int sourceIndex = int.Parse((string)road["source"]);
            int destinationIndex = int.Parse((string)road["destination"]);

            Road roadGO = GameObject.Instantiate(Road.Prefab).GetComponent<Road>();
            roadGO.SetEndPoints(Intersection.Intersections[sourceIndex].GenerateInlet(), Intersection.Intersections[destinationIndex].GenerateInlet());
            Road.Roads.Add(roadGO.GetComponent<Road>());
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

        //clear intersections
        //foreach (var intersection in Intersection.Intersections) { intersection.Clear(); }
        //clear point of interest lists
        foreach (var list in Intersection.POIDestinations) { list.Clear(); }
        //add intersections to point of interest lists
        foreach (var intersection in Intersection.Intersections) { intersection.AddPointOfInterest(); }
        //get destinations
        foreach (var intersection in Intersection.Intersections) { intersection.GetDestinations(); }

        //create a new single instance of the Navigator
        Navigator.Instance = new Navigator();

        Simulation.Running = true;

        //spin up active objects
        foreach (Intersection i in Intersection.Intersections) { i.Run(); }

        //show stop simulation button
        stopSimulationButton.SetActive(true);
    }
    public void OnStopClick()
    {
        stopSimulationButton.SetActive(false);
        Simulation.Running = false;

        foreach (var intersection in Intersection.Intersections)
        {
            if (intersection.Thread != null)
            {
                intersection.Thread.Abort();
                intersection.Thread = null;
            }
            if (intersection.VehicleInstantiationThread != null)
            {
                intersection.VehicleInstantiationThread.Abort();
                intersection.VehicleInstantiationThread = null;
            }
        }
    }
    public void OnSpeedChanged()
    {
        Simulation.SpeedMultiplier = float.Parse(speedInputField.text);
    }
    //Save street network information into "temp.save"
    public void OnSaveClick()
    {
        //add intersections
        JsonObject intersections = new JsonObject();
        int i = 0;
        foreach (var intersection in Intersection.Intersections)
        {
            JsonObject temp = new JsonObject();

            intersection.Index = i++;

            temp["posX"] = intersection.transform.position.x.ToString();
            temp["posY"] = intersection.transform.position.y.ToString();
            temp["poi"] = ((int)intersection.POI).ToString();
            temp["index"] = intersection.Index.ToString();
            temp["name"] = intersection.Name;

            if (intersection.POI != PointsOfInterest.None)
            {
                JsonObject pathTimes = new JsonObject();

                foreach (var pathData in intersection.PathData)
                {
                    if (pathData.destinationIntersection.Name != "")
                    {
                        string val;
                        if (pathData.travels > 0)
                        {
                            val = (pathData.travelTime / (float)pathData.travels).ToString();
                        }
                        else
                        {
                            val = "no travels";
                        }
                        pathTimes[pathData.destinationIntersection.Name + "_AvgTime"] = val;
                    }
                }

                temp["pathTimes"] = pathTimes;
            }

            intersections[i.ToString() + "_" + intersection.Name] = temp;
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

        File.WriteAllText("save.json", output);
    }
}
