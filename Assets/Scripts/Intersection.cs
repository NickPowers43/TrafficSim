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
    Services = 3,
    Work = 4,
    None = 5
}

public class Intersection : MonoBehaviour {

    private const double TIME_PER_INLET = 15.0f;

    private static GameObject prefab;
    private static List<PointsOfInterest> listofplaces = new List<PointsOfInterest>();

    public static List<PointsOfInterest> Listofplaces
    {
        get
        {
            return listofplaces;
        }
    }
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

    public Sprite[] POISprites;
    public GameObject iconGO;

    private PointsOfInterest poi;

    private double mean;
    private double stddev;

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

    public const float Z_POSITION = 0.0f;
    private const int MAX_THREAD_STACK_SIZE = 2 << 10; // 2KB
    public const float INITIAL_RADIUS = 0.09f;
    //private TrafficLight tlight = new TrafficLight();

    private static LinkedList<Intersection> intersections = new LinkedList<Intersection>();
    public static LinkedList<Intersection> Intersections
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

    private Intersection[] adjIntersections;
    public Intersection[] AdjIntersections
    {
        get
        {
            return adjIntersections;
        }
        set
        {
            adjIntersections = value;
        }
    }

    private bool running;

    private Thread thread;
    public Thread Thread
    {
        get
        {
            return thread;
        }
    }

    public float Radius
    {
        get
        {
            return INITIAL_RADIUS;
        }
    }

	// Use this for initialization
	void Start()
    {
        Intersections.AddLast(this);
	}
	
	// Update is called once per frame
	void Update()
    {
	    
	}

    public void Run()
    {
        thread = new Thread(new ThreadStart(RunMethod), MAX_THREAD_STACK_SIZE);
        running = true;
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
                                    turningVehicle = source.Dequeue();
                                }
                                else if (timeToDriveLeft < remainingLightTime)
                                {
                                    //sleep until vehicle arrives at the light
                                    Simulation.SleepSimSeconds(timeToDriveLeft);
                                    remainingLightTime -= timeToDriveLeft;
                                    turningVehicle = source.Dequeue();
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

                                //sleep for some amount of time to simulate time
                                Simulation.SleepSimSeconds(1.0);

                                //get the destination LaneQueue to deposit the vehicle
                                LaneQueue destination = Navigator.Instance.GetNextHop(0, source.Index, turningVehicle.Destination);

                                //transfer the vehicle to the next intersection
                                lock (destination)
                                {
                                    //set TimeBehind property to represent time this vehicle is behind the one in front
                                    turningVehicle.TimeQueued = Simulation.GetTime();
                                    destination.Enqueue(turningVehicle);
                                }
                            }

                            //sleep for the time it takes for the next vehicle to arrive at the light
                        }
                        else
                        {
                            //TODO: sleep for the rest of the light
                        }
                    }
                }
            }
        }
    }
    public void Stop()
    {
        if (running)
        {
            running = false;
            thread.Join(Timeout.Infinite);
        }
    }

    private void HandlePOI()
    {
        //spawn vehicles that have an appropriate destination

        switch (poi)
        {
            case PointsOfInterest.House:
                break;
            case PointsOfInterest.Food:
                break;
            case PointsOfInterest.Fuel:
                break;
            case PointsOfInterest.Services:
                break;
            case PointsOfInterest.Work:
                break;
            case PointsOfInterest.None:
                break;
            default:
                break;
        }
    }

    public void SortAdjIntersectionsByAngle()
    {
        //float[] angles = new float[adjIntersections.Length];
        //for (int i = 0; i < adjIntersections.Length; i++)
        //{
        //    Vector3 temp = adjIntersections[i].transform.position - transform.position;
        //    angles[i] = Mathf.Atan2(temp.y, temp.x) * Mathf.Rad2Deg;
        //    if (true)
        //    {
		 
        //    }
        //}

        //for (int i = 0; i < adjIntersections.Length; i++)
        //{
        //    int smallest = i;
        //    for (int j = i; j < adjIntersections.Length - 1; j++)
        //    {
        //        if (angles[i] < )
        //        {
                    
        //        }
        //    }
        //}
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

    public void ConnectInlets()
    {
        //TODO: void Intersection.ConnectInlets()
        if (inlets[0] != null)
            inlets[0].ConnectLaneQueues(inlets[3], inlets[1], inlets[2]);
        if (inlets[1] != null)
            inlets[1].ConnectLaneQueues(inlets[2], inlets[0], inlets[3]);
        if (inlets[2] != null)
            inlets[2].ConnectLaneQueues(inlets[0], inlets[3], inlets[1]);
        if (inlets[3] != null)
            inlets[3].ConnectLaneQueues(inlets[1], inlets[2], inlets[0]);
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
}
