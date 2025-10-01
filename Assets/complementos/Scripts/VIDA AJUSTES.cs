

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace scgFullBodyController
{
    public class HealthController : MonoBehaviour
    {
        

        [Header("Basics")]
        public float health;
        float maxHealth;
        public GameObject ragdoll;
        public bool dontSpawnRagdoll;
        public float deadTime;
        GameObject tempdoll;
        bool meleeDeath;
        public bool isAiOrDummy;

        [Header("Sound")]
        public bool playNoiseOnHurt;
        public float percentageToPlay;
        public AudioClip hurtNoise;

        [Header("Regen")]
        public bool regen;
        public float timeBeforeRegen;
        float origTimeBeforeRegen;
        public float regenSpeed;
        bool alreadyRegenning;

        void Start()
        {
            
            origTimeBeforeRegen = timeBeforeRegen;

            
            maxHealth = health;
        }

        void Update()
        {
            
            if (health <= 0)
            {
                if (!meleeDeath)
                    Die();
            }

            
            if (!isAiOrDummy)
            {
                if (health > 0)
                {
                    GameObject ui = GameObject.FindGameObjectWithTag("hud");
                    ui.GetComponent<hudController>().uiHealth.text = health.ToString();
                }
                else
                {
                    GameObject ui = GameObject.FindGameObjectWithTag("hud");
                    ui.GetComponent<hudController>().uiHealth.text = "0";
                }
            }


            
            if (health == maxHealth && regen && alreadyRegenning)
            {
                alreadyRegenning = false;
                StopCoroutine("regenHealth");
            }

        }

        public void Damage(float damage)
        {
            
            if (!isAiOrDummy)
            {
                health -= damage;

                if (playNoiseOnHurt)
                {
                    if (Random.value < percentageToPlay)
                    {
                        GetComponent<AudioSource>().PlayOneShot(hurtNoise);
                    }
                }
            }
            else
            {
                health -= damage;
                GetComponent<Animator>().SetTrigger("hit");

                if (gameObject.GetComponent<AiController>())
                    gameObject.GetComponent<AiController>().overrideAttack = true;

                if (playNoiseOnHurt)
                {
                    if (Random.value < percentageToPlay)
                    {
                        GetComponent<AudioSource>().PlayOneShot(hurtNoise);
                    }
                }

            }

            
            if (regen)
            {
                timeBeforeRegen = origTimeBeforeRegen;
                StopCoroutine("regenHealth");
                CancelInvoke();
                if (timeBeforeRegen == origTimeBeforeRegen)
                {
                    alreadyRegenning = true;
                    Invoke(nameof(regenEnumeratorStart), timeBeforeRegen);
                }
            }
        }

        void regenEnumeratorStart()
        {
            StartCoroutine("regenHealth");
        }

        IEnumerator regenHealth()
        {
            
            while (health < maxHealth)
            {
                health++;
                yield return new WaitForSeconds(regenSpeed);
            }
        }

        void Die()
        {
            
            if (!dontSpawnRagdoll)
            {
                
                tempdoll = Instantiate(ragdoll, this.transform.position, this.transform.rotation) as GameObject;

                
                tempdoll.GetComponent<ragdollCamera>().isAi = isAiOrDummy;
                Destroy(gameObject);

                Debug.Log("dontSpawnRagdoll");

                if (isAiOrDummy)
                    Destroy(tempdoll, deadTime);
            }
            else if (isAiOrDummy)
            {

                Debug.Log("isAiOrDummy");

                if (gameObject.GetComponent<Animator>())
                    gameObject.GetComponent<Animator>().enabled = false;

                if (gameObject.GetComponent<AiController>())
                    gameObject.GetComponent<AiController>().enabled = false;

                if (gameObject.GetComponent<HealthController>())
                    gameObject.GetComponent<HealthController>().enabled = false;

                if (gameObject.GetComponent<SimpleFootsteps>())
                    gameObject.GetComponent<SimpleFootsteps>().enabled = false;

                if (gameObject.GetComponent<NavMeshAgent>())
                    gameObject.GetComponent<NavMeshAgent>().enabled = false;

                if (gameObject.GetComponentInChildren<OffsetRotation>())
                    gameObject.GetComponentInChildren<OffsetRotation>().enabled = false;

                if (gameObject.GetComponentInChildren<AiGunController>())
                    gameObject.GetComponentInChildren<AiGunController>().enabled = false;

                if (gameObject.GetComponentInChildren<Adjuster>())
                    gameObject.GetComponentInChildren<Adjuster>().enabled = false;

                Destroy(gameObject, deadTime);
            }
        }

        public void DamageByKick(Vector3 pos, float kickForce, int kickDamage)
        {
            
            health -= kickDamage;

            
            if (health <= 0)
            {
                meleeDeath = true;
                tempdoll = Instantiate(ragdoll, this.transform.position, this.transform.rotation) as GameObject;
                tempdoll.GetComponent<ragdollCamera>().isAi = isAiOrDummy;
                Destroy(gameObject);

                foreach (Rigidbody rb in tempdoll.GetComponentsInChildren<Rigidbody>())
                {
                    rb.AddForce(pos * kickForce);
                }
            }
            else
            {
                
                gameObject.GetComponent<Animator>().SetTrigger("hit");
            }
        }
    }
}