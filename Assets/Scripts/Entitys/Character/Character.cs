using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARP.Entitys
{
    public class Character : MonoBehaviour
    {
        private System.Tuple<bool, Walker> walker = System.Tuple.Create<bool, Walker>(false, null);
        protected Walker Walker
        {
            get
            {
                if (!walker.Item1)
                {
                    walker = System.Tuple.Create<bool, Walker>(true, this.GetComponent<Walker>());
                }

                return walker.Item2;
            }
        }

        private System.Tuple<bool, Assaulter> assaulter = System.Tuple.Create<bool, Assaulter>(false, null);
        protected Assaulter Assaulter
        {
            get
            {
                if (!assaulter.Item1)
                {
                    assaulter = System.Tuple.Create<bool, Assaulter>(true, this.GetComponent<Assaulter>());
                }

                return assaulter.Item2;
            }
        }

        private System.Tuple<bool, Victim> victim = System.Tuple.Create<bool, Victim>(false, null);
        protected Victim Victim
        {
            get
            {
                if (!victim.Item1)
                {
                    victim = System.Tuple.Create<bool, Victim>(true, this.GetComponent<Victim>());
                }

                return victim.Item2;
            }
        }

        private void Start()
        {
            if (Victim != null)
            {
                Victim.Died += delegate () { Walker.Able = false; Assaulter.Able = false; };
            }
        }
    }
}
