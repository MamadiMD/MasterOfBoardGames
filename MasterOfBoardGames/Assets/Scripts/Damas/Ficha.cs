using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ficha : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public int tipoFicha;
    public bool esDama = false;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorOriginal = spriteRenderer.color;
    }

    public void SeleccionarVisualmente(bool estaSeleccionada)
    {
        if (estaSeleccionada)
            spriteRenderer.color = Color.yellow; 
        else
            spriteRenderer.color = colorOriginal; 
    }

    public void Coronar()
    {
        esDama = true;
        if (tipoFicha == 1) spriteRenderer.color = Color.cyan; 
        if (tipoFicha == 2) spriteRenderer.color = Color.magenta; 
        
        colorOriginal = spriteRenderer.color;
    }

}
