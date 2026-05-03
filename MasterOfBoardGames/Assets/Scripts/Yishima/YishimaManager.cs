using System.Collections.Generic;
using UnityEngine;

public class YishimaManager : MonoBehaviour
{
    public GameObject prefabFicha;
    public Sprite spriteJ1; 
    public Sprite spriteJ2; 
    public List<Nodo> todosLosNodos;

    [Header("Estado del Juego")]
    public PiezaYishima piezaSeleccionada;
    public bool turnoJugador1 = true;

    void Start()
    {
        ColocarFichasIniciales();
    }

    void ColocarFichasIniciales()
    {
        // Necesitas decidir qué índices de tu lista 'todosLosNodos' son para cada uno
        CrearPieza(todosLosNodos[0], true);
        CrearPieza(todosLosNodos[1], true);
        CrearPieza(todosLosNodos[2], true);

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
        // Detectamos el clic izquierdo del ratón (o toque en móvil)
        if (Input.GetMouseButtonDown(0))
        {
            DetectarClic();
        }
    }

    void DetectarClic()
    {
        // Lanzamos un rayo desde la cámara a la posición del ratón
        Debug.Log("Has hecho clic en la pantalla");
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Has tocado un objeto llamado: " + hit.collider.name);
            // 1. ¿Tocamos una pieza?
            PiezaYishima pieza = hit.collider.GetComponent<PiezaYishima>();
            if (pieza != null)
            {
                // Solo podemos seleccionar si es el turno del dueño de la pieza
                if (pieza.esJugador1 == turnoJugador1)
                {
                    piezaSeleccionada = pieza;
                    Debug.Log("Pieza seleccionada: " + (turnoJugador1 ? "Blanca" : "Negra"));
                }
                return;
            }

            // 2. ¿Tocamos un nodo vacío teniendo una pieza ya seleccionada?
            Nodo nodo = hit.collider.GetComponent<Nodo>();
            if (nodo != null && piezaSeleccionada != null)
            {
                IntentarMovimiento(nodo);
            }
        }
    }

    void IntentarMovimiento(Nodo destino)
    {
        // REGLA DE ORO DEL YISHIMA: 
        // Debe estar vacío Y ser vecino directo (conectado por una línea)
        if (!destino.ocupado && piezaSeleccionada.nodoActual.vecinos.Contains(destino))
        {
            piezaSeleccionada.MoverA(destino);

            ComprobarVictoria();

            piezaSeleccionada = null; // Deseleccionamos
            turnoJugador1 = !turnoJugador1; // Cambio de turno
            Debug.Log("Turno de: " + (turnoJugador1 ? "Jugador 1" : "Jugador 2"));
        }
        else
        {
            Debug.LogWarning("Movimiento no permitido: El nodo no es vecino o está ocupado.");
        }
    }

    void ComprobarVictoria()
    {
        // El nodo central es el índice 8 según tu Inspector
        Nodo centro = todosLosNodos[8];
    
        // Si el centro está vacío, nadie ha ganado aún
        if (!centro.ocupado || centro.piezaActual == null) return;
    
        // Obtenemos quién es el dueño de la pieza del centro
        bool esJ1EnCentro = centro.piezaActual.GetComponent<PiezaYishima>().esJugador1;
    
        // Comprobamos las 4 líneas diagonales/horizontales que pasan por el centro
        // Usamos las parejas opuestas según tu orden en el Inspector
        
        if (VerificarLinea(0, 4, esJ1EnCentro)) return; // Pareja Nodo_1 y Nodo_8
        if (VerificarLinea(1, 5, esJ1EnCentro)) return; // Pareja Nodo_2 y Nodo_5
        if (VerificarLinea(2, 6, esJ1EnCentro)) return; // Pareja Nodo_3 y Nodo_6
        if (VerificarLinea(3, 7, esJ1EnCentro)) return; // Pareja Nodo_4 y Nodo_7
    }
    
    bool VerificarLinea(int indexA, int indexB, bool esJ1EnCentro)
    {
        Nodo nodoA = todosLosNodos[indexA];
        Nodo nodoB = todosLosNodos[indexB];
    
        if (nodoA.ocupado && nodoB.ocupado)
        {
            bool dueñoA = nodoA.piezaActual.GetComponent<PiezaYishima>().esJugador1;
            bool dueñoB = nodoB.piezaActual.GetComponent<PiezaYishima>().esJugador1;
    
            // Si los tres son del mismo jugador... ¡Victoria!
            if (dueñoA == esJ1EnCentro && dueñoB == esJ1EnCentro)
            {
                FinalizarJuego(esJ1EnCentro);
                return true;
            }
        }
        return false;
    }

// Función auxiliar para no repetir código
bool CompararDueño(Nodo n, bool dueñoCentro)
{
    if (!n.ocupado) return false;
    return n.piezaActual.GetComponent<PiezaYishima>().esJugador1 == dueñoCentro;
}

    bool HayLinea(Nodo n1, Nodo n2, Nodo n3)
    {
        if (n1.ocupado && n2.ocupado && n3.ocupado)
        {
            bool p1 = n1.piezaActual.GetComponent<PiezaYishima>().esJugador1;
            bool p2 = n2.piezaActual.GetComponent<PiezaYishima>().esJugador1;
            bool p3 = n3.piezaActual.GetComponent<PiezaYishima>().esJugador1;

            return (p1 == p2 && p2 == p3);
        }
        return false;
    }

    void FinalizarJuego(bool ganoJ1)
    {
        Debug.Log("¡EL GANADOR ES EL JUGADOR " + (ganoJ1 ? "1 (Blanco)" : "2 (Negro)") + "!");
        
    }
}
