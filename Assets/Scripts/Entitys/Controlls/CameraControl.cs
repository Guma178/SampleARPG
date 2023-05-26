using SARPG.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SARP.Entitys
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField]
        Joystick movementStick;

        [SerializeField]
        private Transform targetTransform;

        [SerializeField]
        private float roatationSpeed = 2f, minDistance, maxDistance;

        private float currentDistance;

        private Vector3 fromTargetDirection;

        private Transform cameraTransorm;
        private Transform CameraTransform
        {
            get
            {
                if (cameraTransorm == null)
                {
                    cameraTransorm = this.transform;
                }

                return cameraTransorm;
            }
        }

        private Vector3 TargetPosition
        {
            get
            {
                return targetTransform.position;
            }
        }

        public void Rotate(Vector2 direction)
        {
            Quaternion rot;
            float angle;

            //CameraTransform.position = TargetPosition + fromTargetDirection * currentDistance;

            angle = direction.x * roatationSpeed * Time.deltaTime;
            rot = Quaternion.AngleAxis(angle, Vector3.up);
            CameraTransform.position = rot * (CameraTransform.position - TargetPosition) + TargetPosition;
            CameraTransform.rotation = rot * CameraTransform.rotation;

            fromTargetDirection = (CameraTransform.position - TargetPosition).normalized;
        }

        public void Zoom(Vector2 direction)
        {
            currentDistance += (direction.y * Time.deltaTime * roatationSpeed);
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        private void Fix()
        {
            CameraTransform.position = TargetPosition + fromTargetDirection * currentDistance;
        }


        void Start()
        {
            currentDistance = (CameraTransform.position - TargetPosition).magnitude;
            fromTargetDirection = (CameraTransform.position - TargetPosition).normalized;
        }

        void LateUpdate()
        {
            Fix();

#if UNITY_EDITOR || UNITY_STANDALONE
            if (UnityEngine.Input.GetMouseButton(1))
            {
                Rotate(new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")));
            }
            Zoom(UnityEngine.Input.mouseScrollDelta);
#endif

#if UNITY_EDITOR || UNITY_ANDROID
            Rotate(movementStick.Value);
            Zoom(movementStick.Value);
#endif
        }
    }
}
