using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Albert.Kalashnikova
{
    public class Weapon : MonoBehaviour
    {

        #region Variables
        public Gun[] loadout;
        public Transform weaponParent;
        public GameObject bulletholePrefab;
        public LayerMask canBeShot;

        private int currentIndex;
        private GameObject currentWeapon;

        #endregion

        #region Monobehaviour Callbacks

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);

            if (currentWeapon != null)
            {
                Aim(Input.GetMouseButton(1));

                if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.LeftControl))
                {
                    Shoot();
                }

            }
        }

        #endregion

        #region Private Methods

        void Equip(int p_ind)
        {
            if (currentWeapon != null) Destroy(currentWeapon);

            currentIndex = p_ind;

            GameObject t_newEquipment = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newEquipment.transform.localPosition = Vector3.zero;
            t_newEquipment.transform.localEulerAngles = Vector3.zero;

            currentWeapon = t_newEquipment;
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

        void Shoot()
        {
            Transform t_spawn = transform.Find("Cameras/NormalCamera");

            RaycastHit t_hit = new RaycastHit();

            if (Physics.Raycast(t_spawn.position, t_spawn.forward, out t_hit, 1000f, canBeShot))
            {
                GameObject t_newHole = Instantiate(bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                Destroy(t_newHole, 5f);
            }

        }

        #endregion
    }
}