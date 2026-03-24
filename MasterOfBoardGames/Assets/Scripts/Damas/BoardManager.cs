using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    [Header("Prefabs de las Fichas")]
    public GameObject prefabFichaBlanca;
    public GameObject prefabFichaNegra;

    [Header("Ajustes del Tablero Visual")]
    [Tooltip("Coordenada de la casilla inferior izquierda (0,0)")]
    public Vector2 casillaOrigen = new Vector2(-3.5f, -3.5f); 
    [Tooltip("La distancia en unidades de Unity entre el centro de una casilla y la siguiente")]
    public float tamañoCasilla = 1.0f;

    // Nuestra matriz lógica: 0 = vacío, 1 = blanca, 2 = negra
    public int[,] logicBoard = new int[8, 8];

    [Header("Lógica de Juego")]
    public Ficha fichaSeleccionada;
    public bool turnoBlancas = true;

    public TextMeshProUGUI textoVictoria;
    private bool juegoTerminado = false;

    void Start()
    {
        GenerarTableroInicial();
    }

    void GenerarTableroInicial()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if ((x + y) % 2 == 0)
                {
                    if (y < 3)
                    {
                        InstanciarFicha(1, x, y);
                    }
                    else if (y > 4)
                    {
                        InstanciarFicha(2, x, y);
                    }
                }
            }
        }
    }

    void InstanciarFicha(int tipoFicha, int x, int y)
    {
        logicBoard[x, y] = tipoFicha;

        GameObject prefabAInstanciar = (tipoFicha == 1) ? prefabFichaBlanca : prefabFichaNegra;

        float posX = casillaOrigen.x + (x * tamañoCasilla);
        float posY = casillaOrigen.y + (y * tamañoCasilla);
        Vector2 posicionMundo = new Vector2(posX, posY);

        GameObject nuevaFicha = Instantiate(prefabAInstanciar, posicionMundo, Quaternion.identity);
        nuevaFicha.transform.parent = this.transform; 

        Ficha scriptFicha = nuevaFicha.GetComponent<Ficha>();
        scriptFicha.gridX = x;
        scriptFicha.gridY = y;
        scriptFicha.tipoFicha = tipoFicha;
    }

    void Update()
    {
        if (!turnoBlancas) return;

        if (juegoTerminado) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 posicionRaton = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(posicionRaton, Vector2.zero);

            if (hit.collider != null)
            {
                // Si tocamos una ficha
                Ficha fichaTocada = hit.collider.GetComponent<Ficha>();
                if (fichaTocada != null)
                {
                    IntentarSeleccionarFicha(fichaTocada);
                }
            }
            else
            {
                // Si NO tocamos una ficha (hicimos clic en una zona vacía)
                if (fichaSeleccionada != null)
                {
                    // Calculamos a qué casilla de la matriz corresponde el clic del ratón
                    int destinoX = Mathf.RoundToInt((posicionRaton.x - casillaOrigen.x) / tamañoCasilla);
                    int destinoY = Mathf.RoundToInt((posicionRaton.y - casillaOrigen.y) / tamañoCasilla);

                    // Nos aseguramos de que el clic esté dentro de los límites del tablero (0 a 7)
                    if (destinoX >= 0 && destinoX < 8 && destinoY >= 0 && destinoY < 8)
                    {
                        IntentarMoverFicha(destinoX, destinoY);
                    }
                }
            }
        }
    }


    public void IntentarMoverFicha(int x, int y)
    {
        if (logicBoard[x, y] != 0) return; // La casilla destino debe estar vacía

        int distanciaX = Mathf.Abs(x - fichaSeleccionada.gridX);
        int distanciaY = y - fichaSeleccionada.gridY;

        bool movimientoValido = false;
        bool esCaptura = false;
        Ficha fichaCapturada = null;

        // Comprobamos si va en la direccion correcta
        bool direccionCorrecta = false;
        
        if (fichaSeleccionada.esDama) {
            direccionCorrecta = true; // Las damas pueden moverse a donde sea
        } else if (fichaSeleccionada.tipoFicha == 1 && distanciaY > 0) {
            direccionCorrecta = true; // Blancas suben
        } else if (fichaSeleccionada.tipoFicha == 2 && distanciaY < 0) {
            direccionCorrecta = true; // Negras bajan
        }

        // Aqui validamos el movimiento es decir si vamos realizar un movimiento normal(1 casilla) o capturar una ficha(2 casillas)
        if (direccionCorrecta)
        {
            // Movimiento normal (1 paso en diagonal)
            if (distanciaX == 1 && Mathf.Abs(distanciaY) == 1)
            {
                movimientoValido = true;
            }
            // Captura (2 pasos en diagonal)
            else if (distanciaX == 2 && Mathf.Abs(distanciaY) == 2)
            {
                int medioX = (fichaSeleccionada.gridX + x) / 2;
                int medioY = (fichaSeleccionada.gridY + y) / 2;

                // Buscamos qué ficha hay en la casilla intermedia
                Ficha posibleEnemigo = ObtenerFichaEn(medioX, medioY);
                
                // Si hay una ficha y NO es de nuestro equipo, la podemos comer
                if (posibleEnemigo != null && posibleEnemigo.tipoFicha != fichaSeleccionada.tipoFicha)
                {
                    movimientoValido = true;
                    esCaptura = true;
                    fichaCapturada = posibleEnemigo;
                }
            }
        }

        if (fichaSeleccionada == null) return;
        if (logicBoard[x, y] != 0) return;

        // Aqui realizamos el movimiento
        if (movimientoValido)
        {
            // Si hemos comido, destruimos la ficha enemiga
            if (esCaptura && fichaCapturada != null)
            {
                logicBoard[fichaCapturada.gridX, fichaCapturada.gridY] = 0;
                Destroy(fichaCapturada.gameObject);
            }

            // Actualizamos la matriz
            logicBoard[fichaSeleccionada.gridX, fichaSeleccionada.gridY] = 0; 
            logicBoard[x, y] = fichaSeleccionada.tipoFicha; 

            // Actualizamos los datos de la ficha
            fichaSeleccionada.gridX = x;
            fichaSeleccionada.gridY = y;

            // Movemos el sprite visualmente
            float nuevaPosX = casillaOrigen.x + (x * tamañoCasilla);
            float nuevaPosY = casillaOrigen.y + (y * tamañoCasilla);
            fichaSeleccionada.transform.position = new Vector2(nuevaPosX, nuevaPosY);

            // Aqui coronamos a la dama
            // Si es blanca y llega a la fila 7, o es negra y llega a la fila 0
            if (fichaSeleccionada.tipoFicha == 1 && y == 7 && !fichaSeleccionada.esDama)
            {
                fichaSeleccionada.Coronar();
            }
            else if (fichaSeleccionada.tipoFicha == 2 && y == 0 && !fichaSeleccionada.esDama)
            {
                fichaSeleccionada.Coronar();
            }

            // Aqui soltamos la ficha y cambiamos turno
            fichaSeleccionada.SeleccionarVisualmente(false);
            fichaSeleccionada = null;
            turnoBlancas = !turnoBlancas;

            VerificarVictoria();

            if (!juegoTerminado) {
                if (!turnoBlancas) Invoke("LlamarIA", 1.0f);
            }
        }
        else
        {
            Debug.LogWarning("Movimiento rechazado: Valido=" + movimientoValido + " Dirección=" + direccionCorrecta);
        
            // Si es el turno de la IA y falló, devolvemos el turno al jugador para no bloquear
            if (!turnoBlancas) {
                turnoBlancas = true;
                fichaSeleccionada = null;
            }
        }
    }


    void IntentarSeleccionarFicha(Ficha ficha)
    {
        bool esFichaPropia = (turnoBlancas && ficha.tipoFicha == 1) || (!turnoBlancas && ficha.tipoFicha == 2);

        if (esFichaPropia)
        {
            if (fichaSeleccionada != null)
            {
                fichaSeleccionada.SeleccionarVisualmente(false);
            }

            fichaSeleccionada = ficha;
            fichaSeleccionada.SeleccionarVisualmente(true);
            
            Debug.Log("Ficha seleccionada en: " + ficha.gridX + ", " + ficha.gridY);
        }
    }

    Ficha ObtenerFichaEn(int x, int y)
    {
        // Busca todas las fichas que están agrupadas dentro de este BoardManager
        Ficha[] todasLasFichas = GetComponentsInChildren<Ficha>();
        foreach (Ficha f in todasLasFichas)
        {
            if (f.gridX == x && f.gridY == y)
            {
                return f;
            }
        }
        return null;
    }

    void LlamarIA() {
        GetComponent<CpuDamas>().RealizarTurnoCPU();
    }

    // Esta funcion comprueba si los movimientos que realiza la Cpu cuando esta haciendo sus pruebas de movimiento son posibles
    public bool EsMovimientoValidoParaIA(Ficha f, int x, int y, bool soloCaptura)
    {
        // 1. Límites del tablero
        if (x < 0 || x > 7 || y < 0 || y > 7) return false;
        
        // 2. ¿La casilla de destino está ocupada?
        if (logicBoard[x, y] != 0) return false;
    
        int distX = Mathf.Abs(x - f.gridX);
        int distY = y - f.gridY;
    
        if (soloCaptura)
        {
            // Para capturar, la distancia debe ser exactamente 2 en X y 2 en Y
            if (distX == 2 && Mathf.Abs(distY) == 2)
            {
                int mx = (f.gridX + x) / 2;
                int my = (f.gridY + y) / 2;
                
                // Comprobamos si hay una ficha enemiga en medio
                int fichaEnMedio = logicBoard[mx, my];
                if (fichaEnMedio != 0 && fichaEnMedio != f.tipoFicha)
                {
                    return true;
                }
            }
        }
        else
        {
            // Movimiento normal: distancia de 1
            if (distX == 1)
            {
                // Si es Dama, puede ir en cualquier dirección vertical
                if (f.esDama && Mathf.Abs(distY) == 1) return true;
                
                // Si es ficha negra normal, SOLO puede bajar (distY debe ser -1)
                if (f.tipoFicha == 2 && distY == -1) return true;
            }
        }
        
        return false;
    }

    void VerificarVictoria()
    {
        int blancas = 0;
        int negras = 0;

        // Recorremos la matriz lógica para contar
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (logicBoard[x, y] == 1) blancas++;
                if (logicBoard[x, y] == 2) negras++;
            }
        }

        // DEBUG: Para ver qué pasa en la matriz
        Debug.Log("Matriz dice -> Blancas: " + blancas + " Negras: " + negras);

        // IMPORTANTE: Solo disparamos la victoria si el juego NO está vacío.
        if (blancas == 0 || negras == 0)
        {
            if (blancas == 0 && negras == 0) return; 

            string mensajeResultado;
            if (blancas > 0)
            {
                mensajeResultado = "¡GANA EL JUGADOR!";
                GameManager.instance.GanaJugador();
            }
            else
            {
                mensajeResultado = "¡GANA LA CPU!";
                GameManager.instance.GanaCPU();
            }

            FinalizarPartida(mensajeResultado);
        }
    }

    void FinalizarPartida(string mensaje)
    {
        juegoTerminado = true;
        textoVictoria.gameObject.SetActive(true);
        textoVictoria.text = mensaje;

        Invoke("CambiarEscena",3.0f);
    }

    void CambiarEscena()
    {
        SceneManager.LoadScene(1);
    }
}
