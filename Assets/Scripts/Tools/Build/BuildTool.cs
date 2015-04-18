using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Build
{
    public class BuildTool : Tool
    {
        protected static GameObject roadPrefab;
        public static GameObject RoadPrefab
        {
            get
            {
                return roadPrefab;
            }
            set
            {
                roadPrefab = value;
            }
        }

        protected static GameObject intersectionPrefab;
        public static GameObject IntersectionPrefab
        {
            get
            {
                return intersectionPrefab;
            }
            set
            {
                intersectionPrefab = value;
            }
        }

        protected static GameObject cursor;
        public static GameObject Cursor
        {
            get
            {
                return cursor;
            }
            set
            {
                cursor = value;
                cursor.SetActive(false);
            }
        }

        public BuildTool()
        {

        }

        public override void Activate()
        {
            cursor.SetActive(true);
        }
        public override void Deactivate()
        {
            cursor.SetActive(false);
        }
        public override void Update()
        {
            //rotate cursor
            cursor.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), Time.deltaTime * 360.0f);
            //position cursor
            cursor.transform.position = CursorPos(CURSOR_Z);

        }
        public override void OnClick()
        {
            GameObject nodeGO = (GameObject)GameObject.Instantiate(intersectionPrefab);
            nodeGO.transform.position = CursorPos(Intersection.Z_POSITION);

            //UpdateRoadNetwork();
        }
    }
}
