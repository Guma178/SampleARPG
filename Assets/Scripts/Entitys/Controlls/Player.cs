using SARPG.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SARP.Entitys
{
    public class Player : Character
    {
        [SerializeField]
        float searchRadius;

        [SerializeField]
        Button selectNextButton, attackButton;

        [SerializeField]
        Joystick movementStick;

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

        private SARPG.UI.Radar radar;
        private Character selected;
        private ProcessState acting;
        private bool assaulting = false, inMove = false;
        private List<Character> inView = new List<Character>();
        int selectionpointer = 0;


        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Victim.Health > 0 & false)
            {
                if (UnityEngine.Input.GetMouseButton(0))
                {
                    Floor floor;
                    Character character;
                    RaycastHit hit;
                    Ray ray = MainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        floor = hit.collider.gameObject.GetComponent<Floor>();
                        if (floor != null)
                        {
                            acting = new ProcessState();
                            acting.Finished += delegate () { Walker.EndWalk(); };
                            Walker.Walk(hit.point, acting);
                            Walker.BeginWalk();
                        }
                        else
                        {
                            character = hit.collider.gameObject.GetComponent<Character>();
                            if (character != null && character != this && character.Victim.Health > 0)
                            {
                                if (radar != null)
                                {
                                    radar.Turn(false);
                                }
                                radar = hit.collider.gameObject.GetComponent<Radar>();
                                if (radar != null)
                                {
                                    radar.Turn(true);
                                    radar.Draw(Vector3.ProjectOnPlane(character.Size, character.ThisTransorm.up).magnitude);
                                }
                                selected = character;
                                character.Victim.Died += delegate () { if (selected.gameObject == radar.gameObject) { radar.Turn(false); } };


                                if ((character.ThisTransorm.position - ThisTransorm.position).magnitude > Assaulter.AttackRange)
                                {
                                    acting = new ProcessState();
                                    acting.Finished += delegate () { Walker.EndWalk(); };
                                    Walker.Walk(character.ThisTransorm, acting, Vector3.ProjectOnPlane(character.Size, character.ThisTransorm.up).magnitude / 2);
                                    Walker.BeginWalk();
                                }
                                else
                                {
                                    if (!assaulting)
                                    {
                                        acting = new ProcessState();
                                        assaulting = true;
                                        acting.Finished += delegate () { assaulting = false; };
                                        Walker.EndWalk();
                                        Walker.Walk(character.ThisTransorm.position, new ProcessState());
                                        Assaulter.Assault(character.Victim, acting);
                                    }
                                }
                            }
                        }
                    }
                }
            }
#endif
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR || UNITY_ANDROID
            Collider[] inRange;
            Character found;
            List<Character> current;

            current = new List<Character>();
            inRange = Physics.OverlapSphere(ThisTransorm.position + ThisTransorm.forward * searchRadius, searchRadius, ~LayerMask.GetMask("Ignore Raycast"));
            foreach (Collider col in inRange)
            {
                found = col.gameObject.GetComponent<Character>();
                if (found != null && col.gameObject != this.gameObject && found.Victim.Health > 0)
                {
                    current.Add(found);
                }
            }

            if (inView.Count == 0 || inView.Any(iv => current.Any(cr => cr != iv))) 
            { 
                inView = current; 
                if(inView.Count == 1)
                {
                    SelectNext();
                }
            }
#endif
        }

        private new void Start()
        {
            base.Start();
            movementStick.Begin += MoveStart;
            movementStick.End += MoveEnd;
            selectNextButton.onClick.AddListener(SelectNext);
            attackButton.onClick.AddListener(AttackSelected);
        }

        private void MoveStart()
        {
            Walker.BeginWalk();
            inMove = true;
            StartCoroutine(Moving());
        }
        private void MoveEnd()
        {
            Walker.EndWalk();
            inMove = false;
        }

        private IEnumerator Moving()
        {
            while (inMove)
            {
                Walker.Walk(new Vector3(movementStick.Value.x, 0, movementStick.Value.y));
                yield return new WaitForEndOfFrame();
            }
        }

        private void SelectNext()
        {
            if (inView.Any())
            {
                if (selectionpointer >= inView.Count)
                {
                    selectionpointer = 0;
                }
                if (radar != null)
                {
                    radar.Turn(false);
                }
                radar = inView[selectionpointer].gameObject.GetComponent<Radar>();
                if (radar != null)
                {
                    radar.Turn(true);
                    radar.Draw(Vector3.ProjectOnPlane(inView[selectionpointer].Size, inView[selectionpointer].ThisTransorm.up).magnitude);
                }
                inView[selectionpointer].Victim.Died += delegate () { if (selected.gameObject == radar.gameObject) { radar.Turn(false); } };
                selected = inView[selectionpointer];
                selectionpointer++;
            }
        }

        private void AttackSelected()
        {
            if (selected != null && selected.Victim.Health > 0 && (selected.ThisTransorm.position - ThisTransorm.position).magnitude <= Assaulter.AttackRange)
            {
                if (!assaulting)
                {
                    acting = new ProcessState();
                    assaulting = true;
                    acting.Finished += delegate () { assaulting = false; };
                    Walker.EndWalk();
                    Walker.Walk(selected.ThisTransorm.position, new ProcessState());
                    Assaulter.Assault(selected.Victim, acting);
                }
            }
        }
    }
}
