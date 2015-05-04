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

    private static GameObject prefab;
    private static List<int>[] poiDestinations = new List<int>[5];
    public static List<int>[] POIDestinations
    {
        get
        {
            return poiDestinations;
        }
    }

    //stores the unity prefab for intersection GameObjects
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

        switch (poi)
        {
            case PointsOfInterest.House:
                //pick random destination in the appropriate poiDestination list
                v = new Vehicle(Simulation.GetTime(), 0.05f, poiDestinations[(int)PointsOfInterest.Work][0]);
                break;
            case PointsOfInterest.Food:
                break;
            case PointsOfInterest.Fuel:
                break;
            case PointsOfInterest.Work:
                break;
            case PointsOfInterest.None:
                break;
            default:
                break;
        }

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

        //add incoming LaneQueues to destination indices lists
        if (poi != PointsOfInterest.None)
        {
            for (int i = 0; i < 4; i++)
            {
                if (inlets[i] != null)
                {
                    foreach (LaneQueue lq in inlets[i].LaneQueues)
                    {
                        //add this LaneQueue's index to the appropriate point of interest destination index list
                        lq.IsDestination = true;
                        poiDestinations[(int)poi].Add(lq.Index);
                    }
                }
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
}
