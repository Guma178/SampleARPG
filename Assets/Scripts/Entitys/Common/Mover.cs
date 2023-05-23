using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SARP.Entitys
{
    public class Mover : MonoBehaviour
    {
        [SerializeField]
        float rotationSpeed, moveSpeed;

        [SerializeField]
        Renderer bodyrenderer;

        public float RotationSpeed => rotationSpeed;
        public float MoveSpeed => moveSpeed;

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

        private Renderer Renderer
        {
            get
            {
                return bodyrenderer;
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

        private Coroutine movingProcess;
        private ProcessState movingState;


        public void MoveTo(Vector3 target, ProcessState processState, bool inPlane = false)
        {
            if (movingProcess != null && movingState != null)
            {
                StopCoroutine(movingProcess);
                movingState.Interrupt();
            }
            movingProcess = StartCoroutine(MovingTo(target, processState, inPlane));
            movingState = processState;
        }
        public void MoveTo(Transform target, ProcessState processState, bool targetRotation = false, bool inPlane = false)
        {
            if (movingProcess != null && movingState != null)
            {
                StopCoroutine(movingProcess);
                movingState.Interrupt();
            }
            movingProcess = StartCoroutine(MovingTo(target, processState, 0, targetRotation, inPlane));
            movingState = processState;
        }
        public void Move(Vector3 direction)
        {
            if (movingProcess != null && movingState != null)
            {
                StopCoroutine(movingProcess);
                movingState.Interrupt();
            }
            Toward(direction);
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

        private IEnumerator MovingTo(Transform target, ProcessState processState, float distance = 0, bool targetRotation = false, bool inPlane = false)
        {
            Vector3 toTarget;

            toTarget = (target.position - ThisTransorm.position);
            while ((inPlane ? Vector3.ProjectOnPlane(toTarget, ThisTransorm.up).magnitude : toTarget.magnitude) > (distance) + Vector3.ProjectOnPlane(Renderer.bounds.size, ThisTransorm.up).magnitude / 2 + MoveSpeed * Time.deltaTime * 2)
            {
                toTarget = (target.position - ThisTransorm.position);
                Toward(inPlane ? Vector3.ProjectOnPlane(toTarget, ThisTransorm.up) : toTarget);
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
            processState?.Complet();
            movingProcess = null;
            movingState = null;
        }
        private IEnumerator MovingTo(Vector3 target, ProcessState processState, bool inPlane = false)
        {
            Vector3 toTarget;

            toTarget = (target - ThisTransorm.position);
            while ((inPlane ? Vector3.ProjectOnPlane(toTarget, ThisTransorm.up).magnitude : toTarget.magnitude) > Vector3.ProjectOnPlane(Renderer.bounds.size, ThisTransorm.up).magnitude / 2 + MoveSpeed * Time.deltaTime * 2)
            {
                toTarget = (target - ThisTransorm.position);
                Toward(inPlane ? Vector3.ProjectOnPlane(toTarget, ThisTransorm.up) : toTarget);
                yield return new WaitForEndOfFrame();
            }
            SetPosition(target, ThisTransorm.rotation);
            processState?.Complet();
            movingProcess = null;
            movingState = null;
        }
    }
}
