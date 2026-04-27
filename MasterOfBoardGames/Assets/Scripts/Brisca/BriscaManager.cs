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

    [Header("Marcadores")]
    public int puntosJugador;
    public int puntosCPU;
    public TMPro.TextMeshProUGUI textoPuntosJugador;
    public TMPro.TextMeshProUGUI textoPuntosCPU;
    public TMPro.TextMeshProUGUI textoContadorMazo;
    public TMPro.TextMeshProUGUI textoAvisoBaza;

    [Header("Final de Partida")]
    public GameObject panelFinal;
    public TMPro.TextMeshProUGUI textoResultadoFinal;
    private bool juegoTerminado = false;

    private string nombrePaloTriunfo;

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
        nombrePaloTriunfo = cartaTriunfo.palo.ToString();
        
        triunfoGO.transform.position = triunfoPos.position;
        triunfoGO.transform.rotation = triunfoPos.rotation;
        cartaTriunfo.MostrarLado(true); // El triunfo siempre se ve
    }

    public void DarCarta(bool esJugador)
    {
        Carta cartaARobar = null;

        if (baraja.mazo.Count > 0)
        {
            GameObject cartaGO = baraja.mazo[0];
            baraja.mazo.RemoveAt(0);
            cartaARobar = cartaGO.GetComponent<Carta>();
        }
        else if (cartaTriunfo != null)
        {
            // Si no hay mazo, el siguiente se lleva el triunfo de la mesa
            cartaARobar = cartaTriunfo;
            cartaTriunfo = null; 
            Debug.Log("¡Se ha robado el Triunfo!");
        }

        if (cartaARobar != null)
        {
            cartaARobar.esDelJugador = esJugador;

            if (esJugador)
            {
                manoJugador.Add(cartaARobar);
                cartaARobar.MostrarLado(true);
            }
            else
            {
                manoCPU.Add(cartaARobar);
                cartaARobar.MostrarLado(false);
            }
            ActualizarVisualMano(esJugador);
        }

        ActualizarContadorMazo();

        // Comprobar si ya no quedan cartas ni en manos ni en mazo
        ComprobarFinalPartida();
    }

    void ComprobarFinalPartida()
    {
        Debug.Log($"Check Final: Mazo({baraja.mazo.Count}) Triunfo({(cartaTriunfo == null ? "null" : "si")}) ManoJ({manoJugador.Count}) ManoCPU({manoCPU.Count})");

        // En la Brisca, la partida termina cuando nadie tiene cartas en la mano
        if (manoJugador.Count == 0 && manoCPU.Count == 0)
        {
            if (juegoTerminado) return; 

            juegoTerminado = true;

            if (panelFinal != null)
            {
                panelFinal.SetActive(true);

                if (puntosJugador > puntosCPU)
                {
                    textoResultadoFinal.text = "¡VICTORIA!\n" + "Gana el Jugador con\n" +puntosJugador + " Puntos";
                    textoResultadoFinal.color = Color.green;
                }
                else if (puntosCPU > puntosJugador)
                {
                    textoResultadoFinal.text = "DERROTA\n" + "Gana la CPU con\n" +puntosCPU + " Puntos";
                    textoResultadoFinal.color = Color.red;
                }
                else
                {
                    textoResultadoFinal.text = "EMPATE\n60 - 60";
                }
            }
        }
    }

    void ActualizarContadorMazo()
    {
        if (textoContadorMazo != null)
        {
            // Sumamos las cartas que quedan por salir del mazo
            int cantidadMazo = baraja.mazo.Count;
            
            // Sumamos 1 si el triunfo todavía está físicamente en la mesa
            int cantidadTriunfo = (cartaTriunfo != null) ? 1 : 0;
    
            int totalRestante = cantidadMazo + cantidadTriunfo;
            
            textoContadorMazo.text = "Cartas restantes: " + totalRestante;
            
            // Opcional: Si no quedan cartas, ponerlo en rojo
            if(totalRestante == 0) textoContadorMazo.color = Color.red;
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
        if (!turnoJugador || cartaJugadorMesa != null) return;

        manoJugador.Remove(carta);

        cartaJugadorMesa = carta;
        carta.transform.position = anclaJugador.position;
        ActualizarVisualMano(true);

        // Si la CPU ya había lanzado su carta, entonces resolvemos
        if (cartaCPUMesa != null)
        {
            Invoke("ResolverBaza", 1.0f);
        }
        else // Si no, le toca a la CPU responder
        {
            turnoJugador = false;
            Invoke("TurnoCPU", 1.0f);
        }

        if (baraja.mazo.Count == 0 && cartaTriunfo == null) ComprobarFinalPartida();
    }

    void TurnoCPU()
    {
        if (manoCPU.Count == 0 || cartaCPUMesa != null) return;

        cartaCPUMesa = manoCPU[0];
        manoCPU.RemoveAt(0);

        cartaCPUMesa.transform.position = anclaCPU.position;
        cartaCPUMesa.MostrarLado(true);
        ActualizarVisualMano(false);

        // Si el jugador ya había lanzado su carta, resolvemos
        if (cartaJugadorMesa != null)
        {
            Invoke("ResolverBaza", 1.0f);
        }
        else // Si no, el jugador debe responder (esperamos a que arrastre su carta)
        {
            turnoJugador = true;
        }

        if (baraja.mazo.Count == 0 && cartaTriunfo == null) ComprobarFinalPartida();
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
        if (cartaJugadorMesa == null || cartaCPUMesa == null) 
        {
            Debug.LogWarning("Intentando resolver baza pero faltan cartas en la mesa.");
            return;
        }

        if (juegoTerminado) return;

        bool ganaJugador = false;
        string paloJ = cartaJugadorMesa.palo.ToString();
        string paloC = cartaCPUMesa.palo.ToString();
        int puntosEnJuego = cartaJugadorMesa.valorPuntos + cartaCPUMesa.valorPuntos;

        // Lógica de quién gana
        if (paloJ == paloC)
        {
            ganaJugador = ObtenerFuerza(cartaJugadorMesa) > ObtenerFuerza(cartaCPUMesa);
        }
        // USAMOS EL NOMBRE GUARDADO, NO LA CARTA
        else if (paloC.Equals(nombrePaloTriunfo))
        {
            ganaJugador = false; 
        }
        else if (paloJ.Equals(nombrePaloTriunfo))
        {
            ganaJugador = true;
        }
        else
        {
            ganaJugador = !turnoJugador; 
        }

        if (textoAvisoBaza != null)
        {
            textoAvisoBaza.text = ganaJugador ? "Gana el Jugador" : "Gana la CPU";
            textoAvisoBaza.gameObject.SetActive(true);
        }

        // Sumar puntos
        if (ganaJugador) {
            puntosJugador += puntosEnJuego;
            ActualizarInterfazPuntos();
        } else {
            puntosCPU += puntosEnJuego;
            ActualizarInterfazPuntos();
        }

        StartCoroutine(LimpiarMesaYRobar(ganaJugador));
    }

    void ActualizarInterfazPuntos()
    {
        textoPuntosJugador.text = "Puntos Jugador: " + puntosJugador;
        textoPuntosCPU.text = "Puntos CPU: " + puntosCPU;
    }

    IEnumerator LimpiarMesaYRobar(bool ganaJugador)
    {
        yield return new WaitForSeconds(2.0f); 

        if (textoAvisoBaza != null) 
        textoAvisoBaza.gameObject.SetActive(false);

        // 1. Guardamos la referencia localmente antes de limpiar las variables globales
        GameObject objJugador = cartaJugadorMesa.gameObject;
        GameObject objCPU = cartaCPUMesa.gameObject;

        // 2. Limpiamos las variables globales del Manager para que no haya conflictos
        cartaJugadorMesa = null;
        cartaCPUMesa = null;

        // 3. Desactivamos los objetos 
        objJugador.SetActive(false);
        objCPU.SetActive(false);

        // 4. Lógica de robo
        if (baraja.mazo.Count > 0 || cartaTriunfo != null)
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
            // Si no hay cartas para robar, simplemente pasamos el turno
            turnoJugador = ganaJugador;
            if (!turnoJugador) Invoke("TurnoCPU", 1.0f);
        }

        Destroy(objJugador);
        Destroy(objCPU);

        yield return new WaitForSeconds(0.5f);
    
        ComprobarFinalPartida();
    }
}
