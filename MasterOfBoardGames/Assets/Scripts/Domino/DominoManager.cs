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

    [Header("UI Final")]
    public TMPro.TextMeshProUGUI textoResultado; 
    public GameObject panelFinal;

    public bool juegoActivo = true;


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

            PosicionarEnAncla(ficha, anclaCentro, !ficha.EsDoble, false);

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

            ComprobarGanador();

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
        if (!juegoActivo) yield break;
        
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

    private void PosicionarEnAncla(LogicaFicha ficha, Transform punto, bool esHorizontal, bool invertir)
    {
        ficha.transform.position = punto.position;

        if (esHorizontal)
        {
            // 90 grados normal, o 270 (-90) para darle la vuelta completa
            float anguloZ = invertir ? -270f : -90f;
            ficha.transform.rotation = Quaternion.Euler(0, 0, anguloZ);
        }
        else
        {
            // Para los dobles que van verticales
            float anguloZ = invertir ? 180f : 0f;
            ficha.transform.rotation = Quaternion.Euler(0, 0, anguloZ);
        }

        // Aseguramos visibilidad y limpiamos componentes
        ficha.GetComponent<SpriteRenderer>().enabled = true;
        if (ficha.TryGetComponent(out FichaInteractiva fi)) Destroy(fi);
        if (ficha.TryGetComponent(out BoxCollider2D col)) col.enabled = false;
    }

    private bool ComprobarYJugar(LogicaFicha ficha, bool esIzquierda)
    {
        int valorMesa = esIzquierda ? extremoIzquierdo : extremoDerecho;
        Transform ancla = esIzquierda ? anclaIzquierda : anclaDerecha;

        if (ficha.ladoA == valorMesa || ficha.ladoB == valorMesa)
        {
            bool debeInvertir = false;

            if (esIzquierda)
            {
                // En el lado IZQUIERDO:
                // Si el que encaja es el lado A, el lado B debe quedar hacia afuera (extremo).
                // Con rotación de 90°, el lado A queda a la derecha (pegado a la mesa). Perfecto.
                // Pero si el que encaja es el B, tenemos que invertirla (180° más) para que el B sea el que pegue a la mesa.
                debeInvertir = (ficha.ladoB == valorMesa);
                extremoIzquierdo = (ficha.ladoA == valorMesa) ? ficha.ladoB : ficha.ladoA;
            }
            else
            {
                // En el lado DERECHO:
                // Con rotación de 90°, el lado B es el que queda a la izquierda (pegado a la mesa).
                // Por tanto, si lado B es el que coincide, no invertimos.
                // Si lado A es el que coincide, invertimos para que el A mire a la izquierda.
                debeInvertir = (ficha.ladoA == valorMesa);
                extremoDerecho = (ficha.ladoA == valorMesa) ? ficha.ladoB : ficha.ladoA;
            }

            bool debeIrHorizontal = !ficha.EsDoble;
            PosicionarEnAncla(ficha, ancla, debeIrHorizontal, debeInvertir);

            float desplazamiento = debeIrHorizontal ? distanciaEntreFichas : distanciaEntreFichas / 2;
            ancla.position += new Vector3(esIzquierda ? -desplazamiento : desplazamiento, 0, 0);

            return true;
        }
        return false;
    }

    public void BotonRobarPresionado()
    {
        if (!juegoActivo || !esTurnoJugador) return;

        // 1. Solo puede robar si es su turno
        if (!esTurnoJugador) return;

        // 2. Robar del pozo
        if (pozo.Count > 0)
        {
            LogicaFicha fichaRobada = pozo[0];
            pozo.RemoveAt(0);

            // Añadir a la lista lógica
            manoJugador.Add(fichaRobada);

            // Configuración para el jugador
            GameObject fichaGO = fichaRobada.gameObject;
            fichaGO.GetComponent<SpriteRenderer>().enabled = true;

            // Importante: Volver a activar la interacción por si acaso
            if (fichaGO.GetComponent<FichaInteractiva>() == null)
                fichaGO.AddComponent<FichaInteractiva>();

            fichaGO.GetComponent<FichaInteractiva>().esDelJugador = true;
            fichaGO.GetComponent<BoxCollider2D>().enabled = true;

            // Posicionamiento visual en la mano
            ActualizarPosicionesManoJugador();

            Debug.Log("Has robado una ficha. Ahora tienes: " + manoJugador.Count);
        }
        else
        {
            Debug.Log("El pozo está vacío.");
            // Si el pozo está vacío y no tienes nada que jugar, 
            // podrías añadir aquí un botón de "Pasar Turno" manual.
        }
    }

    // Método para reordenar las fichas de la mano del jugador cuando roba una nueva ficha
    private void ActualizarPosicionesManoJugador()
    {
        for (int i = 0; i < manoJugador.Count; i++)
        {
            // Ponemos la ficha como hija del contenedor de la mano
            manoJugador[i].transform.SetParent(GameObject.Find("ManoJugador").transform);

            float espacio = 1.2f; 
            manoJugador[i].transform.localPosition = new Vector3(i * espacio, 0, 0);
            manoJugador[i].transform.rotation = Quaternion.identity; // Que estén verticales en la mano
        }
    }

    // Metodos para el final de la partida

    private void ComprobarGanador()
    {
        // 1. Victoria por quedarse sin fichas
        if (manoJugador.Count == 0)
        {
            FinalizarPartida("¡FELICIDADES! HAS GANADO");
        }
        else if (manoCPU.Count == 0)
        {
            FinalizarPartida("LA CPU HA GANADO... INTÉNTALO DE NUEVO");
        }
        // 2. Comprobar si el juego se ha trabado (nadie puede jugar y mazo vacío)
        else if (pozo.Count == 0 && !TieneJugadaPosible(manoJugador) && !TieneJugadaPosible(manoCPU))
        {
            ResolverTraba();
        }
    }

    private void ResolverTraba()
    {
        // En el dominó, si se traba, gana quien tenga menos puntos
        int puntosJugador = CalcularPuntos(manoJugador);
        int puntosCPU = CalcularPuntos(manoCPU);

        if (puntosJugador < puntosCPU)
            FinalizarPartida("JUEGO TRABADO: GANAS POR PUNTOS (" + puntosJugador + " vs " + puntosCPU + ")");
        else if (puntosCPU < puntosJugador)
            FinalizarPartida("JUEGO TRABADO: GANA CPU POR PUNTOS (" + puntosCPU + " vs " + puntosJugador + ")");
        else
            FinalizarPartida("¡EMPATE TÉCNICO!");
    }

    private int CalcularPuntos(List<LogicaFicha> mano)
    {
        int total = 0;
        foreach (LogicaFicha f in mano)
        {
            total += (f.ladoA + f.ladoB);
        }
        return total;
    }

    private void FinalizarPartida(string mensaje)
    {
        juegoActivo = false; // <-- Detenemos todo

        if (textoResultado != null)
        {
            textoResultado.text = mensaje;
            textoResultado.gameObject.SetActive(true);
            textoResultado.transform.SetAsLastSibling();
        }

        StopAllCoroutines(); 

        Debug.Log("Juego Detenido.");
    }
}
