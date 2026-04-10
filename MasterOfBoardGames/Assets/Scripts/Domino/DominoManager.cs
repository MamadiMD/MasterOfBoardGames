using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoManager : MonoBehaviour
{
    public static DominoManager Instance;

    [Header("Estado del Juego")]
    public int extremoIzquierdo = -1;
    public int extremoDerecho = -1;
    public bool esTurnoJugador = true;

    [Header("Listas de Fichas")]
    public List<LogicaFicha> manoJugador = new List<LogicaFicha>();
    public List<LogicaFicha> manoCPU = new List<LogicaFicha>();
    public List<LogicaFicha> pozo = new List<LogicaFicha>();

    [Header("Ajustes Visuales")]
    public float distanciaEntreFichas = 1.2f; 
    public Transform anclaIzquierda;
    public Transform anclaDerecha;
    public Transform anclaCentro;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public bool IntentarJugarFicha(LogicaFicha ficha, Vector3 posicionSoltada)
    {
        // 1. Caso Mesa vacía (Primera ficha)
        if (extremoIzquierdo == -1 && extremoDerecho == -1)
        {
            extremoIzquierdo = ficha.ladoA;
            extremoDerecho = ficha.ladoB;

            if (esTurnoJugador) manoJugador.Remove(ficha);
            else manoCPU.Remove(ficha);

            PosicionarEnAncla(ficha, anclaCentro, !ficha.EsDoble);

            // Inicializamos las anclas laterales partiendo del centro
            anclaIzquierda.position = anclaCentro.position + new Vector3(-distanciaEntreFichas, 0, 0);
            anclaDerecha.position = anclaCentro.position + new Vector3(distanciaEntreFichas, 0, 0);

            CambiarTurno();
            return true;
        }

        // 2. Decidir a qué lado intentar jugar según la cercanía del ratón
        float distIzq = Vector3.Distance(posicionSoltada, anclaIzquierda.position);
        float distDer = Vector3.Distance(posicionSoltada, anclaDerecha.position);

        bool exito = false;
        if (distIzq < distDer)
        {
            exito = ComprobarYJugar(ficha, true); // Intenta izquierda primero
            if (!exito) exito = ComprobarYJugar(ficha, false); // Si no, intenta derecha
        }
        else
        {
            exito = ComprobarYJugar(ficha, false); // Intenta derecha primero
            if (!exito) exito = ComprobarYJugar(ficha, true); // Si no, intenta izquierda
        }

        if (exito)
        {
            if (esTurnoJugador) manoJugador.Remove(ficha);
            else manoCPU.Remove(ficha);

            CambiarTurno();
            return true;
        }

        return false;
    }

    public void RobarFicha(bool paraJugador)
    {
        if (pozo.Count > 0)
        {
            LogicaFicha fichaRobada = pozo[0];
            pozo.RemoveAt(0);

            if (paraJugador)
            {
                manoJugador.Add(fichaRobada);
            }
            else
            {
                manoCPU.Add(fichaRobada);
            }
            
            Debug.Log(paraJugador ? "Jugador robó ficha" : "CPU robó ficha");
        }
        else
        {
            Debug.Log("El pozo está vacío.");
        }
    }

    public void CambiarTurno()
    {
        esTurnoJugador = !esTurnoJugador;

        if (!esTurnoJugador)
        {
            StartCoroutine(RutinaTurnoCPU());
        }
        else
        {
            if (!TieneJugadaPosible(manoJugador))
            {
                Debug.Log("Jugador no tiene fichas válidas. ¡Debe robar!");
            }
        }
    }

    public bool TieneJugadaPosible(List<LogicaFicha> mano)
    {
        if (extremoIzquierdo == -1) return true; // Mesa vacía, siempre puede

        foreach (LogicaFicha f in mano)
        {
            if (f.ladoA == extremoIzquierdo || f.ladoB == extremoIzquierdo ||
                f.ladoA == extremoDerecho || f.ladoB == extremoDerecho)
                return true;
        }
        return false;
    }

    private IEnumerator RutinaTurnoCPU()
    {
        yield return new WaitForSeconds(1.5f);

        // La CPU intenta jugar
        bool cpuJugo = false;
        foreach (LogicaFicha f in manoCPU)
        {
            if (IntentarJugarFicha(f, Vector3.zero)) 
            {
                cpuJugo = true;
                break; 
            }
        }

        // Si no pudo jugar, intenta robar
        if (!cpuJugo)
        {
            if (pozo.Count > 0)
            {
                RobarFicha(false);
                StartCoroutine(RutinaTurnoCPU()); // Reintenta jugar tras robar
            }
            else
            {
                Debug.Log("CPU no puede jugar y pozo vacío. Pasa turno.");
                CambiarTurno();
            }
        }
    }

    private void PosicionarFichaEnMesa(LogicaFicha ficha, bool esPrimera)
    {
        // Desactivamos interacción
        if(ficha.TryGetComponent(out FichaInteractiva fi)) Destroy(fi);
        
        Debug.Log("Ficha colocada en mesa. Extremos: " + extremoIzquierdo + " | " + extremoDerecho);
        
        // Aquí conectaremos con la Opción 1 (Puntos de anclaje)
    }

    private void PosicionarEnAncla(LogicaFicha ficha, Transform punto, bool esHorizontal)
    {
        // 1. Mover a la posición del ancla
        ficha.transform.position = punto.position;

        // 2. Rotar la ficha
        if (esHorizontal)
        {
            // Lo rotamos 90 grados
            ficha.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
        else
        {
            // Posición vertical original
            ficha.transform.rotation = Quaternion.identity;
        }

        // 3. Lógica de "Espejo" (Invertir números)
        // Importante: Si el lado A debe ir hacia la izquierda pero el sprite 
        // tiene el lado A arriba, a veces necesitarás rotar 270 en vez de 90.
        // De momento, dejémoslo en 90 para probar el encaje.

        // 4. Limpieza de componentes
        if (ficha.TryGetComponent(out FichaInteractiva fi)) Destroy(fi);
        if (ficha.TryGetComponent(out BoxCollider2D col)) col.enabled = false;
    }

    private bool ComprobarYJugar(LogicaFicha ficha, bool esIzquierda)
    {
        int valorMesa = esIzquierda ? extremoIzquierdo : extremoDerecho;
        Transform ancla = esIzquierda ? anclaIzquierda : anclaDerecha;

        if (ficha.ladoA == valorMesa || ficha.ladoB == valorMesa)
        {
            // Calculamos el nuevo extremo
            int nuevoExtremo = (ficha.ladoA == valorMesa) ? ficha.ladoB : ficha.ladoA;

            if (esIzquierda) extremoIzquierdo = nuevoExtremo;
            else extremoDerecho = nuevoExtremo;

            // Si no es un doble, la acostamos (true). Si es doble, puedes dejarla vertical (false).
            bool debeIrHorizontal = !ficha.EsDoble; 
            PosicionarEnAncla(ficha, ancla, debeIrHorizontal);

            // Desplazamos el ancla para la siguiente ficha
            float desplazamiento = debeIrHorizontal ? distanciaEntreFichas : distanciaEntreFichas / 2;
            ancla.position += new Vector3(esIzquierda ? -desplazamiento : desplazamiento, 0, 0);

            return true;
        }
        return false;
    }
}
