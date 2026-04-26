using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carta : MonoBehaviour
{
    public enum Palo { Oros, Copas, Espadas, Bastos }
    
    public bool esDelJugador = false;
    
    [Header("Atributos")]
    public Palo palo;
    public int numero; 
    public int valorPuntos; 
    
    [Header("Sprites")]
    public Sprite imagenFrontal;
    public Sprite imagenTrasera;
    
    private SpriteRenderer sr;
    private bool esVisible = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Configurar(Sprite frontal, Sprite trasera, Palo p, int n, int puntos)
    {
        imagenFrontal = frontal;
        imagenTrasera = trasera;
        palo = p;
        numero = n;
        valorPuntos = puntos;
        MostrarLado(false); // Por defecto boca abajo
    }

    public void MostrarLado(bool mostrarFrontal)
    {
        esVisible = mostrarFrontal;
        sr.sprite = mostrarFrontal ? imagenFrontal : imagenTrasera;
    }
}
