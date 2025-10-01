
using UnityEngine;
using UnityEngine.AI;

namespace scgFullBodyController
{
    public class AiController : MonoBehaviour
    {
        NavMeshAgent agent;

        public AiGunController AiGun;

        Transform player;

        public LayerMask groundLayer, playerLayer;

        public float health;

        
        public Vector3 walkPoint;
        bool walkPointSet;
        public float walkPointRange;
        [HideInInspector] public bool moving;

        
        public float sightRange, attackRange;
        public bool playerInSightRange, playerInAttackRange;
        public bool overrideAttack;

        
        public Transform spine;
        public float m_ForwardAmount;
        float m_TurnAmount;
        Animator m_Animator;

        
        public float moveDamping;


        Vector3 lastPos;
        public Transform shootPoint;

        private void Awake()
        {
            
            player = GameObject.FindWithTag("hitCollider").transform;
            agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
        }

        private void Update()
        {
            
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

            
            if (!overrideAttack)
            {
                if (!playerInSightRange && !playerInAttackRange) Patroling();
                if (playerInSightRange && !playerInAttackRange) ChasePlayer();
                if (playerInAttackRange && playerInSightRange)
                    AttackPlayerFixed();
                else if (AiGun.firing)
                    AiGun.FireCancel();
            }
            else
            {
                
                if (!playerInAttackRange && player != null)
                    ChasePlayerAttack();

                if (playerInAttackRange)
                    overrideAttack = false;
            }

            
            m_TurnAmount = transform.rotation.y;

            
            if (transform.position != lastPos)
            {
                m_ForwardAmount = 1 * moveDamping;
                moving = true;
            }
            else
            {
                m_ForwardAmount = 0;
                moving = false;
            }

            lastPos = transform.position;

            
            UpdateAnimator();
        }

        private void Patroling()
        {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet)
                agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }

        private void SearchWalkPoint()
        {
            
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
                walkPointSet = true;
        }

        public void ChasePlayer()
        {
            agent.SetDestination(player.position);
        }

        public void ChasePlayerAttack()
        {
            agent.SetDestination(player.position);
            AttackPlayer();
        }

        public void AttackPlayerFixed()
        {
            
            agent.SetDestination(transform.position);

            Vector3 targetPostitionXZ = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPostitionXZ);
            shootPoint.LookAt(player);

            
            AiGun.Fire();
        }

        public void AttackPlayer()
        {
            

            Vector3 targetPostitionXZ = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPostitionXZ);
            shootPoint.LookAt(player);
            
            AiGun.Fire();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        public void UpdateAnimator()
        {
            
            m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Turn", m_TurnAmount * 0.3f, 0.1f, Time.deltaTime);
        }
    }
}
