using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARPG.UI
{
    public class UIBillboard : MonoBehaviour
    {
        private Transform cameraTransform, thisTransform;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
            thisTransform = this.transform;
        }

        private void LateUpdate()
        {
            thisTransform.LookAt(thisTransform.position + cameraTransform.forward);
        }
    }
}
