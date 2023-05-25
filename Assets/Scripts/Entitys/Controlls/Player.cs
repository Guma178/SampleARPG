using SARPG.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SARP.Entitys
{
    public class Player : Character
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

        private SARPG.UI.Radar radar;
        private ProcessState acting;
        private bool assaulting = false;

        private new void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            if (Victim.Health > 0)
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
                                character.Victim.Died += delegate () { radar.Turn(false); };


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
        }
    }
}
