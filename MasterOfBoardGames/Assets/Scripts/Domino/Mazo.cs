using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mazo : MonoBehaviour
{
    public List<LogicaFicha> fichasEnPozo = new List<LogicaFicha>();

    public LogicaFicha RobarFicha()
    {
        if (fichasEnPozo.Count > 0)
        {
            LogicaFicha ficha = fichasEnPozo[0];
            fichasEnPozo.RemoveAt(0);
            return ficha;
        }
        return null; // Pozo vacío
    }
}
