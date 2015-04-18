using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public class Tool
    {
        public const float CURSOR_Z = -0.25f;

        protected Intersection GetHoveredIntersection()
        {
            return Intersection.ClosestToPositionAndInRadius(CursorPos(Intersection.Z_POSITION));
        }
        protected void ToggleHighlight(Intersection intersection)
        {
            intersection.selectionSprite.SetActive(!intersection.selectionSprite.activeSelf);
        }
        protected Vector3 CursorPos(float z)
        {
            Vector3 cursorPos = MainCamera.Instance.Self.ScreenToWorldPoint(Input.mousePosition);
            cursorPos.z = z;
            return cursorPos;
        }



        public virtual void Deactivate()
        {

        }
        public virtual void Activate()
        {

        }
        public virtual void Update()
        {

        }
        public virtual void OnClick()
        {

        }
    } 
}
