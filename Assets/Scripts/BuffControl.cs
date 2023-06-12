using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Com.Albert.Kalashnikova
{
    public class BuffControl : MonoBehaviour
    {
        private Buff buff;
        private bool debounce = false;

        private void waitfor()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 11)
            {
                if (debounce == true) return;

                debounce = true;
                buff = collision.gameObject.GetComponent<Buff>();

                if (buff.hp != 0)
                {
                    buff.addHP(transform.gameObject.GetComponent<ControlPlayer>());
                }

                if (buff.ammo > 0)
                {
                    buff.addAmmo(transform.gameObject.GetComponent<Weapon>());
                }

                if (buff.damagemultiplier > 1)
                {
                    buff.multiplyDamage(transform.gameObject.GetComponent<Weapon>());
                }

                Object.Destroy(collision.gameObject);
                Invoke("waitfor", 1);
                debounce = false;
            }
        }
    }
}