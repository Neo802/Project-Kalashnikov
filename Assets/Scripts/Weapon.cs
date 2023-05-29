using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.Albert.Kalashnikova
{
    public class Weapon : MonoBehaviourPunCallbacks
    {

        #region Variables
        public Gun[] loadout;
        public Transform weaponParent;
        public GameObject bulletholePrefab;
        public LayerMask canBeShot;

        private float currentCooldown;
        private int currentIndex;
        private GameObject currentWeapon;

        #endregion

        #region Monobehaviour Callbacks

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine) return;
            if (Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.All, 0); }

            if (currentWeapon != null)
            {
                Aim(Input.GetMouseButton(1));

                if ((Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.LeftControl)) && currentCooldown <= 0)
                {
                    photonView.RPC("Shoot", RpcTarget.All);
                }

                // weapon position elasticity
                currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

                // cooldown
                if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
            }
        }

        #endregion

        #region Private Methods

        [PunRPC]
        void Equip(int p_ind)
        {
            if (currentWeapon != null) Destroy(currentWeapon);

            currentIndex = p_ind;

            GameObject t_newEquipment = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newEquipment.transform.localPosition = Vector3.zero;
            t_newEquipment.transform.localEulerAngles = Vector3.zero;
            t_newEquipment.GetComponent<Sway>().isMine = photonView.IsMine;

            currentWeapon = t_newEquipment;
        }

        [PunRPC]
        void Shoot()
        {
            Transform t_spawn = transform.Find("Cameras/NormalCamera");
            // setup bloom
            Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;

            // bloom
            t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
            t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
            t_bloom -= t_spawn.position;
            t_bloom.Normalize();

            // current cooldown
            currentCooldown = loadout[currentIndex].firerate;

            // raycast
            RaycastHit t_hit = new RaycastHit();

            if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
            {
                GameObject t_newHole = Instantiate(bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                Destroy(t_newHole, 5f);

                if (photonView.IsMine)
                {
                    // shooting a player
                    if (t_hit.collider.gameObject.layer == 9)
                    {
                        // RPC Call to damage player goes here
                        t_hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage);
                    }
                }

            }

            // gun fx
            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;

        }

        [PunRPC]
        private void TakeDamage(int p_damage)
        {
            GetComponent<ControlPlayer>().TakeDamage(p_damage);
        }


        void Aim(bool p_isAiming)
        {
            Transform t_anchor = currentWeapon.transform.Find("Anchor");
            Transform t_state_ads = currentWeapon.transform.Find("States/ADS");
            Transform t_state_hip = currentWeapon.transform.Find("States/Hip");

            if (p_isAiming)
            {
                // aim
                t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }
            else
            {
                // hip
                t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }

        }

        #endregion
    }
}