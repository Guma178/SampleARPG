using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

namespace SARP.Entitys
{
    public class Solder : IntelligenceCharacter
    {
        [SerializeField]
        Transform dst;

        private void FixedUpdate()
        {
            Decision();
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("KeyCode.Space");
                Walker.Walk(dst.position, new ProcessState());
            }
        }

        private void Decision()
        {
            NavMeshPath path;
            ProcessState process;
            IEnumerable<Character> charactersInView;

            if (Victim.Health > 0)
            {
                charactersInView = ObjectsInView<Character>();
                foreach (Character ch in charactersInView)
                {
                    if (Faction.Enemys.Any(f => f == ch.Faction))
                    {
                        if ((ch.ThisTransorm.position - ThisTransorm.position).magnitude > Assaulter.AttackRange)
                        {
                            if (ch.Victim.Health > 0)
                            {
                                path = new NavMeshPath();
                                if (NavMesh.CalculatePath(ThisTransorm.position, ch.ThisTransorm.position, NavMesh.AllAreas, path))
                                {
                                    process = new ProcessState();
                                    if (path.corners.Length >= 2)
                                    {
                                        Walker.Walk(path.corners[1], process, Vector3.ProjectOnPlane(ch.Size, ch.ThisTransorm.up).magnitude / 2);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ch.Victim.Health > 0)
                            {
                                process = new ProcessState();
                                Walker.Walk(ch.ThisTransorm.position, process, Vector3.ProjectOnPlane(ch.Size, ch.ThisTransorm.up).magnitude / 2);
                                Assaulter.Assault(ch.Victim);
                            }
                        }
                    }
                }
            }
        }
    }
}
