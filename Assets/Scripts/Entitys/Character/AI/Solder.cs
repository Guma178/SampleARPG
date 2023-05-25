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
        private void FixedUpdate()
        {
            Decision();
        }

        private void Decision()
        {
            NavMeshPath path;
            IEnumerable<Character> charactersInView;

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
                                if (path.corners.Length > 2)
                                {
                                    Walker.Walk(path.corners[1], new ProcessState());
                                }
                                else if (path.corners.Length == 2)
                                {
                                    Walker.Walk(path.corners[1] - (path.corners[1] - path.corners[0]).normalized * (Assaulter.AttackRange / 2), new ProcessState());
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ch.Victim.Health > 0)
                        {
                            Assaulter.Assault(ch.Victim);
                        }
                    }
                }
            }
        }
    }
}
