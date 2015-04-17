using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

public class Intersection : MonoBehaviour {

    private const int MAX_THREAD_STACK_SIZE = 1024;
    public const float INITIAL_RADIUS = 0.09f;

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
                inlets[i] = new IntersectionInlet(this);
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
    public bool Running
    {
        get
        {
            return running;
        }
        set
        {
            running = value;
        }
    }

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
        while (true)
        {
            //check incoming car queues

            //iterate street light state


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
}
