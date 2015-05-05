using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road : MonoBehaviour {

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

    private static List<Road> roads = new List<Road>();
    public static List<Road> Roads
    {
        get
        {
            return roads;
        }
    }

    public static Road ClosestToCursor(ref float dist, Vector3 cursorPosition)
    {
        Road closest = null;
        dist = float.PositiveInfinity;
        foreach (Road road in roads)
        {
            IntersectionInlet sourceInlet = road.Source;
            IntersectionInlet destinationInlet = road.Destination;
            Intersection sourceParent = sourceInlet.Parent;
            Intersection destinationParent = destinationInlet.Parent;

            Vector3 diff = destinationParent.transform.position - sourceParent.transform.position;
            float diff_length = diff.magnitude;
            diff.Normalize();

            Vector3 loc_diff = cursorPosition - sourceParent.transform.position;

            float dot = Vector3.Dot(diff, loc_diff);

            Vector3 intersection_pos = sourceParent.transform.position + (diff * dot);

            float currDist = Vector3.Magnitude(loc_diff - (diff * dot));
            if (currDist < dist && dot > 0.0f && dot < diff_length)
            {
                closest = road;
                dist = currDist;
            }
        }

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

    public GameObject tarmacGO;

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

        source.OutgoingInlet = destination;
        destination.OutgoingInlet = source;

        Vector3 pathVector = source.Parent.transform.position - destination.Parent.transform.position;

        float length = Vector3.Magnitude(pathVector);
        for (int i = 0; i < source.LaneQueues.Length; i++)
        {
            source.LaneQueues[i].MaxLength = length;
        }
        for (int i = 0; i < destination.LaneQueues.Length; i++)
        {
            destination.LaneQueues[i].MaxLength = length;
        }

        this.transform.position = source.Parent.transform.position;
        float rotation = Mathf.Atan2(pathVector.y, pathVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotation);

        tarmacGO.transform.localPosition = new Vector3(-0.5f * length, 0.0f, 0.0f);
        tarmacGO.transform.localScale = new Vector3(length, Intersection.INITIAL_RADIUS * 2.0f, 1.0f);
    }

    public void Disconnect()
    {
        source.Parent.RemoveInlet(source.InletIndex);
        destination.Parent.RemoveInlet(destination.InletIndex);
    }

    public Intersection SplitWithDegenerate(Vector3 position)
    {
        Vector3 diff = destination.Parent.transform.position - source.Parent.transform.position;
        diff.Normalize();

        Vector3 loc_diff = position - source.Parent.transform.position;

        float dot = Vector3.Dot(diff, loc_diff);

        Vector3 intersection_pos = source.Parent.transform.position + (diff * dot);


        Intersection output = Intersection.CreateIntersection(new Vector3(0.0f, 0.0f, 0.0f), PointsOfInterest.None);
        output.transform.position = intersection_pos;

        Road newRoad = GameObject.Instantiate(Road.Prefab).GetComponent<Road>();
        Road.Roads.Add(newRoad);

        Intersection a = source.Parent;
        Intersection b = destination.Parent;

        Disconnect();

        SetEndPoints(a.GenerateInlet(), output.GenerateInlet());
        newRoad.SetEndPoints(output.GenerateInlet(), b.GenerateInlet());

        return output;
    }
}
