using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Albert.Kalashnikova
{
    public class Buff : MonoBehaviour
    {
        public int hp;
        public int ammo;
        public int damagemultiplier = 1;

        public void addHP(ControlPlayer player)
        {
            if (hp == 0) return;
            player.TakeDamage(-hp);
        }

        public void addAmmo(Weapon weapon)
        {
            if (ammo == 0) return;
            weapon.loadout[weapon.currentIndex].AddAmmo(ammo);
        }

        public void multiplyDamage(Weapon weapon)
        {
            if (damagemultiplier <= 1) return;
            weapon.loadout[weapon.currentIndex].Multiply(damagemultiplier);
        }
    }
}
