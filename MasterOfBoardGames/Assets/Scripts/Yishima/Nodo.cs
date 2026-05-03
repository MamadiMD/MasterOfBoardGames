using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo : MonoBehaviour
{
    public bool ocupado = false;      // ¿Hay una pieza en esa posicion?
    public GameObject piezaActual;    // Referencia a la pieza que lo ocupa
    public List<Nodo> vecinos;        // Nodos a los que se puede saltar desde aquí
}

