using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

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

public class Intersection : MonoBehaviour {

    private const double TIME_PER_INLET = 15.0;
    private const double CHECK_DESTINATION_RATE = 1.0;
    private const double CHECK_NEXT_VEHICLE_RATE = 5.0;

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
    //the thread simulating this object
    private Thread thread;
    public Thread Thread
    {
        get
        {
            return thread;
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
    private PointsOfInterest poi;
    public PointsOfInterest POI
    {
        get
        {
            return poi;
        }
        set
        {
            poi = value;
        }
    }
    //destination intersections
    private Dictionary<Intersection, float> destinationTravelTimeAvg = new Dictionary<Intersection,float>();
    //path lines
    private List<GameObject> pathLines = new List<GameObject>();

    private double mean;
    private double stddev;


    public const float Z_POSITION = 0.0f;
    private const int MAX_THREAD_STACK_SIZE = 2 << 10; // 2KB
    public const float INITIAL_RADIUS = 0.09f;
    //private TrafficLight tlight = new TrafficLight();

    private static List<Intersection> intersections = new List<Intersection>();
    public static List<Intersection> Intersections
    {
        get
        {
            return intersections;
        }
    }

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

    //overriden MonoBehaviour functions
	void Start()
    {
        
	}
	void Update()
    {
	    
	}

    public void Run()
    {
        thread = new Thread(new ThreadStart(RunMethod), MAX_THREAD_STACK_SIZE);
        thread.Start();
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

                    while (remainingLightTime != 0.0)
                    {
                        //handle any point of interest duties
                        HandlePOI();

                        LaneQueue source = inlets[i].LaneQueues[0];
                        Vehicle turningVehicle = null;

                        //peek at vehicle coming into intersection
                        lock (source)
                        {
                            if (source.Count > 0)
                            {
                                //if the car has been in the queue long enough to drive to the end
                                double currentTime = Simulation.GetTime();
                                double timeToDrive = currentTime - source.Peek().TimeQueued;
                                double timeToDriveLeft = source.DistanceToTime(source.MaxLength) - timeToDrive;
                                if (timeToDrive > source.DistanceToTime(source.MaxLength))
                                {
                                    //take waiting vehicle
                                    turningVehicle = source.Dequeue();
                                }
                                else if (timeToDriveLeft < remainingLightTime)
                                {
                                    //take waiting vehicle
                                    turningVehicle = source.Dequeue();
                                    //sleep until vehicle arrives at the light
                                    Simulation.SleepSimSeconds(timeToDriveLeft);
                                    remainingLightTime -= timeToDriveLeft;
                                }
                                else
                                {
                                    //sleep for the rest of the light time
                                    Simulation.SleepSimSeconds(remainingLightTime);
                                    remainingLightTime = 0.0;
                                }
                            }
                        }

                        if (turningVehicle != null)
                        {
                            //determine what to do with the vehicle
                            if (turningVehicle.Destination == source.Index)
                            {
                                //TODO: take vehicle out of simulation
                            }
                            else
                            {
                                //route vehicle to its destination

                                //sleep until vehicle passes light
                                Simulation.SleepSimSeconds(source.DistanceToTime(turningVehicle.Length));
                                remainingLightTime -= source.DistanceToTime(turningVehicle.Length);

                                //get the destination LaneQueue to enqueue the vehicle
                                LaneQueue destination = Navigator.Instance.GetNextHop(0, source.Index, turningVehicle.Destination);

                                while (true)
                                {
                                    if (destination.Available(turningVehicle.Length))
                                        break;

                                    //wait for chance and hold up traffic
                                    Simulation.SleepSimSeconds(source.DistanceToTime(CHECK_DESTINATION_RATE));
                                    remainingLightTime -= CHECK_DESTINATION_RATE;
                                }

                                //transfer the vehicle to the next intersection
                                lock (destination)
                                {
                                    //set TimeBehind property to represent time this vehicle is behind the one in front
                                    destination.Enqueue(turningVehicle);
                                }
                            }
                        }
                        else
                        {
                            Simulation.SleepSimSeconds(source.DistanceToTime(CHECK_NEXT_VEHICLE_RATE));
                            remainingLightTime -= CHECK_NEXT_VEHICLE_RATE;
                        }



                        if (remainingLightTime < 0.0)
                            remainingLightTime = 0.0;
                    }
                }
            }
        }
    }

    private void HandlePOI()
    {
        //spawn vehicles that have an appropriate destination

        Vehicle v = null;

        v = new Vehicle(Simulation.GetTime(), 0.05f, poiDestinations[(int)PointsOfInterest.Work][0].DestinationIndex);

        if (v != null)
        {
            inlets[0].LaneQueues[0].Enqueue(v);
        }
    }

    public void MakePOI(PointsOfInterest poi)
    {
        this.poi = poi;

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
    public void ConnectLaneQueues(List<Utility.WeightedEdge<LaneQueue>> edges)
    {
        if (poi != PointsOfInterest.None)
        {
            poiDestinations[(int)poi].Add(this);
        }

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
        //add everything
        foreach (var destinationList in poiDestinations)
        {
            AddDestinations(destinationList);
        }

        //switch (poi)
        //{
        //    case PointsOfInterest.House:
        //        AddDestinations(poiDestinations[(int)PointsOfInterest.Work]);
        //        AddDestinations(poiDestinations[(int)PointsOfInterest.Food]);
        //        AddDestinations(poiDestinations[(int)PointsOfInterest.Fuel]);
        //        break;
        //    case PointsOfInterest.Food:
        //        break;
        //    case PointsOfInterest.Fuel:
        //        break;
        //    case PointsOfInterest.Work:
        //        break;
        //    default:
        //        break;
        //}
    }
    private void AddDestinations(List<Intersection> destinations)
    {
        foreach (var destination in destinations)
        {
            destinationTravelTimeAvg[destination] = float.PositiveInfinity;
        }
    }
    public void Clear()
    {
        destinationTravelTimeAvg.Clear();
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
        foreach (var key in destinationTravelTimeAvg.Keys)
        {
            Vector3 labelPosition = (key.transform.position + transform.position) * 0.5f;

            GameObject pathLine = (GameObject)GameObject.Instantiate(pathLineprefab, labelPosition, Quaternion.identity);

            pathLine.transform.localScale = new Vector3(Vector3.Magnitude(key.transform.position - transform.position), Intersection.INITIAL_RADIUS * 0.2f, 1.0f);
            float rotation = Mathf.Atan2((key.transform.position - transform.position).y, (key.transform.position - transform.position).x) * Mathf.Rad2Deg;
            pathLine.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotation);
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
