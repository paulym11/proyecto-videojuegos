using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMouseCursor : MonoBehaviour
{
    // El m�todo Awake() se llama cuando el script es cargado.
    void Awake()
    {
        // Hacemos que el cursor sea visible.
        Cursor.visible = true;

        // Desbloqueamos el cursor para que se pueda mover libremente.
        Cursor.lockState = CursorLockMode.None;
    }
}
