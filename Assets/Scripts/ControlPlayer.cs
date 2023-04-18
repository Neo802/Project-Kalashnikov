using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Albert.Kalashnikova
{
    public class ControlPlayer : MonoBehaviour
    {

        public float speed;
        public float sprintModifier;
        public float jumpForce;
        public Camera normalCam;
        private Rigidbody rig;

        private float baseFOV;
        private float sprintFOVModifier = 1.25f;

        private bool jump;
        private bool sprint;

        // Start is called before the first frame update
        void Start()
        {
            baseFOV = normalCam.fieldOfView;
            Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
        }

        void Update()
        {
            // Controls
            sprint = Input.GetKey(KeyCode.LeftShift); // left-shift is always easier to reach sooo
            jump = Input.GetKeyDown(KeyCode.Space); // so that you jump lol
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Axles
            float t_hmove = Input.GetAxis("Horizontal");
            float t_vmove = Input.GetAxis("Vertical");

            // States
            bool isJumping = jump;
            bool isSprinting = sprint && t_vmove > 0 && !isJumping;

            // Jumping
            if (isJumping)
            {
                rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            // Movement
            Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
            t_direction.Normalize();
            float t_adjustedSpeed = speed;
            
            // FOV + WalkSpeed
            if (isSprinting)
            {
                t_adjustedSpeed *= sprintModifier;
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 8f); //baseFOV * sprintFOVModifier;
            }
            else
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 8f);

            Vector3 t_targetVelocity = transform.TransformDirection(t_direction) * t_adjustedSpeed * Time.deltaTime;
            t_targetVelocity.y = rig.velocity.y;
            rig.velocity = t_targetVelocity;
        }
    }
}
