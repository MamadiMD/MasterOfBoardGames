using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoCPU : MonoBehaviour
{
    public void JugarTurnoCPU(List<LogicaFicha> manoCPU)
{
    foreach (LogicaFicha ficha in manoCPU)
    {
        // Si la mesa está vacía, juega la primera que tenga (o el doble más alto)
        if (DominoManager.Instance.extremoIzquierdo == -1)
        {
            JugarFichaCPU(ficha);
            return;
        }

        // Comprueba si encaja en la izquierda o derecha
        if (ficha.ladoA == DominoManager.Instance.extremoIzquierdo || 
            ficha.ladoB == DominoManager.Instance.extremoIzquierdo ||
            ficha.ladoA == DominoManager.Instance.extremoDerecho || 
            ficha.ladoB == DominoManager.Instance.extremoDerecho)
        {
            JugarFichaCPU(ficha);
            return;
        }
    }

    // Si termina el bucle y no jugó, significa que la CPU no tiene fichas válidas y debe "Robar" o "Pasar"
    Debug.Log("La CPU no tiene fichas, debe pasar turno.");
}

private void JugarFichaCPU(LogicaFicha ficha)
{
    DominoManager.Instance.IntentarJugarFicha(ficha, Vector3.zero); // La posición soltada no importa para la CPU, el GameManager hace el "Snap"
}
}
