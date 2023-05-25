using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SARP.Entitys
{
    public abstract class IntelligenceCharacter : Character
    {
        [SerializeField]
        float viewRadius, viewAngle, feelRadius;

        private System.Tuple<bool, NavMeshAgent> navMeshAgent = System.Tuple.Create<bool, NavMeshAgent>(false, null);
        protected NavMeshAgent NavMeshAgent
        {
            get
            {
                if (!navMeshAgent.Item1)
                {
                    navMeshAgent = System.Tuple.Create<bool, NavMeshAgent>(true, this.GetComponent<NavMeshAgent>());
                }

                return navMeshAgent.Item2;
            }
        }

        protected IEnumerable<T> ObjectsInView<T>()
        {
            T found;
            RaycastHit hit;
            Vector3 toTarget;
            Collider[] inRange;
            LinkedList<T> inView = new LinkedList<T>();

            inRange = Physics.OverlapSphere(ThisTransorm.position, feelRadius, ~LayerMask.GetMask("Ignore Raycast"));
            foreach (Collider col in inRange)
            {
                found  = col.gameObject.GetComponent<T>();
                if (found != null && col.gameObject != this.gameObject)
                {
                    inView.AddFirst(found);
                }
            }
            inRange = Physics.OverlapSphere(ThisTransorm.position + ThisTransorm.forward * viewRadius, viewRadius, ~LayerMask.GetMask("Ignore Raycast"));
            foreach (Collider col in inRange)
            {
                toTarget = col.transform.position - ThisTransorm.position;
                if (Vector3.Angle(ThisTransorm.forward, toTarget) < viewAngle / 2)
                {
                    toTarget = col.bounds.center - ThisTransorm.position;
                    if (Physics.Raycast(ThisTransorm.position, toTarget, out hit, viewRadius * 2))
                    {
                        if (hit.collider == col)
                        {
                            found = col.gameObject.GetComponent<T>();
                            if (found != null && col.gameObject != this.gameObject)
                            {
                                inView.AddFirst(found);
                            }
                        }
                    }
                }
            }

            return inView;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ThisTransorm.position + ThisTransorm.forward * viewRadius, viewRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ThisTransorm.position, feelRadius);
        }
    }
}
