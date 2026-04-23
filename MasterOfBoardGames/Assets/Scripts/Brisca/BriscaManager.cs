using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriscaManager : MonoBehaviour
{
    public static BriscaManager Instance;

    [Header("Referencias de Escena")]
    public BarajaManager baraja;
    public Transform mazoPos, triunfoPos, manoJugadorPos, manoCPUPos;

    [Header("Listas de Juego")]
    public List<Carta> manoJugador = new List<Carta>();
    public List<Carta> manoCPU = new List<Carta>();
    public Carta cartaTriunfo;

    [Header("Tablero de Juego")]
    public Carta cartaJugadorMesa;
    public Carta cartaCPUMesa;
    public Transform anclaJugador, anclaCPU;
    public bool turnoJugador = true;

    public int puntosJugador;
    public int puntosCPU;

    void Awake() => Instance = this;

    void Start()
    {
        // Esperamos un momento a que la baraja se cree y se mezcle
        Invoke("RepartoInicial", 0.5f);
    }

    void RepartoInicial()
    {
        // 1. Ponemos el mazo visualmente en su sitio
        foreach (GameObject go in baraja.mazo)
        {
            go.transform.position = mazoPos.position;
            go.transform.rotation = mazoPos.rotation;
        }

        // 2. Repartir 3 al jugador y 3 a la CPU
        for (int i = 0; i < 3; i++)
        {
            DarCarta(true);  // Jugador
            DarCarta(false); // CPU
        }

        // 3. Sacar el Triunfo
        GameObject triunfoGO = baraja.mazo[0];
        baraja.mazo.RemoveAt(0);
        cartaTriunfo = triunfoGO.GetComponent<Carta>();
        
        triunfoGO.transform.position = triunfoPos.position;
        triunfoGO.transform.rotation = triunfoPos.rotation;
        cartaTriunfo.MostrarLado(true); // El triunfo siempre se ve
    }

    public void DarCarta(bool esJugador)
    {
        if (baraja.mazo.Count == 0) return;

        GameObject cartaGO = baraja.mazo[0];
        baraja.mazo.RemoveAt(0);
        Carta cartaScript = cartaGO.GetComponent<Carta>();

        if (esJugador)
        {
            manoJugador.Add(cartaScript);
            cartaScript.MostrarLado(true); // Tú ves tus cartas
            ActualizarVisualMano(true);
        }
        else
        {
            manoCPU.Add(cartaScript);
            cartaScript.MostrarLado(false); // No ves las de la CPU
            ActualizarVisualMano(false);
        }
    }

    void ActualizarVisualMano(bool esJugador)
    {
        List<Carta> mano = esJugador ? manoJugador : manoCPU;
        Transform puntoRaiz = esJugador ? manoJugadorPos : manoCPUPos;

        for (int i = 0; i < mano.Count; i++)
        {
            // Separamos las cartas un poco para que no estén una encima de otra
            float offset = i * 1.5f; 
            mano[i].transform.position = puntoRaiz.position + new Vector3(offset, 0, 0);
        }
    }

    public void JugadorLanzaCarta(Carta carta)
    {
        if (!turnoJugador) return;

        cartaJugadorMesa = carta;
        manoJugador.Remove(carta);

        carta.transform.position = anclaJugador.position;

        ActualizarVisualMano(true);
        turnoJugador = false;

        Invoke("TurnoCPU", 1.0f);
    }

    void TurnoCPU()
    {
        if (manoCPU.Count == 0) return;

        // La CPU elige su primera carta
        cartaCPUMesa = manoCPU[0];
        manoCPU.RemoveAt(0);

        // Mover al ancla y mostrarla
        cartaCPUMesa.transform.position = anclaCPU.position;
        cartaCPUMesa.MostrarLado(true);

        ActualizarVisualMano(false);

        // Tras un segundo, decidimos quién gana
        Invoke("ResolverBaza", 1.5f);
    }

    int ObtenerFuerza(Carta carta)
    {
        switch (carta.numero)
        {
            case 1: return 12; 
            case 3: return 11; 
            case 12: return 10;
            case 11: return 9;
            case 10: return 8;
            default: return carta.numero; 
        }
    }

    void ResolverBaza()
    {
        bool ganaJugador = false;

        // Caso 1: Son del mismo palo
        if (cartaJugadorMesa.palo == cartaCPUMesa.palo)
        {
            ganaJugador = ObtenerFuerza(cartaJugadorMesa) > ObtenerFuerza(cartaCPUMesa);
        }
        // Caso 2: Palos distintos, pero la CPU tiró Triunfo
        else if (cartaCPUMesa.palo == cartaTriunfo.palo)
        {
            ganaJugador = false; 
        }
        // Caso 3: Palos distintos, pero el Jugador tiró Triunfo 
        else if (cartaJugadorMesa.palo == cartaTriunfo.palo)
        {
            ganaJugador = true;
        }
        // Caso 4: Palos distintos y ninguno es triunfo 
        else
        {
            // Si el jugador empezó la mano, gana él. Si empezó la CPU, gana ella.
            ganaJugador = true; 
        }

        Debug.Log(ganaJugador ? "¡Ganas la baza!" : "La CPU gana la baza.");

        int suma = cartaJugadorMesa.valorPuntos + cartaCPUMesa.valorPuntos;

        if (!ganaJugador)
        {
            puntosCPU += suma;
        }
        else
        {
            puntosJugador += suma;
        }

        // Limpiar la mesa y repartir puntos 
        StartCoroutine(LimpiarMesaYRobar(ganaJugador));
    }

    IEnumerator LimpiarMesaYRobar(bool ganaJugador)
    {
        yield return new WaitForSeconds(2.0f); // Pausa para que el jugador vea qué pasó

        Destroy(cartaJugadorMesa.gameObject);
        Destroy(cartaCPUMesa.gameObject);

        if (baraja.mazo.Count > 0)
        {
            if (ganaJugador)
            {
                DarCarta(true);
                DarCarta(false);
                turnoJugador = true;
            }
            else
            {
                DarCarta(false);
                DarCarta(true);
                turnoJugador = false;
                Invoke("TurnoCPU", 1.0f);
            }
        }
        else
        {
            // Si no hay mazo, solo se cambia el turno
            turnoJugador = ganaJugador;
            if (!turnoJugador) Invoke("TurnoCPU", 1.0f);
        }
    }
}
