using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCF
{
    public class ProcessState
    {
        public event System.Action Completed, Interrupted, Finished;

        public void Interrupt()
        {
            Interrupted?.Invoke();
            Finished?.Invoke();
        }

        public void Complet()
        {
            Completed?.Invoke();
            Finished?.Invoke();
        }

        public ProcessState()
        {
        }

        public ProcessState(System.Action onCompleted, System.Action onInterrupted)
        {
            Completed += onCompleted;
            Interrupted += onInterrupted;
        }

        public ProcessState(System.Action onCompleted, System.Action onInterrupted, System.Action onFinish)
        {
            Completed += onCompleted;
            Interrupted += onInterrupted;
            Finished += onFinish;
        }
    }
}
