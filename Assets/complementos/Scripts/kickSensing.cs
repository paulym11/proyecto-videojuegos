﻿

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scgFullBodyController
{
    public class kickSensing : MonoBehaviour
    {
        public float playerKickforce;
        public float doorKickforce;
        public GameObject cameraObj;
        public AudioClip kickSound;
        public int kickDamage;

        void OnTriggerEnter(Collider col)
        {
            
            if (col.transform.tag == "Player" && transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Kick") && col.transform.root.GetComponent<HealthController>())
            {
                col.transform.root.GetComponent<HealthController>().DamageByKick(cameraObj.transform.forward * 360, playerKickforce, kickDamage);
                gameObject.GetComponent<AudioSource>().PlayOneShot(kickSound);
            }

            
            if (col.transform.tag == "Door" && transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Kick"))
            {
                col.GetComponent<Rigidbody>().AddForce(cameraObj.transform.forward * 360 * doorKickforce);
                gameObject.GetComponent<AudioSource>().PlayOneShot(kickSound);
            }
        }
    }
}
