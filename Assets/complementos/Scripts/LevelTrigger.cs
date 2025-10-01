using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTrigger : MonoBehaviour
{
    [Tooltip("Arrastra aqu� el objeto padre que contiene todas las rejas/barrotes.")]
    public GameObject objectToDisable;

    [Header("Di�logo a Mostrar")]
    [Tooltip("Arrastra aqu� el objeto de Texto de la UI donde se mostrar� el di�logo.")]
    public Text dialogueTextUI;

    [Tooltip("Escribe aqu� el di�logo que quieres que aparezca. Puedes copiar y pegar desde tu Canvas.")]
    [TextArea(3, 10)] // Esto hace que el campo de texto sea m�s grande en el Inspector.
    public string dialogueToShow;

    [Tooltip("El nombre exacto de la escena a la que quieres pasar (ej: Nivel2).")]
    public string levelToLoad = "Nivel2";

    [Tooltip("Tiempo en segundos que esperar� el juego antes de cambiar de nivel.")]
    public float delayBeforeLoading = 3.0f;

    // Variable para asegurarnos de que el trigger solo se active una vez.
    private bool hasBeenTriggered = false;

    /// <summary>
    /// Este m�todo se ejecuta autom�ticamente cuando un objeto con un Rigidbody entra en el trigger.
    /// </summary>
    /// <param name="other">El collider del objeto que ha entrado.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que entr� es el jugador y si el trigger no ha sido activado antes.
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            // Marcamos que ya se activ� para evitar que se ejecute m�ltiples veces.
            hasBeenTriggered = true;

            Debug.Log("Jugador ha activado el trigger. Desactivando objeto y comenzando cuenta atr�s.");

            // Iniciamos la secuencia de eventos.
            StartCoroutine(TriggerSequence());
        }
    }

    /// <summary>
    /// Corutina que maneja la secuencia de desactivar el objeto y cargar el nivel.
    /// </summary>
    private IEnumerator TriggerSequence()
    {
        // Primero, comprobamos si hay un objeto asignado para desactivar.
        if (objectToDisable != null)
        {
            // Desactivamos el objeto (las rejas).
            objectToDisable.SetActive(false);
            dialogueTextUI.text = dialogueToShow;
        }

        // Esperamos el tiempo definido en 'delayBeforeLoading'.
        yield return new WaitForSeconds(delayBeforeLoading);

        // Finalmente, cargamos el siguiente nivel.
        Debug.Log("Cargando nivel: " + levelToLoad);
        SceneManager.LoadScene(levelToLoad);
    }
}
