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
        Coroutine acting;

        private void Start()
        {
            acting = StartCoroutine(Decisioning());
            Victim.Died += delegate () { StopCoroutine(acting); };
        }

        private IEnumerator Decisioning()
        {
            const float pathRecalcRelation = 0.25f; 

            Character enemy;
            NavMeshPath path;
            ProcessState process;
            Vector3 targetPathPosition, toDestination;
            int stepInd = 0;

            while (true)
            {
                enemy = ObjectsInView<Character>().FirstOrDefault(ch => ch.Victim.Health > 0 && Faction.Enemys.Any(ef => ef == ch.Faction));

                if (enemy != null)
                {
                    targetPathPosition = enemy.ThisTransorm.position;
                    path = new NavMeshPath();
                    stepInd = 0;
                    if (NavMesh.CalculatePath(ThisTransorm.position, enemy.ThisTransorm.position, NavMesh.AllAreas, path))
                    {
                        Walker.BeginWalk();
                        while ((enemy.ThisTransorm.position - ThisTransorm.position).magnitude > Assaulter.AttackRange
                            && enemy.Victim.Health > 0)
                        {
                            if ((targetPathPosition - enemy.ThisTransorm.position).magnitude /
                                (enemy.ThisTransorm.position - ThisTransorm.position).magnitude > 
                                pathRecalcRelation)
                            {
                                path = new NavMeshPath();
                                stepInd = 0;
                                targetPathPosition = enemy.ThisTransorm.position;
                                if (!NavMesh.CalculatePath(ThisTransorm.position, enemy.ThisTransorm.position, NavMesh.AllAreas, path))
                                {
                                    break;
                                }
                            }

                            if (stepInd < path.corners.Length)
                            {
                                toDestination = path.corners[stepInd] - ThisTransorm.position;
                                Walker.Walk(toDestination);
                                yield return new WaitForEndOfFrame();
                                if (toDestination.magnitude < Vector3.ProjectOnPlane(Size, ThisTransorm.up).magnitude / 2)
                                {
                                    stepInd++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        Walker.EndWalk();

                        if (enemy.Victim.Health > 0)
                        {
                            process = new ProcessState();
                            Walker.Walk(enemy.ThisTransorm.position, process, Vector3.ProjectOnPlane(enemy.Size, enemy.ThisTransorm.up).magnitude / 2);
                            Assaulter.Assault(enemy.Victim);
                        }
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }
    }
}
