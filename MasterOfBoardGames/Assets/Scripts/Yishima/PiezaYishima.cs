using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiezaYishima : MonoBehaviour
{
    public bool esJugador1; // True para blancas, False para negras
    public Nodo nodoActual; 

    public void MoverA(Nodo nuevoNodo)
    {
        // Si ya estábamos en un nodo, lo liberamos
        if (nodoActual != null) 
        {
            nodoActual.ocupado = false;
            nodoActual.piezaActual = null;
        }
        
        // Actualizamos al nuevo nodo
        nodoActual = nuevoNodo;
        nodoActual.ocupado = true;
        nodoActual.piezaActual = this.gameObject;
        
        // Movimiento físico de la ficha
        transform.position = nuevoNodo.transform.position;
    }
}
