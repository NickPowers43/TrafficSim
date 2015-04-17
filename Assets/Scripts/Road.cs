using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road : MonoBehaviour {

    private static LinkedList<Road> roads = new LinkedList<Road>();
    public static LinkedList<Road> Roads
    {
        get
        {
            return roads;
        }
    }

    public static Road ClosestToCursor(ref float dist)
    {
        Road closest = null;
        //dist = float.PositiveInfinity;
        //foreach (Road road in roads)
        //{
        //    float currDist = Vector3.Magnitude(MainCamera.Instance.Self.WorldToScreenPoint(node.transform.position) - Input.mousePosition);
        //    if (currDist < dist)
        //    {
        //        closest = node;
        //        dist = currDist;
        //    }

        //    node.selectionSprite.SetActive(false);
        //}

        return closest;
    }
    public static bool CheckOverlap(Vector3 position, float r)
    {
        //foreach (Intersection node in intersections)
        //{
        //    float currDist = Vector3.Magnitude(node.transform.position - position);
        //    if (currDist - r - node.Radius < 0.0f)
        //    {
        //        return true;
        //    }
        //}

        return false;
    }

    private IntersectionInlet source;
    public IntersectionInlet Source
    {
        get
        {
            return source;
        }
        //set
        //{
        //    start = value;
        //}
    }

    private IntersectionInlet destination;
    public IntersectionInlet Destination
    {
        get
        {
            return destination;
        }
        //set
        //{
        //    end = value;
        //}
    }

    public void SetEndPoints(IntersectionInlet source, IntersectionInlet destination)
    {
        this.source = source;
        this.destination = destination;
    }

	void Start ()
    {
        this.source = null;
        this.destination = null;

        roads.AddLast(this);
	}
	
	void Update () 
    {
        if (source != null && destination != null)
        {
            Debug.DrawLine(source.Parent.transform.position, destination.Parent.transform.position, new Color(0, 0, 0));
        }
	}
}
