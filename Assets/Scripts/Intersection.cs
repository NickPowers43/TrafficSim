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

    public const float Z_POSITION = 0.0f;
    private const int MAX_THREAD_STACK_SIZE = 2 << 10; // 2KB
    public const float INITIAL_RADIUS = 0.09f;
    private TrafficLight tlight = new TrafficLight();

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

    private static void UpdateRoadNetwork()
    {

    }

    public GameObject selectionSprite = null;

    private IntersectionInlet[] inlets = new IntersectionInlet[4];

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
        while (running)
        {
            Vehicle v = new Vehicle();

            //iterate street light state
            tlight.OperateTrafficLightsatIntersection(this.index);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < inlets[i].LaneQueues.Length; j++)
			    {
                    LaneQueue source = inlets[i].LaneQueues[j];
                    Vehicle turningVehicle = null;

                    lock (source)
                    {
                        turningVehicle = source.DeQueue();
                    }

                    //determine what to do with first vehicle
                    if (turningVehicle.Destination == source.Index)
                    {
                        //TODO: take vehicle out of simulation
                    }
                    else
                    {
                        //route vehicle to its destination
                        LaneQueue destination = Navigator.Instance.GetNextHop(0, source.Index, turningVehicle.Destination);

                        lock (destination)
                        {
                            destination.Queue(turningVehicle);
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

    public void createPointofInterest(int destination)
    {
        Vehicle v = new Vehicle();
        if (v.Destination == index)
        {
            vehicles.Add(v);
        }
        else
        {
            vehicles.Remove(v);
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
}
