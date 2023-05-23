using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TCF.Entitys
{
    public class Mover : MonoBehaviour
    {
        [SerializeField]
        float rotationSpeed, moveSpeed;

        private bool movable = true;

        public float RotationSpeed => rotationSpeed;
        public float MoveSpeed => moveSpeed;    
        public bool Movable
        {
            get { return movable; }

            private set { movable = value; }
        }

        private Transform thisTransorm;
        private Transform ThisTransorm
        {
            get
            {
                if (thisTransorm == null)
                {
                    thisTransorm = this.transform;
                }

                return thisTransorm;
            }
        }

        private System.Tuple<bool, Renderer> bodyrenderer = System.Tuple.Create<bool, Renderer>(false, null);
        private Renderer Renderer
        {
            get
            {
                if (!bodyrenderer.Item1)
                {
                    bodyrenderer = System.Tuple.Create<bool, Renderer>(true, this.GetComponent<Renderer>());
                }

                return bodyrenderer.Item2;
            }
        }
        private System.Tuple<bool, CharacterController> characterController = System.Tuple.Create<bool, CharacterController>(false, null);
        private CharacterController CharacterController
        {
            get
            {
                if (!characterController.Item1)
                {
                    characterController = System.Tuple.Create<bool, CharacterController>(true, this.GetComponent<CharacterController>());
                }

                return characterController.Item2;
            }
        }

        private System.Tuple<bool, Rigidbody> movablebody = System.Tuple.Create<bool, Rigidbody>(false, null);
        private Rigidbody Movablebody
        {
            get
            {
                if (!movablebody.Item1)
                {
                    movablebody = System.Tuple.Create<bool, Rigidbody>(true, this.GetComponent<Rigidbody>());
                }

                return movablebody.Item2;
            }
        }

        public void MoveTo(Vector3 target, ProcessState processState)
        {
            if (Movable)
            {
                StartCoroutine(MovingTo(target, processState));
            }
        }
        public void MoveTo(Transform target, ProcessState processState)
        {
            if (Movable)
            {
                StartCoroutine(MovingTo(target, processState));
            }
        }
        public void Move(Vector3 direction)
        {
            if (Movable)
            {
                Toward(direction);
            }
        }
        public void SetPosition(Vector3 position, Quaternion rotation)
        {
            if (Movablebody != null)
            {
                Movablebody.MovePosition(position);
                Movablebody.rotation = rotation;
            }
            else
            {
                ThisTransorm.position = position;
                ThisTransorm.rotation = rotation;
            }
        }

        private void Toward(Vector3 direction)
        {
            float targetAngle, acceleration;
            Quaternion rot = Quaternion.LookRotation(direction.normalized, ThisTransorm.up);
            targetAngle = Vector3.Angle(ThisTransorm.forward, direction);
            acceleration = ((180f - targetAngle) / 180f);
            Vector3 translation = ThisTransorm.forward * acceleration * MoveSpeed * Time.deltaTime;
            if (CharacterController != null)
            {
                ThisTransorm.rotation = Quaternion.Lerp(ThisTransorm.rotation, rot, Time.deltaTime * RotationSpeed);
                CharacterController.Move(translation);
            }
            else if (Movablebody != null)
            {
                if (Movablebody.isKinematic)
                {
                    Movablebody.rotation = Quaternion.Lerp(ThisTransorm.rotation, rot, Time.deltaTime * RotationSpeed);
                    Movablebody.MovePosition(Movablebody.position + translation);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                ThisTransorm.rotation = Quaternion.Lerp(ThisTransorm.rotation, rot, Time.deltaTime * RotationSpeed);
                ThisTransorm.Translate(translation, Space.World);
            }
        }
            
        private IEnumerator MovingTo(Transform target, ProcessState processState, float distance = 0, bool targetRotation = false)
        {
            Vector3 toTarget;

            processState.Finished += delegate () { Movable = true;  };
            Movable = false;
            toTarget = (target.position - ThisTransorm.position);
            while (toTarget.magnitude > (distance) + MoveSpeed * Time.deltaTime * 2)
            {
                toTarget = (target.position - ThisTransorm.position);
                Toward(toTarget.normalized);
                yield return new WaitForEndOfFrame();
            }
            if (targetRotation)
            {
                float angle = Quaternion.Angle(ThisTransorm.rotation, target.rotation);
                while (angle > 3)
                {
                    ThisTransorm.rotation = Quaternion.Lerp(ThisTransorm.rotation, target.rotation, Time.deltaTime * RotationSpeed);
                    yield return new WaitForEndOfFrame();
                    angle = Quaternion.Angle(ThisTransorm.rotation, target.rotation);
                }
                SetPosition(target.position, target.rotation);
            }
            else
            {
                SetPosition(target.position, ThisTransorm.rotation);
            }
            Movable = true;
            processState?.Complet();
        }
        private IEnumerator MovingTo(Vector3 target, ProcessState processState)
        {
            Vector3 toTarget;

            processState.Finished += delegate () { Movable = true; };
            Movable = false;
            toTarget = (target - ThisTransorm.position);
            while (toTarget.magnitude > MoveSpeed * Time.deltaTime * 2)
            {
                toTarget = (target - ThisTransorm.position);
                Toward(toTarget);
                yield return new WaitForEndOfFrame();
            }
            SetPosition(target, ThisTransorm.rotation);
            Movable = true;
            processState?.Complet();
        }
    }
}
