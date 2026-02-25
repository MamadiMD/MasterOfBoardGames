using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    void IntentarMoverFicha(int x, int y)
    {
        // 1. Comprobar que la casilla de destino está vacía
        if (logicBoard[x, y] != 0)
        {
            return; // Si no está vacía, no hacemos nada
        }

        // 2. Calcular cuántas casillas nos estamos moviendo
        int distanciaX = Mathf.Abs(x - fichaSeleccionada.gridX); // Absoluto porque puede ser izquierda o derecha
        int distanciaY = y - fichaSeleccionada.gridY; // Sin absoluto porque importa si va hacia arriba o abajo

        bool movimientoValido = false;

        // Reglas para las Blancas (tipoFicha == 1): Se mueven 1 paso en X y +1 paso en Y (hacia arriba)
        if (fichaSeleccionada.tipoFicha == 1 && distanciaX == 1 && distanciaY == 1)
        {
            movimientoValido = true;
        }
        // Reglas para las Negras (tipoFicha == 2): Se mueven 1 paso en X y -1 paso en Y (hacia abajo)
        else if (fichaSeleccionada.tipoFicha == 2 && distanciaX == 1 && distanciaY == -1)
        {
            movimientoValido = true;
        }

        // Si el movimiento cumple las reglas, ejecutamos el traslado
        if (movimientoValido)
        {
            // A. Actualizamos la matriz lógica
            logicBoard[fichaSeleccionada.gridX, fichaSeleccionada.gridY] = 0; // Vaciamos la casilla antigua
            logicBoard[x, y] = fichaSeleccionada.tipoFicha; // Llenamos la casilla nueva

            // B. Actualizamos los datos internos de la ficha
            fichaSeleccionada.gridX = x;
            fichaSeleccionada.gridY = y;

            // C. Movemos el sprite visualmente en Unity
            float nuevaPosX = casillaOrigen.x + (x * tamañoCasilla);
            float nuevaPosY = casillaOrigen.y + (y * tamañoCasilla);
            fichaSeleccionada.transform.position = new Vector2(nuevaPosX, nuevaPosY);

            // D. Soltamos la ficha y cambiamos de turno
            fichaSeleccionada.SeleccionarVisualmente(false);
            fichaSeleccionada = null;
            turnoBlancas = !turnoBlancas; // Cambiamos de true a false, o de false a true
            
            Debug.Log("Movimiento realizado. Ahora es el turno de las " + (turnoBlancas ? "Blancas" : "Negras"));
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

}
