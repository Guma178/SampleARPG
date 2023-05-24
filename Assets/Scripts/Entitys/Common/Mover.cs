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


        public void MoveTo(Vector3 target, ProcessState processState, bool align = false, bool inPlane = false)
        {
            if (movingProcess != null && movingState != null)
            {
                StopCoroutine(movingProcess);
                movingState.Interrupt();
            }
            movingProcess = StartCoroutine(MovingTo(target, processState, align, inPlane));
            movingState = processState;
        }
        public void MoveTo(Transform target, ProcessState processState, bool align = false, bool inheritRotation = false, bool inPlane = false)
        {
            if (movingProcess != null && movingState != null)
            {
                StopCoroutine(movingProcess);
                movingState.Interrupt();
            }
            movingProcess = StartCoroutine(MovingTo(target, processState, align, inheritRotation, inPlane));
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
            else if (CharacterController != null)
            {
                CharacterController.Move(position - ThisTransorm.position);
                ThisTransorm.rotation = rotation;
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
            Vector3 translation = ThisTransorm.forward * MoveSpeed * Time.deltaTime;
            if (CharacterController != null)
            {
                ThisTransorm.rotation = Quaternion.Lerp(ThisTransorm.rotation, rot, Time.deltaTime * RotationSpeed * MoveSpeed);
                CharacterController.Move(translation);
            }
            else if (Movablebody != null)
            {
                if (Movablebody.isKinematic)
                {
                    Movablebody.rotation = Quaternion.Lerp(ThisTransorm.rotation, rot, Time.deltaTime * RotationSpeed * MoveSpeed);
                    Movablebody.MovePosition(Movablebody.position + translation);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                ThisTransorm.rotation = Quaternion.Lerp(ThisTransorm.rotation, rot, Time.deltaTime * RotationSpeed * MoveSpeed);
                ThisTransorm.Translate(translation, Space.World);
            }
        }

        private IEnumerator MovingTo(Transform target, ProcessState processState, bool align = false, bool inheritRotation = false, bool inPlane = false)
        {
            Vector3 toTarget;

            toTarget = inPlane ? Vector3.ProjectOnPlane((target.position - ThisTransorm.position), ThisTransorm.up) : (target.position - ThisTransorm.position);
            while (toTarget.magnitude > Vector3.ProjectOnPlane(Renderer.bounds.size, ThisTransorm.up).magnitude * 0.2f + MoveSpeed * Time.deltaTime * 2)
            {
                toTarget = inPlane ? Vector3.ProjectOnPlane((target.position - ThisTransorm.position), ThisTransorm.up) : (target.position - ThisTransorm.position);
                Toward(toTarget);
                yield return new WaitForEndOfFrame();
            }
            if (inheritRotation)
            {
                float angle = Quaternion.Angle(ThisTransorm.rotation, target.rotation);
                while (angle > 3)
                {
                    ThisTransorm.rotation = Quaternion.Lerp(ThisTransorm.rotation, target.rotation, Time.deltaTime * RotationSpeed);
                    yield return new WaitForEndOfFrame();
                    angle = Quaternion.Angle(ThisTransorm.rotation, target.rotation);
                }
            }
            if (align)
            {
                toTarget = inPlane ? Vector3.ProjectOnPlane((target.position - ThisTransorm.position), ThisTransorm.up) : (target.position - ThisTransorm.position);
                if (inheritRotation)
                {
                    SetPosition(ThisTransorm.position + toTarget, target.rotation);
                }
                else
                {
                    SetPosition(ThisTransorm.position + toTarget, ThisTransorm.rotation);
                }
            }
            processState?.Complet();
            movingProcess = null;
            movingState = null;
        }
        private IEnumerator MovingTo(Vector3 target, ProcessState processState, bool align = false, bool inPlane = false)
        {
            Vector3 toTarget;

            toTarget = inPlane ? Vector3.ProjectOnPlane((target - ThisTransorm.position), ThisTransorm.up) : (target - ThisTransorm.position);
            while (toTarget.magnitude > Vector3.ProjectOnPlane(Renderer.bounds.size, ThisTransorm.up).magnitude * 0.2f + MoveSpeed * Time.deltaTime * 2)
            {
                toTarget = inPlane ? Vector3.ProjectOnPlane((target - ThisTransorm.position), ThisTransorm.up) : (target - ThisTransorm.position);
                Toward(toTarget);
                yield return new WaitForEndOfFrame();
            }
            if (align)
            {
                toTarget = inPlane ? Vector3.ProjectOnPlane((target - ThisTransorm.position), ThisTransorm.up) : (target - ThisTransorm.position);
                SetPosition(ThisTransorm.position + toTarget, ThisTransorm.rotation);
            }
            processState?.Complet();
            movingProcess = null;
            movingState = null;
        }
    }
}
