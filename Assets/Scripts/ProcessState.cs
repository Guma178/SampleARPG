using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARP
{
    public class ProcessState
    {
        public bool IsFinished { get; private set; }

        public event System.Action Completed, Interrupted, Finished;

        public void Interrupt()
        {
            if (!IsFinished)
            {
                Interrupted?.Invoke();
                Finished?.Invoke();
            }
        }

        public void Complet()
        {
            if (!IsFinished)
            {
                IsFinished = true;
                Completed?.Invoke();
                Finished?.Invoke();
            }
        }

        public ProcessState()
        {
            IsFinished = false;
        }

        public ProcessState(System.Action onCompleted, System.Action onInterrupted)
        {
            IsFinished = false;
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
