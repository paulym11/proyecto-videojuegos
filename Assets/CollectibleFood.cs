using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleFood : MonoBehaviour
{
    [Tooltip("Velocidad a la que el objeto rota sobre sí mismo.")]
    public float rotationSpeed = 50f;

    [Tooltip("Velocidad con la que el objeto sube y baja.")]
    public float bobSpeed = 2f;

    [Tooltip("Altura máxima que alcanzará el objeto al flotar.")]
    public float bobHeight = 0.5f;

    private Vector3 startPosition;

    void Start()
    {
        // Guardamos la posición inicial para calcular el movimiento de flotación.
        startPosition = transform.position;
    }

    void Update()
    {
        // --- Efecto visual de rotación y flotación ---

        // Rotar el objeto constantemente.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Hacer que el objeto suba y baje (flote).
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    /// <summary>
    /// Se llama cuando otro collider entra en nuestro trigger.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que nos ha tocado es el jugador.
        if (other.CompareTag("Player"))
        {
            // Le decimos al script del jugador que hemos sido recogidos.
            PlayerCollection.instance.Collect();

            // Nos destruimos para desaparecer del mapa.
            Destroy(gameObject);
        }
    }
}
