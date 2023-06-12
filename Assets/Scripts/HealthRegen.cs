using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Albert.Kalashnikova
{
    public class HealthRegen : MonoBehaviour
    {
        public int regenRate = 1;
        public float regenInterval = 1f;
        private ControlPlayer plr;

        private int currentHealth;
        private int maxHealth;

        private IEnumerator Start()
        {
            plr = transform.GetComponent<ControlPlayer>();

            currentHealth = plr.current_health;
            maxHealth = plr.max_health;

            while (true)
            {
                yield return new WaitForSeconds(regenInterval);
                currentHealth = plr.current_health;

                if (currentHealth < maxHealth)
                {
                    plr.TakeDamage(-regenRate);
                }
            }
        }
    }
}