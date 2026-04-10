using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicaFicha : MonoBehaviour
{
    public int ladoA;
    public int ladoB;
    
    // Si la ficha es doble (ej. 6-6)
    public bool EsDoble => ladoA == ladoB;
}
