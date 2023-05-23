using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARP.Entitys
{
    public class Input : MonoBehaviour
    {
        private System.Tuple<bool, Camera> mainCamera = System.Tuple.Create<bool, Camera>(false, null);
        public Camera MainCamera
        {
            get
            {
                if (!mainCamera.Item1)
                {
                    mainCamera = System.Tuple.Create<bool, Camera>(true, Camera.main);
                }

                return mainCamera.Item2;
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
