using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ficha : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public int tipoFicha;

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


}
