using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrastreCarta : MonoBehaviour
{
    private bool arrastrando = false;
    private Vector3 posicionOriginal;
    private Camera cam;

    private Carta miCarta;

    void Start() 
    {
        cam = Camera.main;
        miCarta = GetComponent<Carta>(); // Obtenemos el script Carta
    }

    void OnMouseDown()
    {

        // 1. ¿Es mi carta?
        if (!miCarta.esDelJugador) return; 

        // 2. ¿Es mi turno?
        if (!BriscaManager.Instance.turnoJugador) return;

        // 3. ¿La mesa está ocupada? 
        if (BriscaManager.Instance.cartaJugadorMesa != null) return;

        posicionOriginal = transform.position;
        arrastrando = true;
    }

    void OnMouseDrag()
    {
        if (arrastrando)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }

    void OnMouseUp()
    {
        arrastrando = false;
        DetectarSoltado();
    }

    void DetectarSoltado()
    {
        GameObject ancla = GameObject.Find("Ancla_Jugador");
        float distancia = Vector3.Distance(transform.position, ancla.transform.position);

        // Si está cerca y es mi turno
        if (distancia < 3f && BriscaManager.Instance.turnoJugador) 
        {
            // Avisamos al manager
            BriscaManager.Instance.JugadorLanzaCarta(this.GetComponent<Carta>());
            // Desactivamos el arrastre para esta carta ya jugada
            this.enabled = false;
        }
        else
        {
            // Si no, vuelve a la mano 
            transform.position = posicionOriginal; 
        }
    }
}
