

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace scgFullBodyController
{
    public class ragdollCamera : MonoBehaviour
    {
        public Transform headBone;
        public bool isAi;

        // Variable pública para que puedas ajustar el tiempo de espera en el Inspector de Unity.
        public float delayBeforeLoad = 0.0f;

        void Start()
        {
            if (!isAi)
            {
                // En lugar de poner todo aquí, iniciamos la corutina.
                StartCoroutine(DeathSequence());
            }
        }

        // Una corutina nos permite pausar la ejecución del código.
        IEnumerator DeathSequence()
        {
            Debug.Log("MORI AQUI - Iniciando secuencia de muerte.");

            // Tu código original para preparar la cámara.
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            GameObject hud = GameObject.FindGameObjectWithTag("hud");

            if (hud != null)
            {
                hud.SetActive(false);
            }

            if (cam != null && headBone != null)
            {
                cam.transform.parent = headBone;
                cam.transform.localPosition = new Vector3(0, 0, .3f);
            }

            // --- LA PARTE NUEVA E IMPORTANTE ---
            // Esperamos los segundos que definimos en 'delayBeforeLoad'.
            // Durante este tiempo, verás la animación de la caída.
            yield return new WaitForSeconds(delayBeforeLoad);

            // Después de la espera, cargamos la escena de Game Over.
            SceneManager.LoadScene("Scenes/GameOver");
        }

    }
}
