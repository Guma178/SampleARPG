using SARP.Entitys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SARPG.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        Victim target;

        [SerializeField]
        Slider healthBar;

        private void Start()
        {
            healthBar.value = target.Health / target.MaximalHealth;
            target.HealthChanged += delegate (float health)
            {
                healthBar.value = health / target.MaximalHealth;
            };
            target.Died += delegate ()
            {
                this.gameObject.SetActive(false);
            };
        }
    }
}
