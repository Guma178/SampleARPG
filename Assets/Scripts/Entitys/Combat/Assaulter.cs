using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SARP.Entitys
{
    public class Assaulter : MonoBehaviour
    {
        [SerializeField]
        float attackRange, attackPower;

        public virtual float AttackRange => attackRange;
        public virtual float AttackPower => attackPower;

        private Transform thisTransorm;
        public Transform ThisTransorm
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

        private System.Tuple<bool, Animator> animator = System.Tuple.Create<bool, Animator>(false, null);
        private Animator Animator
        {
            get
            {
                if (!animator.Item1)
                {
                    animator = System.Tuple.Create<bool, Animator>(true, this.GetComponent<Animator>());
                }

                return animator.Item2;
            }
        }

        private Victim victim;
        private ProcessState process;

        public void Assault(Victim victim, ProcessState process)
        {
            this.victim = victim;
            this.process = process;
            Animator.SetTrigger("Attack");
        }

        private void Attack()
        {
            Vector3 distance;

            if (victim != null && process != null)
            {
                process.Complet();

                distance = victim.ThisTransorm.position - ThisTransorm.position;
                if (distance.magnitude <= AttackRange)
                {
                    float angle = Vector3.Angle(ThisTransorm.forward, distance.normalized);
                    if (Vector3.Angle(ThisTransorm.forward, distance.normalized) < 45)
                    {
                        victim.Hit(AttackPower);
                    }
                }
            }
        }
    }
}
