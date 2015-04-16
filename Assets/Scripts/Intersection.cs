using UnityEngine;
using System.Collections;
using System.Threading;

public class Intersection : MonoBehaviour {

    private const int MAX_THREAD_STACK_SIZE = 1024;

    public GameObject selectionSprite = null;

    private IntersectionInlet[] inlets = new IntersectionInlet[4];

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

    private Thread thread;

	// Use this for initialization
	void Start()
    {
        thread = new Thread(new ThreadStart(Run), MAX_THREAD_STACK_SIZE);
        thread.Start();
	}
	
	// Update is called once per frame
	void Update()
    {
	    
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

    void Run()
    {
        while (true)
        {
            //check incoming car queues

            //iterate street light state


        }
    }
}
