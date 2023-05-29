using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.Albert.Kalashnikova
{
    public class ControlPlayer : MonoBehaviourPunCallbacks
    {

        #region Variables

        public float speed;
        public float sprintModifier;
        public float jumpForce;
        public Camera normalCam;
        public GameObject cameraParent;
        public Transform weaponParent;
        public Transform groundDetector;
        public LayerMask ground;

        private Rigidbody rig;

        private Vector3 targetWeaponBobPosition;
        private Vector3 weaponParentOrigin;

        private float movementCounter;
        private float idleCounter;
        
        private float baseFOV;
        private float sprintFOVModifier = 1.25f;

        private int current_health;
        public int max_health;

        private Manager manager;

        #endregion

        #region Monobehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            current_health = max_health;
            cameraParent.SetActive(photonView.IsMine);

            if (!photonView.IsMine) gameObject.layer = 9;

            baseFOV = normalCam.fieldOfView;
            if (Camera.main) Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
            weaponParentOrigin = weaponParent.localPosition;
        }

        void Update()
        {   // so the logic remains the same
            if (!photonView.IsMine) return;

            // Axles
            float t_hmove = Input.GetAxis("Horizontal");
            float t_vmove = Input.GetAxis("Vertical");

            // Controls
            bool sprint = Input.GetKey(KeyCode.LeftShift); // left-shift is always easier to reach sooo
            bool jump = Input.GetKeyDown(KeyCode.Space); // so that you jump lol

            bool aim = Input.GetMouseButton(1);

            // States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && t_vmove > 0 && !isJumping && isGrounded && !aim;

            // Jumping
            if (isJumping)
            {
                rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            if (Input.GetKeyDown(KeyCode.U)) TakeDamage(50); 

            // head bob
            if (t_hmove == 0 && t_vmove == 0) 
            { 
                HeadBob(idleCounter, 0.025f, 0.025f); 
                idleCounter += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
            }
            else if (!isSprinting)
            {
                HeadBob(movementCounter, 0.035f, 0.035f); 
                movementCounter += Time.deltaTime * 3f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else
            {
                HeadBob(movementCounter, 0.15f, 0.075f);
                movementCounter += Time.deltaTime * 7f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!photonView.IsMine) return;

            // Axles
            float t_hmove = Input.GetAxis("Horizontal");
            float t_vmove = Input.GetAxis("Vertical");

            // Controls
            bool sprint = Input.GetKey(KeyCode.LeftShift); // left-shift is always easier to reach sooo
            bool jump = Input.GetKeyDown(KeyCode.Space); // so that you jump lol

            bool aim = Input.GetMouseButton(1);

            // States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && t_vmove > 0 && !isJumping && isGrounded && !aim;

            // Movement
            Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
            t_direction.Normalize();

            float t_adjustedSpeed = speed;
            if (isSprinting) t_adjustedSpeed *= sprintModifier;

            Vector3 t_targetVelocity = transform.TransformDirection(t_direction) * t_adjustedSpeed * Time.deltaTime;
            t_targetVelocity.y = rig.velocity.y;
            rig.velocity = t_targetVelocity;

            // FOV
            if (isSprinting)
            {
                t_adjustedSpeed *= sprintModifier;
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 8f); //baseFOV * sprintFOVModifier;
            }
            else
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 8f);

        }

        #endregion

        #region Private Methods

        void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
        {
            targetWeaponBobPosition = weaponParentOrigin + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
        }

        #endregion

        #region Public Methods

        public void TakeDamage(int p_damage)
        {
            if (photonView.IsMine)
            {
                current_health -= p_damage;
                Debug.Log(current_health);

                if (current_health <= 0)
                {
                    manager.Spawn();
                    PhotonNetwork.Destroy(gameObject);
                    Debug.Log("You are dead!");
                }

            } // until I add healthbars, after that, photonView will be removed
        }

        #endregion 


    }
}
