
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scgFullBodyController
{
    public class registerHit : MonoBehaviour
    {
        

        public GameObject impactParticle;
        public GameObject impactBloodParticle;
        public float impactDespawnTime;
        [HideInInspector] public int damage;

        void OnCollisionEnter(Collision col)
        {
            
            if (col.transform.tag == "Player")
            {
                
                if (col.transform.root.gameObject.GetComponent<HealthController>())
                {
                    col.transform.root.gameObject.GetComponent<HealthController>().Damage(damage);
                }

                
                GameObject tempImpact;
                tempImpact = Instantiate(impactBloodParticle, this.transform.position, this.transform.rotation) as GameObject;
                tempImpact.transform.Rotate(Vector3.left * 90);
                Destroy(tempImpact, impactDespawnTime);
            }
            else
            {
                
                GameObject tempImpact;
                tempImpact = Instantiate(impactParticle, this.transform.position, this.transform.rotation) as GameObject;
                tempImpact.transform.Rotate(Vector3.left * 90);
                Destroy(tempImpact, impactDespawnTime);
            }

            
            Destroy(gameObject);
        }
    }
}
