using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Albert.Kalashnikova
{
    public class ControlLook : MonoBehaviour
    {

        #region Variables

        public static bool cursorLocked = true;
        public static bool cameraEnabled = cursorLocked;

        public Transform player;
        public Transform cams;
        public Transform weapon;

        public float xSens; // sensitivity X
        public float ySens; // sensitivity Y
        public float maxAngle;

        private Quaternion camCenter;

        #endregion

        #region Monobehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            camCenter = cams.localRotation; // set rotation origin for cameras to camCenter
            updateCursorLock();
        }

        // Update is called once per frame
        void Update()
        {
            if (cameraEnabled)
            {
                setY();
                setX();
            }

            updateCursorLock();
        }
        #endregion

        #region Private Methods
        void setY()
        {
            float t_input = Input.GetAxis("Mouse Y") * ySens * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
            Quaternion t_delta = cams.localRotation * t_adj;

            if (Quaternion.Angle(camCenter, t_delta) < maxAngle)
            {
                cams.localRotation = t_delta;
                weapon.localRotation = t_delta;
            }

            weapon.rotation = cams.rotation;
        }

        void setX()
        {
            float t_input = Input.GetAxis("Mouse X") * xSens * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
            Quaternion t_delta = player.localRotation * t_adj;
            player.localRotation = t_delta;
        }

        void updateCursorLock()
        {
            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = false;
                    cameraEnabled = cursorLocked;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = true;
                    cameraEnabled = cursorLocked;
                }
            }
        }
        #endregion

    }
}