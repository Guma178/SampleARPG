using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARP.Entitys
{
    public class Input : MonoBehaviour
    {
        [SerializeField]
        Character controlled;

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

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Floor floor;
                RaycastHit hit;
                Ray ray = MainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    floor = hit.collider.gameObject.GetComponent<Floor>();
                    if (floor != null)
                    {
                        controlled.Walker.Walk(hit.point, new ProcessState());
                    }
                }
            }
        }
    }
}
