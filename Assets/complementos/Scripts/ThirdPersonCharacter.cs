

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scgFullBodyController
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ThirdPersonCharacter : MonoBehaviour
    {
        

        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_RunCycleLegOffset = 0.2f; 
        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;
        public float sensitivity;
        Rigidbody m_Rigidbody;
        Animator m_Animator;
        [HideInInspector] public bool m_IsGrounded;
        float m_OrigGroundCheckDistance;
        const float k_Half = 0.5f;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;
        float m_CapsuleHeight;
        Vector3 m_CapsuleCenter;
        CapsuleCollider m_Capsule;
        [HideInInspector] public bool m_Crouching;
        [HideInInspector] public bool m_Sliding;
        public float jumpDamping;
        float maxCamOriginal;
        float minCamOriginal;
        public CameraController camController;
        bool toggle;

        void Start()
        {
            maxCamOriginal = camController.maxPitch;
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }


        public void Move(Vector3 move, bool crouch, bool jump, bool slide, bool vaulting)
        {
            
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus(vaulting);
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            if (!vaulting)
                m_ForwardAmount = move.z;
            else
                m_ForwardAmount = 0;


            ScaleCapsuleForCrouching(crouch);
            ScaleCapsuleForSliding(slide);
            PreventStandingInLowHeadroom();
        }
        public void updateLate(Vector3 move, bool crouch, bool prone, bool vaulting, bool forwards, bool backwards, bool strafe, float horizontal, float vertical)
        {
            m_TurnAmount = camController.relativeYaw;
            transform.eulerAngles = new Vector3(0, camController.transform.eulerAngles.y, 0);
            UpdateAnimator(move, crouch, prone, vaulting, forwards, backwards, strafe, horizontal, vertical);
        }

        public void HandleGroundMovement(bool crouch, bool jump, bool slide)
        {
            if (m_IsGrounded)
            {
                HandleGroundedMovement(crouch, jump, slide);
            }
            else
            {
                HandleAirborneMovement();
            }
        }


        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (m_IsGrounded && crouch)
            {
                if (m_Crouching) return;
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                m_Crouching = true;
            }
            else
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                    return;
                }
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                m_Crouching = false;
            }
        }

        void ScaleCapsuleForSliding(bool slide)
        {
            if (m_IsGrounded && slide)
            {
                if (m_Sliding) return;
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                m_Sliding = true;
            }
            else
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Sliding = true;
                    return;
                }
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                m_Sliding = false;
            }
        }

        void PreventStandingInLowHeadroom()
        {
            
            if (!m_Crouching)
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                }
            }
        }

        public void kick()
        {
            m_Animator.SetTrigger("kick");
        }

        public void UpdateAnimator(Vector3 move, bool crouch, bool prone, bool vaulting, bool forwards, bool backwards, bool strafe, float horizontal, float vertical)
        {
            

            if (backwards)
            {
                m_Animator.SetFloat("Forward", m_ForwardAmount * -1, 0.1f, Time.deltaTime);
                m_Animator.SetFloat("animSpeed", -1);
            }
            else if (forwards)
            {
                m_Animator.SetFloat("animSpeed", 1);
                m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            }
            else
            {
                m_Animator.SetFloat("animSpeed", 1);
                m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            }

            if (strafe)
            {
                m_Animator.SetBool("strafe", true);
            }
            else
            {
                m_Animator.SetBool("strafe", false);
            }

            m_Animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);

            if (prone)
            {
                m_Animator.SetBool("prone", true);
                camController.maxPitch = 45;
            }
            else
            {
                m_Animator.SetBool("prone", false);
                camController.maxPitch = maxCamOriginal;
            }

            if (vaulting)
            {
                m_Animator.SetBool("vaulting", true);
            }
            else
            {
                m_Animator.SetBool("vaulting", false);
            }

            m_Animator.SetFloat("Turn", m_TurnAmount * 0.3f, 0.1f, Time.deltaTime);
            m_Animator.SetBool("Crouch", m_Crouching);
            m_Animator.SetBool("slide", m_Sliding);
            m_Animator.SetBool("OnGround", m_IsGrounded);

            if (!m_IsGrounded)
            {
                m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
            }

            
            float runCycle =
                Mathf.Repeat(
                    m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
            if (m_IsGrounded)
            {
                m_Animator.SetFloat("JumpLeg", jumpLeg);
            }

            
            if (m_IsGrounded && move.magnitude > 0)
            {
                m_Animator.speed = m_AnimSpeedMultiplier;
            }
            else
            {
                
                m_Animator.speed = 1;
            }
        }


        void HandleAirborneMovement()
        {
            
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }


        void HandleGroundedMovement(bool crouch, bool jump, bool slide)
        {
            
            if (jump && !slide && !crouch)
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
                {
                    initiateJump();
                }
                else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("StrafeStanding"))
                {
                    initiateJump();
                }
            }
        }

        void initiateJump()
        {
            
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x * jumpDamping, m_JumpPower, m_Rigidbody.velocity.z * jumpDamping);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }

        public void OnAnimatorMove()
        {
           
            if (m_IsGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

                
                v.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = v;
            }
        }


        void CheckGroundStatus(bool vaulting)
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {

                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                m_Animator.applyRootMotion = true;
            }
            else if (!vaulting)
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
                m_Animator.applyRootMotion = false;
            }
        }
    }
}

