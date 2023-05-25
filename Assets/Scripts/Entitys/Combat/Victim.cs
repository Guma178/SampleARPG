using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARP.Entitys
{
    public class Victim : MonoBehaviour
    {
        [SerializeField]
        float maximalHealth;

        public event System.Action<float> HealthChanged;
        public event System.Action Died;

        private float health;
        public virtual float Health
        {
            get
            {
                return health;
            }

            private set
            {
                if (value > maximalHealth)
                {
                    health = maximalHealth;
                }
                else
                {
                    health = value;
                }

                if (value < 0)
                {
                    health = 0;
                }

                if (health == 0)
                {
                    Died?.Invoke();
                }

                HealthChanged?.Invoke(health);
            }
        }

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

        private void Start()
        {
            Health = maximalHealth;

            Died += delegate () { Animator.SetBool("Dead", true); };
        }

        public void Hit(float power)
        {
            Health -= power;
            Animator.SetTrigger("Hit");
        }
    }
}
