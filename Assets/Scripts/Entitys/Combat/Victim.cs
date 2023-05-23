using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            HealthChanged?.Invoke(value);
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

    public void Hit(float power)
    {
        Health -= power;
    }
}
