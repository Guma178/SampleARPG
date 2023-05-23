using SARP.Entitys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARP.Entitys
{
    public class Walker : MonoBehaviour
    {
        public bool Able
        {
            get;
            set;
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

        private System.Tuple<bool, Mover> mover = System.Tuple.Create<bool, Mover>(false, null);
        private Mover Mover
        {
            get
            {
                if (!mover.Item1)
                {
                    mover = System.Tuple.Create<bool, Mover>(true, this.GetComponent<Mover>());
                }

                return mover.Item2;
            }
        }

        public void BeginWalk()
        {
            Animator.SetBool("Move", true);
        }
        public void EndWalk()
        {
            Animator.SetBool("Move", false);
        }

        public void Walk(Vector3 direction)
        {
            if (Able)
            {
                Mover.Move(direction);
            }
        }

        public void Walk(Vector3 target, ProcessState state)
        {
            state.Finished += delegate () { EndWalk(); };
            Mover.MoveTo(target, state, );
            BeginWalk();
        }

        public void Walk(Transform target, ProcessState state)
        {
            state.Finished += delegate () { EndWalk(); };
            Mover.MoveTo(target, state);
            BeginWalk();
        }
    }
}
