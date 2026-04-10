using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FichaInteractiva : MonoBehaviour
{
    private Vector3 posicionInicial;
    private bool estaArrastrando = false;
    public bool esDelJugador = false;

    void OnMouseDown()
    {
        if (!DominoManager.Instance.esTurnoJugador) return;

        if (!esDelJugador) return;

        // Guardamos la posición original por si la jugada no es válida y debe volver
        posicionInicial = transform.position;
        estaArrastrando = true;
    }

    void OnMouseDrag()
    {
        if (estaArrastrando)
        {
            // Movemos la ficha con el ratón
            Vector3 posicionRaton = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            posicionRaton.z = 0;
            transform.position = posicionRaton;
        }
    }

    void OnMouseUp()
    {
        if (!estaArrastrando) return;

        estaArrastrando = false;
        
        bool jugadaValida = DominoManager.Instance.IntentarJugarFicha(GetComponent<LogicaFicha>(), transform.position);

        
        if (!jugadaValida)
        {
            // Si no es válida, vuelve a la mano del jugador
            transform.position = posicionInicial;
        }
        else
        {
            this.enabled = false; 
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
