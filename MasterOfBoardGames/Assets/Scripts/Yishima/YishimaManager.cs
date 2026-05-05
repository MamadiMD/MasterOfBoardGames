using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YishimaManager : MonoBehaviour
{
    public GameObject prefabFicha;
    public Sprite spriteJ1; 
    public Sprite spriteJ2; 
    public List<Nodo> todosLosNodos;

    [Header("Estado del Juego")]
    public PiezaYishima piezaSeleccionada;
    public bool turnoJugador1 = true;
    private bool juegoTerminado = false;

    [Header("Configuración IA")]
    public bool contraCPU = true; // Activa/Desactiva la IA
    public float tiempoEsperaIA = 1.0f;

    [Header("UI Elementos")]
    public GameObject panelVictoria;
    public TMPro.TextMeshProUGUI textoResultado;

    void Start()
    {
        ColocarFichasIniciales();
    }

    void ColocarFichasIniciales()
    {
        // Jugador 1 (Blancas) en los primeros nodos
        CrearPieza(todosLosNodos[0], true);
        CrearPieza(todosLosNodos[1], true);
        CrearPieza(todosLosNodos[2], true);

        // Jugador 2 (Negras) en los nodos opuestos
        CrearPieza(todosLosNodos[5], false);
        CrearPieza(todosLosNodos[6], false);
        CrearPieza(todosLosNodos[7], false);
    }

    void CrearPieza(Nodo nodo, bool esJ1)
    {
        GameObject nuevaFicha = Instantiate(prefabFicha, nodo.transform.position, Quaternion.identity);
        PiezaYishima pieza = nuevaFicha.GetComponent<PiezaYishima>();
        
        pieza.esJugador1 = esJ1;
        SpriteRenderer sr = nuevaFicha.GetComponent<SpriteRenderer>();
        sr.sprite = esJ1 ? spriteJ1 : spriteJ2;

        pieza.MoverA(nodo);
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Bloqueamos clics si es turno de la IA
        if (contraCPU && !turnoJugador1) return;

        if (Input.GetMouseButtonDown(0))
        {
            DetectarClic();
        }
    }

    void DetectarClic()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            // 1. ¿Tocamos una pieza?
            PiezaYishima pieza = hit.collider.GetComponent<PiezaYishima>();
            if (pieza != null)
            {
                if (pieza.esJugador1 == turnoJugador1)
                {
                    piezaSeleccionada = pieza;
                    Debug.Log("Pieza seleccionada: " + (turnoJugador1 ? "Blanca" : "Negra"));
                }
                return;
            }

            // 2. ¿Tocamos un nodo?
            Nodo nodo = hit.collider.GetComponent<Nodo>();
            if (nodo != null && piezaSeleccionada != null)
            {
                IntentarMovimiento(nodo);
            }
        }
    }

    void IntentarMovimiento(Nodo destino)
    {
        if (!destino.ocupado && piezaSeleccionada.nodoActual.vecinos.Contains(destino))
        {
            piezaSeleccionada.MoverA(destino);
            ComprobarVictoria();

            if (!juegoTerminado)
            {
                piezaSeleccionada = null;
                turnoJugador1 = !turnoJugador1;

                // Si ahora es el turno de la IA (Jugador 2)
                if (contraCPU && !turnoJugador1)
                {
                    StartCoroutine(TurnoIA());
                }
            }
        }
    }

    void ComprobarVictoria()
    {
        Nodo centro = todosLosNodos[8];
        if (!centro.ocupado || centro.piezaActual == null) return;

        bool esJ1EnCentro = centro.piezaActual.GetComponent<PiezaYishima>().esJugador1;

        // Revisamos las 4 líneas que pasan por el centro (según tu Inspector)
        if (VerificarLinea(0, 5, esJ1EnCentro)) return; 
        if (VerificarLinea(1, 6, esJ1EnCentro)) return; 
        if (VerificarLinea(2, 7, esJ1EnCentro)) return; 
        if (VerificarLinea(4, 3, esJ1EnCentro)) return; 
    }
    
    bool VerificarLinea(int indexA, int indexB, bool esJ1EnCentro)
    {
        Nodo nodoA = todosLosNodos[indexA];
        Nodo nodoB = todosLosNodos[indexB];

        if (nodoA.ocupado && nodoB.ocupado)
        {
            bool dueñoA = nodoA.piezaActual.GetComponent<PiezaYishima>().esJugador1;
            bool dueñoB = nodoB.piezaActual.GetComponent<PiezaYishima>().esJugador1;
    
            if (dueñoA == esJ1EnCentro && dueñoB == esJ1EnCentro)
            {
                FinalizarJuego(esJ1EnCentro);
                return true;
            }
        }
        return false;
    }

    IEnumerator TurnoIA()
    {
        yield return new WaitForSeconds(tiempoEsperaIA);

        List<PiezaYishima> piezasIA = obtenerPiezas(false);

        // 1. Buscar movimientos (Prioridad: Movimiento válido aleatorio por ahora)

        bool movio = false;
        foreach (PiezaYishima p in piezasIA)
        {
            foreach (Nodo vecino in p.nodoActual.vecinos)
            {
                if (!vecino.ocupado)
                {
                    // La IA elige esta pieza y este nodo
                    piezaSeleccionada = p;
                    IntentarMovimiento(vecino);
                    movio = true;
                    break;
                }
            }
            if (movio) break;
        }
    }

    List<PiezaYishima> obtenerPiezas(bool esJ1)
    {
        List<PiezaYishima> lista = new List<PiezaYishima>();
        PiezaYishima[] todas = FindObjectsOfType<PiezaYishima>();
        foreach (PiezaYishima p in todas)
        {
            if (p.esJugador1 == esJ1) lista.Add(p);
        }
        return lista;
    }

    void FinalizarJuego(bool ganoJ1)
    {
        juegoTerminado = true;
        
        // Activamos el panel que estaba oculto
        panelVictoria.SetActive(true);

        if (ganoJ1)
        {
            textoResultado.text = "¡VICTORIA!\n" + "Ha ganado el Jugador";
            textoResultado.color = Color.green;
            StartCoroutine(GanaJugadorPartida());
            
        }
        else
        {
            textoResultado.text = "DERROTA...\n"+"Ha ganado la CPU";
            textoResultado.color = Color.red;
            StartCoroutine(GanaCpuPartida());
        }
    }

    IEnumerator GanaJugadorPartida()
    {
        GameManager.instance.GanaJugador();
        yield return new WaitForSeconds(6.0f);
        SceneManager.LoadScene(1);
        
    }

    IEnumerator GanaCpuPartida()
    {
        GameManager.instance.GanaCPU();
        yield return new WaitForSeconds(6.0f);
        SceneManager.LoadScene(1);
    }

}
