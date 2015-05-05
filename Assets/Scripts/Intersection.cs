using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using NumberGenerator;
using UnityEngine.UI;

public enum PointsOfInterest
{
    House = 0,
    Food = 1,
    Fuel = 2,
    Work = 4,
    None = 5
}

[Serializable]
public class IntersectionData
{
    public PointsOfInterest poi;
    public Vector2 position;

    public string ToString()
    {
        return "{position: " + position.ToString() + ", poi: " + poi.ToString() + "}";
    }
}

public struct PathData
{
    public Intersection destinationIntersection;
    public double travelTime;
    public int travels;
}

public class Intersection : MonoBehaviour {

    private object dLock = new object();

    private const double TIME_PER_INLET = 5.0;
    private const double CHECK_DESTINATION_INTERVAL = 0.05;
    private const double VEHICLE_INSTANTIATION_INTERVAL = 0.5;
    private const double POLLING_SLEEP_INTERVAL = 0.2;
    private const double TURNING_TIME = 0.08;

    public const float PATHLINE_Z_POSITION = 0.25f;
    public const float Z_POSITION = 0.5f;
    private const int MAX_THREAD_STACK_SIZE = 1 << 17; // 128KB
    public const float INITIAL_RADIUS = 0.09f;

    //random number generator
    SimpleRNG rng;

    private static List<Intersection> intersections = new List<Intersection>();
    public static List<Intersection> Intersections
    {
        get
        {
            return intersections;
        }
    }

    //lists of every type of point of interest
    private static List<Intersection>[] poiDestinations = new List<Intersection>[5] {
        new List<Intersection>(),
        new List<Intersection>(),
        new List<Intersection>(),
        new List<Intersection>(),
        new List<Intersection>()
    };
    public static List<Intersection>[] POIDestinations
    {
        get
        {
            return poiDestinations;
        }
    }

    public static Intersection CreateIntersection(Vector3 position, PointsOfInterest poi)
    {
        GameObject nodeGO = (GameObject)GameObject.Instantiate(Intersection.Prefab);
        nodeGO.transform.position = position;

        Intersection temp = nodeGO.GetComponent<Intersection>();

        Intersection.Intersections.Add(temp);

        temp.POI = poi;
        temp.Name = MainCamera.Instance.intersectionName.text;
        MainCamera.Instance.intersectionName.text = "";

        //Debug.Log("Intersection with name \"" + temp.Name + "\" created.");

        return temp;
    }

    //stores the unity prefab for intersection GameObjects
    private static GameObject prefab;
    public static GameObject Prefab
    {
        get
        {
            return prefab;
        }
        set
        {
            prefab = value;
        }
    }
    //stores the unity prefab for displaying the paths to destination nodes
    private static GameObject pathLineprefab;
    public static GameObject PathLineprefab
    {
        get
        {
            return pathLineprefab;
        }
        set
        {
            pathLineprefab = value;
        }
    }

    public Sprite[] POISprites;
    public GameObject iconGO;
    //index to identify this intersection with
    private int index;
    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }
    //number of road edges connecting to this intersection
    public int Degree
    {
        get
        {
            int j = 0;
            for (int i = 0; i < 4; i++)
            {
                if (inlets[i] != null)
                {
                    j++;
                }
            }
            return j;
        }
    }
    //the index of the LaneQueue that is this point of interest's destination
    public int DestinationIndex
    {
        get
        {
            return TryGetInlet().LaneQueues[0].Index;
        }
    }
    //name of this intersection
    private string name;
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }
    //the thread simulating this object
    private Thread thread = null;
    public Thread Thread
    {
        get
        {
            return thread;
        }
        set
        {
            thread = value;
        }
    }
    //the thread instantiating vehicles
    private Thread vehicleInstantiationThread = null;
    public Thread VehicleInstantiationThread
    {
        get
        {
            return vehicleInstantiationThread;
        }
        set
        {
            vehicleInstantiationThread = value;
        }
    }
    //the radius of the intersection body
    public float Radius
    {
        get
        {
            return INITIAL_RADIUS;
        }
    }
    //the type of intersection
    private PointsOfInterest poi = PointsOfInterest.None;
    public PointsOfInterest POI
    {
        get
        {
            return poi;
        }
        set
        {
            poi = value;

            //set aesthetic properties
            if (poi == PointsOfInterest.None)
            {
                iconGO.SetActive(false);
            }

            else
            {
                iconGO.SetActive(true);
                iconGO.GetComponent<SpriteRenderer>().sprite = POISprites[(int)poi];
            }
        }
    }
    //destination intersections
    private List<PathData> pathData = new List<PathData>();
    public List<PathData> PathData
    {
        get
        {
            return pathData;
        }
        set
        {
            pathData = value;
        }
    }
    //path lines
    private List<GameObject> pathLines = new List<GameObject>();

    public static Intersection ClosestToCursor(ref float dist)
    {
        Intersection closest = null;
        dist = float.PositiveInfinity;
        foreach (Intersection node in intersections)
        {
            float currDist = Vector3.Magnitude(MainCamera.Instance.Self.WorldToScreenPoint(node.transform.position) - Input.mousePosition);
            if (currDist < dist)
            {
                closest = node;
                dist = currDist;
            }
        }

        return closest;
    }
    public static Intersection ClosestToPosition(Vector3 position, ref float dist)
    {
        Intersection closest = null;
        dist = float.PositiveInfinity;
        foreach (Intersection node in intersections)
        {
            float currDist = Vector3.Magnitude(node.transform.position - position);
            if (currDist < dist)
            {
                closest = node;
                dist = currDist;
            }
        }

        return closest;
    }
    public static Intersection ClosestToPositionAndInRadius(Vector3 position)
    {
        Intersection closest = null;
        float dist = float.PositiveInfinity;
        foreach (Intersection node in intersections)
        {
            float currDist = Vector3.Magnitude(node.transform.position - position);
            if (currDist < dist && currDist < node.Radius)
            {
                closest = node;
                dist = currDist;
            }
        }

        return closest;
    }
    public static bool CheckOverlap(Vector3 position, float r)
    {
        foreach (Intersection node in intersections)
        {
            float currDist = Vector3.Magnitude(node.transform.position - position);
            if (currDist - r - node.Radius < 0.0f)
            {
                return true;
            }
        }

        return false;
    }

    public GameObject selectionSprite = null;

    private IntersectionInlet[] inlets = new IntersectionInlet[4];

    private List<Vehicle> vehicles = new List<Vehicle>();

    public void Run()
    {
        thread = new Thread(new ThreadStart(RunMethod), MAX_THREAD_STACK_SIZE);
        thread.Priority = System.Threading.ThreadPriority.Normal;
        thread.Start();

        if (poi != PointsOfInterest.None)
        {
            vehicleInstantiationThread = new Thread(new ThreadStart(InstantiateVehicles), MAX_THREAD_STACK_SIZE);
            vehicleInstantiationThread.Priority = System.Threading.ThreadPriority.Normal;
            vehicleInstantiationThread.Start();
        }
    }
    private void RunMethod()
    {
        while (Simulation.Running)
        {
            for (int i = 0; i < 4; i++)
            {
                if (inlets[i] != null)
                {
                    double remainingLightTime = TIME_PER_INLET;

                    LaneQueue source = inlets[i].LaneQueues[0];

                    while (remainingLightTime != 0.0)
                    {
                        Vehicle turningVehicle = null;
                        double sleepTime = 0.0;

                        //peek at vehicle coming into intersection
                        lock (source)
                        {
                            if (source.Count > 0)
                            {
                                double currentTime = Simulation.GetTime();
                                double timeDriving = currentTime - source.Peek().TimeQueued;
                                double timeToDriveLeft = source.DistanceToTime(source.MaxLength) - timeDriving;
                                if (timeDriving > source.DistanceToTime(source.MaxLength))
                                {
                                    //if the car has been in the queue long enough to drive to the end
                                    //take waiting vehicle
                                    turningVehicle = source.Dequeue();
                                    sleepTime = TURNING_TIME;
                                }
                                else if (timeToDriveLeft < remainingLightTime)
                                {
                                    //take waiting vehicle
                                    turningVehicle = source.Dequeue();
                                    //sleep until vehicle arrives at the light
                                    sleepTime = timeToDriveLeft + TURNING_TIME;
                                }
                                else
                                {
                                    //sleep for the rest of the light time
                                    sleepTime = remainingLightTime;
                                }
                            }
                            else
                            {
                                sleepTime = POLLING_SLEEP_INTERVAL;
                            }
                        }

                        if (sleepTime != 0.0)
                        {
                            Simulation.SleepSimSeconds(sleepTime);
                            remainingLightTime -= sleepTime;
                        }

                        if (turningVehicle != null)
                        {
                            //determine what to do with the vehicle
                            if (turningVehicle.Destination == source.Index)
                            {
                                //take vehicle out of simulation
                                turningVehicle.DestroyTime = Simulation.GetTime();

                                for (int j = 0; j < turningVehicle.StartIntersection.PathData.Count; j++)
                                {
                                    if (turningVehicle.StartIntersection.PathData[j].destinationIntersection == this)
                                    {
                                        lock (turningVehicle.StartIntersection.PathData)
                                        {
                                            PathData temp = turningVehicle.StartIntersection.PathData[j];
                                            temp.travels++;
                                            temp.travelTime += turningVehicle.Age;
                                            turningVehicle.StartIntersection.PathData[j] = temp; 
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //route vehicle to its destination

                                //get the destination LaneQueue to enqueue the vehicle
                                LaneQueue destination = Navigator.Instance.GetTransition(source.Index, turningVehicle.Destination);

                                while (true)
                                {
                                    lock (destination)
                                    {
                                        if (destination.Available(turningVehicle.Length))
                                        {
                                            //transfer the vehicle to the next intersection
                                            destination.Enqueue(turningVehicle);

                                            break;
                                        }
                                    }

                                    //wait for a chance and hold up traffic
                                    Simulation.SleepSimSeconds(CHECK_DESTINATION_INTERVAL);
                                    remainingLightTime -= CHECK_DESTINATION_INTERVAL;
                                }
                            }
                        }

                        if (remainingLightTime < 0.0)
                            remainingLightTime = 0.0;
                    }
                }
            }
        }
    }
    private void InstantiateVehicles()
    {
        IntersectionInlet inlet = TryGetInlet();

        if (inlet != null)
        {
            while (Simulation.Running)
            {
                int r = (int)(SimpleRNG.GetUint() >> 1) % pathData.Count;

                Vehicle v = new Vehicle(
                    Simulation.GetTime(),
                    0.05,
                    pathData[r].destinationIntersection.DestinationIndex);

                v.StartIntersection = this;

                while (true)
                {
                    lock (inlet.OutgoingInlet.LaneQueues[0])
                    {
                        if (inlet.OutgoingInlet.LaneQueues[0].Available(v.Length))
                        {
                            inlet.OutgoingInlet.LaneQueues[0].Enqueue(v);
                            break;
                        }
                    }

                    Simulation.SleepSimSeconds(CHECK_DESTINATION_INTERVAL);
                }

                Simulation.SleepSimSeconds(VEHICLE_INSTANTIATION_INTERVAL);
            }
        }
        
    }

    public void AddPointOfInterest()
    {
        if (poi != PointsOfInterest.None)
        {
            poiDestinations[(int)poi].Add(this);
        }
    }
    public void IndexLaneQueues(ref int nextIndex, List<int> destinationIndices)
    {
        for (int i = 0; i < 4; i++)
        {
            if (inlets[i] != null)
            {
                foreach (var lq in inlets[i].LaneQueues)
                {
                    if (lq.IsDestination)
                    {
                        destinationIndices.Add(nextIndex);
                    }
                    lq.Index = nextIndex++;
                }
            }
        }
    }
    public void ConnectLaneQueues(List<Utility.WeightedEdge<LaneQueue>>[] edges)
    {
        //TODO: void Intersection.ConnectInlets()
        if (inlets[0] != null)
            inlets[0].ConnectLaneQueues(inlets[3], inlets[1], inlets[2], edges);
        if (inlets[1] != null)
            inlets[1].ConnectLaneQueues(inlets[2], inlets[0], inlets[3], edges);
        if (inlets[2] != null)
            inlets[2].ConnectLaneQueues(inlets[0], inlets[3], inlets[1], edges);
        if (inlets[3] != null)
            inlets[3].ConnectLaneQueues(inlets[1], inlets[2], inlets[0], edges);
    }
    public void GetDestinations()
    {
        pathData.Clear();

        foreach (var destinationList in poiDestinations)
        {
            AddDestinations(destinationList);
        }
    }
    private void AddDestinations(List<Intersection> destinations)
    {
        foreach (var destination in destinations)
        {
            if (destination != this)
            {
                PathData tempPathData;
                tempPathData.destinationIntersection = destination;
                tempPathData.travels = 0;
                tempPathData.travelTime = 0.0;
                pathData.Add(tempPathData);
            }
        }
    }

    public void RemoveInlet(int index)
    {
        //TODO: void Intersection.RemoveInlet(int index)
        inlets[index] = null;
    }

    public IntersectionInlet GenerateInlet()
    {
        for (int i = 0; i < 4; i++)
        {
            if (inlets[i] == null)
            {
                inlets[i] = new IntersectionInlet(this, i);
                return inlets[i];
            }
        }

        throw new NotImplementedException();
    }

    public IntersectionData GetData()
    {
        IntersectionData temp = new IntersectionData();
        temp.poi = this.poi;
        temp.position = new Vector2(this.transform.position.x, this.transform.position.y);

        return temp;
    }

    public IntersectionInlet TryGetInlet()
    {
        for (int i = 0; i < 4; i++)
        {
            if (inlets[i] != null)
            {
                return inlets[i];
            }
        }
        return null;
    }

    public void ShowPathLines()
    {
        foreach (var record in pathData)
        {

            Vector3 labelPosition = (record.destinationIntersection.transform.position + transform.position) * 0.5f;
            labelPosition.z = PATHLINE_Z_POSITION;

            GameObject pathLine = (GameObject)GameObject.Instantiate(pathLineprefab, labelPosition, Quaternion.identity);

            pathLine.transform.localScale = new Vector3(Vector3.Magnitude(record.destinationIntersection.transform.position - transform.position), Intersection.INITIAL_RADIUS * 0.2f, 1.0f);
            float rotation = Mathf.Atan2((record.destinationIntersection.transform.position - transform.position).y, (record.destinationIntersection.transform.position - transform.position).x) * Mathf.Rad2Deg;
            pathLine.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotation);

            pathLines.Add(pathLine);
        }
    }
    public void HidePathLines()
    {
        foreach (var path in pathLines)
        {
            GameObject.Destroy(path);
        }

        pathLines.Clear();
    }
}
