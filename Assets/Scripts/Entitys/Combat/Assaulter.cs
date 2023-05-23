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

        public void Attack(Victim victim)
        { 
            Vector3 distance = victim.ThisTransorm.position - thisTransorm.position;

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
