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
            case "Reina_Negra": this.GetComponent<SpriteRenderer>().sprite = Reina_Negra; player = "negro";break;
            case "Rey_Negro": this.GetComponent<SpriteRenderer>().sprite = Rey_Negro; player = "negro";break;
            case "Alfil_Negro": this.GetComponent<SpriteRenderer>().sprite = Alfil_Negro; player = "negro";break;
            case "Torre_Negra": this.GetComponent<SpriteRenderer>().sprite = Torre_Negra; player = "negro";break;
            case "Caballo_Negro": this.GetComponent<SpriteRenderer>().sprite = Caballo_Negro; player = "negro";break;
            case "Peon_Negro": this.GetComponent<SpriteRenderer>().sprite = Peon_Negro; player = "negro";break;

            case "Reina_Blanca": this.GetComponent<SpriteRenderer>().sprite = Reina_Blanca; player = "blanco";break;
            case "Rey_Blanco": this.GetComponent<SpriteRenderer>().sprite = Rey_Blanco; player = "blanco";break;
            case "Alfil_Blanco": this.GetComponent<SpriteRenderer>().sprite = Alfil_Blanco; player = "blanco";break;
            case "Torre_Blanca": this.GetComponent<SpriteRenderer>().sprite = Torre_Blanca; player = "blanco";break;
            case "Caballo_Blanco": this.GetComponent<SpriteRenderer>().sprite = Caballo_Blanco; player = "blanco";break;
            case "Peon_Blanco": this.GetComponent<SpriteRenderer>().sprite = Peon_Blanco; player = "blanco";break;
        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x*= 1.13f;
        y*= 1.1f;

        x += -3.95f;
        y += -3.8f;

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

    private void OnMouseUp()
    {
        DestroyMovePlates();

        InitiateMovePlates();
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");

        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "Reina_Negra":
            case "Reina_Blanca":
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(1,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                LineMovePlate(-1,-1);
                LineMovePlate(1,-1);
                LineMovePlate(-1,1);
                break;
            case "Caballo_Negro":
            case "Caballo_Blanco":
                LMovePlate();
                break;
            case "Alfil_Negro":
            case "Alfil_Blanco":
                LineMovePlate(1,1);
                LineMovePlate(-1,1);
                LineMovePlate(1,-1);
                LineMovePlate(-1,-1);
                break;
            case "Rey_Negro":
            case "Rey_Blanco":
                SurroundMovePlate();
                break;
            case "Torre_Negra":
            case "Torre_Blanca":
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                break;
            case "Peon_Negro":
                PawnMovePlate(xBoard,yBoard-1);
                break;
            case "Peon_Blanco":
                PawnMovePlate(xBoard,yBoard+1);
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y) == null)
        {
            MovePlateSpawn(x,y);

            x += xIncrement;
            y += yIncrement;
        }

        if(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y).GetComponent<Chessman>().player != this.player)
        {
            MovePlateAttackSpawn(x,y);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);

    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != this.player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
             if(sc.GetPosition(x,y) == null)
             {
                MovePlateSpawn(x, y );
             }

             if (sc.GetPosition(x + 1, y) && sc.GetPosition(x + 1, y) != null && 
             sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != this.player)
             {
                 MovePlateAttackSpawn(x + 1, y);
             }

             if (sc.GetPosition(x - 1, y) && sc.GetPosition(x - 1, y) != null && 
             sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != this.player)
             {
                 MovePlateAttackSpawn(x - 1, y);
             }

        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 1.13f;
        y *= 1.1f;

        x += -3.95f;
        y += -3.8f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(this.gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 1.13f;
        y *= 1.1f;

        x += -3.95f;
        y += -3.8f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(this.gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}
