using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCollection : MonoBehaviour
{
    // Usamos un 'Singleton' para que otros scripts puedan acceder a este fácilmente.
    public static PlayerCollection instance;

    [Header("Configuración de Recolección")]
    [Tooltip("Cuántos objetos de comida se necesitan para ganar.")]
    public int foodToWin = 10;

    [Tooltip("El nombre exacto de la escena de victoria a cargar.")]
    public string winSceneName = "Felicidades"; // Asegúrate de que este nombre coincida con tu escena

    [Header("Conexiones del HUD")]
    [Tooltip("Arrastra aquí el objeto de Texto de la UI que mostrará el contador.")]
    public Text foodCounterText;

    // Variable privada para llevar la cuenta.
    private int foodCollected = 0;

    private void Awake()
    {
        // Configuración del Singleton.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Actualizamos el HUD al empezar el nivel.
        UpdateFoodCounter();
    }

    /// <summary>
    /// Este método es llamado por el script del objeto de comida cuando es recogido.
    /// </summary>
    public void Collect()
    {
        // Aumentamos el contador.
        foodCollected++;

        Debug.Log("Comida recogida. Total: " + foodCollected);

        // Actualizamos el texto en la pantalla.
        UpdateFoodCounter();

        // Comprobamos si hemos ganado.
        if (foodCollected >= foodToWin)
        {
            WinGame();
        }
    }

    /// <summary>
    /// Actualiza el texto del HUD para mostrar el progreso.
    /// </summary>
    private void UpdateFoodCounter()
    {
        if (foodCounterText != null)
        {
            foodCounterText.text = "COMIDA: " + foodCollected + " / " + foodToWin;
        }
    }

    /// <summary>
    /// Se llama cuando se cumple la condición de victoria.
    /// </summary>
    private void WinGame()
    {
        Debug.Log("¡Has ganado! Cargando la escena de victoria...");
        // Carga la escena de felicitaciones.
        SceneManager.LoadScene(winSceneName);
    }
}
