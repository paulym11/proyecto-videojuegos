using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEditor;
using TMPro;
using System;

public class MenuController : MonoBehaviour
{
    public void Jugar()
    {
        // Cambia "Nivel1_Rescate" por el nombre real de tu escena del primer nivel
        SceneManager.LoadScene("Scenes/PROYECTOFPS");
    }

    public void Salir()
    {
        Debug.Log("Salir del juego");
        Application.Quit(); 
    }

    public void GoToMenuPrincipal()
    {
        SceneManager.LoadScene("Scenes/MenuPrincipal");
    }
}
