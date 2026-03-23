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

        Debug.Log("CPU tiene " + misFichas.Count + " fichas.");

        // Paso 2: CAPTURAS
        foreach (Ficha f in misFichas)
        {
            Vector2Int? movimientoCaptura = BuscarMovimiento(f, true);
            if (movimientoCaptura != null)
            {
                Debug.Log("CPU encontró CAPTURA en " + movimientoCaptura.Value);
                EjecutarMovimientoIA(f, movimientoCaptura.Value);
                return;
            }
        }

        Debug.Log("CPU no encontró capturas, buscando movimientos normales...");

        // Paso 3: NORMALES
        Shuffle(misFichas); 
        foreach (Ficha f in misFichas)
        {
            Vector2Int? movimientoNormal = BuscarMovimiento(f, false);
            if (movimientoNormal != null)
            {
                Debug.Log("CPU encontró MOVIMIENTO NORMAL en " + movimientoNormal.Value);
                EjecutarMovimientoIA(f, movimientoNormal.Value);
                return;
            }
        }

        Debug.LogWarning("La CPU ha analizado " + misFichas.Count + " fichas y no encontró movimientos legales.");

        // Todo esto en el caso de que la CPU se quede bloqueada
        BoardManager board = GetComponent<BoardManager>();

        // 1. Devolvemos el turno
        board.turnoBlancas = true;

        // 2. Limpiamos la ficha seleccionada por si la IA dejó alguna marcada
        if(board.fichaSeleccionada != null) {
            board.fichaSeleccionada.SeleccionarVisualmente(false);
            board.fichaSeleccionada = null;
        }

        // 3. Devuelvo un mensaje en pantalla indicando que la cpu se bloqueo ("CPU bloqueada, tu turno")
        Debug.Log("Turno devuelto al jugador manualmente.");
        
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
        BoardManager board = GetComponent<BoardManager>();
        if(board == null) {
            Debug.LogError("No se encontró BoardManager en el objeto de la IA");
            return;
        }
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
