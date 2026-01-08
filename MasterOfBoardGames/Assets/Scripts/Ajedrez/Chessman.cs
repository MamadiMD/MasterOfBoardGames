using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;

    private string player;

    public Sprite Reina_Negra , Rey_Negro , Alfil_Negro, Torre_Negra, Caballo_Negro , Peon_Negro;
    public Sprite Reina_Blanca , Rey_Blanco , Alfil_Blanco, Torre_Blanca, Caballo_Blanco , Peon_Blanco;

    public void Activate(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        
        SetCoords();

        switch (this.name)
        {
            case "Reina_Negra": this.GetComponent<SpriteRenderer>().sprite = Reina_Negra; break;
            case "Rey_Negro": this.GetComponent<SpriteRenderer>().sprite = Rey_Negro; break;
            case "Alfil_Negro": this.GetComponent<SpriteRenderer>().sprite = Alfil_Negro; break;
            case "Torre_Negra": this.GetComponent<SpriteRenderer>().sprite = Torre_Negra; break;
            case "Caballo_Negro": this.GetComponent<SpriteRenderer>().sprite = Caballo_Negro; break;
            case "Peon_Negro": this.GetComponent<SpriteRenderer>().sprite = Peon_Negro; break;

            case "Reina_Blanca": this.GetComponent<SpriteRenderer>().sprite = Reina_Blanca; break;
            case "Rey_Blanco": this.GetComponent<SpriteRenderer>().sprite = Rey_Blanco; break;
            case "Alfil_Blanco": this.GetComponent<SpriteRenderer>().sprite = Alfil_Blanco; break;
            case "Torre_Blanca": this.GetComponent<SpriteRenderer>().sprite = Torre_Blanca; break;
            case "Caballo_Blanco": this.GetComponent<SpriteRenderer>().sprite = Caballo_Blanco; break;
            case "Peon_Blanco": this.GetComponent<SpriteRenderer>().sprite = Peon_Blanco; break;
        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x*= 0.66f;
        y*= 0.66f;

        x += -2.3f;
        y += -2.3f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }


}
