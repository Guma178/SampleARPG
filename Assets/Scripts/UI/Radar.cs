using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARPG.UI
{
    public class Radar : MonoBehaviour
    {
        public float ThetaScale = 0.01f;
        private int size;
        private LineRenderer lineDrawer;
        private float theta = 0f;

        private LineRenderer LineDrawer
        {
            get
            {
                if (lineDrawer == null)
                {
                    lineDrawer = GetComponent<LineRenderer>();
                }

                return lineDrawer;
            }
        }

        public void Draw(float radius)
        {
            theta = 0f;
            size = (int)((1f / ThetaScale * radius) + 1f);
            LineDrawer.positionCount = size;
            for (int i = 0; i < size; i++)
            {
                theta += (2.0f * Mathf.PI * ThetaScale);

                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);
                LineDrawer.SetPosition(i, new Vector3(x, 0, y));
            }
        }
    }
}