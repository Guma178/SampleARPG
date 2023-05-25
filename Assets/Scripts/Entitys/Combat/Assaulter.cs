using System.Collections;
using System.Collections.Generic;
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

        public void Assault(Victim victim)
        { 
            this.victim = victim;
            Animator.SetTrigger("Attack");
        }

        private void Attack()
        {
            Vector3 distance;

            if (victim != null)
            {
                distance = victim.ThisTransorm.position - ThisTransorm.position;
                if (distance.magnitude <= AttackRange)
                {
                    if (Vector3.Angle(ThisTransorm.forward, distance.normalized) < 25)
                    {
                        victim.Hit(AttackPower);
                    }
                }
            }
        }
    }
}
