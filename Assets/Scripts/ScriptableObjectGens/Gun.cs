using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Albert.Kalashnikova
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        public string name;
        public int ammo;
        public int clipsize;

        public int damage;
        public float firerate;
        public float bloom;
        public float recoil;
        public float kickback;

        public float aimSpeed;
        public float reload;
        public GameObject prefab;

        //private AudioSource shoot;
        //private AudioSource recharge;

        private int stash; // current ammo
        private int clip; // current clip

        public void Initialize()
        {
            //shoot = prefab.transform.Find("Audio").GetComponent<AudioSource>();

            stash = ammo;
            clip = clipsize;
        }

        public bool FireBullet()
        {
            if (clip > 0)
            {
                clip -= 1;
                return true;
            }
            else return false;
        }

        public void Reload()
        {
            stash += clip;
            clip = Mathf.Min(clipsize, stash);
            stash -= clip;
        }

        public int GetStash() { return stash; }
        public int GetClip() { return clip; }
    }
}
