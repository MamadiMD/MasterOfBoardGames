using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject pieza;

    private GameObject [,] position = new GameObject[8,8];
    private GameObject [] piezasNegras = new GameObject[16];
    private GameObject [] piezasBlancas = new GameObject[16];

    private string currentPlayer = "Blanco";

    private bool gameOver = false;

    void Start()
    {
        piezasBlancas = new GameObject[]
        {
            Create("Reina_Blanca", 3, 0),
            Create("Rey_Blanco", 4, 0),
            Create("Alfil_Blanco", 2, 0),
            Create("Alfil_Blanco", 5, 0),
            Create("Torre_Blanca", 0, 0),
            Create("Torre_Blanca", 7, 0),
            Create("Caballo_Blanco", 1, 0),
            Create("Caballo_Blanco", 6, 0),
            Create("Peon_Blanco", 0, 1),
            Create("Peon_Blanco", 1, 1),
            Create("Peon_Blanco", 2, 1),
            Create("Peon_Blanco", 3, 1),
            Create("Peon_Blanco", 4, 1),
            Create("Peon_Blanco", 5, 1),
            Create("Peon_Blanco", 6, 1),
            Create("Peon_Blanco", 7, 1)

        };

        piezasNegras = new GameObject[]
        {
            Create("Reina_Negra", 3, 7),
            Create("Rey_Negro", 4, 7), 
            Create("Alfil_Negro", 2, 7),
            Create("Alfil_Negro", 5, 7),
            Create("Torre_Negra", 0, 7),
            Create("Torre_Negra", 7, 7),
            Create("Caballo_Negro", 1, 7),
            Create("Caballo_Negro", 6, 7),
            Create("Peon_Negra", 0, 6),
            Create("Peon_Negra", 1, 6),
            Create("Peon_Negra", 2, 6),
            Create("Peon_Negra", 3, 6),
            Create("Peon_Negra", 4, 6),
            Create("Peon_Negra", 5, 6),
            Create("Peon_Negra", 6, 6),
            Create("Peon_Negra", 7, 6)
        };

        for(int i = 0 ; i < piezasNegras.Length ; i++)
        {
            SetPosition(piezasNegras[i]);
            SetPosition(piezasBlancas[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(pieza, new Vector3(0,0,-1), Quaternion.identity); 
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        position[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

}
