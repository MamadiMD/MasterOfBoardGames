using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuDamas : MonoBehaviour
{
    private BoardManager board;

    void Start()
    {
        board = GetComponent<BoardManager>();
    }

    // Explicacion de como funciona la CPU : Basicamente la cpu en cada uno de sus turnos realiza 3 pasos , el primero es buscar todas las 
    // fichas negras del tablero , el segundo es buscar movimientos de captura es decir busca haber si puede capturar alguna ficha , en el caso de que pueda capturar la ficha realizara el respectivo movimiento
    // y por ultimo el tercer paso es en el caso de que no encuentre ningun movimiento de captura , realiza un movimiento normal , barajando la lista de fichas para que no este moviendo todo el rato la misma ficha

    public void RealizarTurnoCPU()
    {
        // Paso 1
        Ficha[] todasLasFichas = GetComponentsInChildren<Ficha>();
        List<Ficha> misFichas = new List<Ficha>();
        foreach (Ficha f in todasLasFichas)
        {
            if (f.tipoFicha == 2) misFichas.Add(f);
        }

        // Paso 2
        foreach (Ficha f in misFichas)
        {
            Vector2Int? movimientoCaptura = BuscarMovimiento(f, true);
            if (movimientoCaptura != null)
            {
                EjecutarMovimientoIA(f, movimientoCaptura.Value);
                return;
            }
        }

        // Paso 3
        Shuffle(misFichas); 
        foreach (Ficha f in misFichas)
        {
            Vector2Int? movimientoNormal = BuscarMovimiento(f, false);
            if (movimientoNormal != null)
            {
                EjecutarMovimientoIA(f, movimientoNormal.Value);
                return;
            }
        }
    }

    // Busca si una ficha específica tiene movimientos válidos
    Vector2Int? BuscarMovimiento(Ficha f, bool soloCapturas)
    {
        int[] direccionesX = { -1, 1, -2, 2 };
        int[] direccionesY = { -1, 1, -2, 2 }; // Las negras bajan (-1), pero las Damas suben (+1)

        foreach (int dx in direccionesX)
        {
            foreach (int dy in direccionesY)
            {
                int targetX = f.gridX + dx;
                int targetY = f.gridY + dy;

                // Comprobamos si el movimiento es válido usando la lógica que ya tienes
                if (board.EsMovimientoValidoParaIA(f, targetX, targetY, soloCapturas))
                {
                    return new Vector2Int(targetX, targetY);
                }
            }
        }
        return null;
    }

    void EjecutarMovimientoIA(Ficha f, Vector2Int destino)
    {
        board.fichaSeleccionada = f;
        board.IntentarMoverFicha(destino.x, destino.y);
    }

    // Mezclo la lista para que la IA sea menos predecible
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
