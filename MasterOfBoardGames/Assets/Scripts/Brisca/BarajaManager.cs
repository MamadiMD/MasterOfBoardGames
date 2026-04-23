using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarajaManager : MonoBehaviour
{
    public GameObject prefabCarta;
    public Sprite[] spritesOros, spritesCopas, spritesEspadas, spritesBastos;
    public Sprite reverso;

    public List<GameObject> mazo = new List<GameObject>();

    void Start()
    {
        CrearBaraja();
        Barajar();
    }

    void CrearBaraja()
    {
        GenerarPalo(Carta.Palo.Oros, spritesOros);
        GenerarPalo(Carta.Palo.Copas, spritesCopas);
        GenerarPalo(Carta.Palo.Espadas, spritesEspadas);
        GenerarPalo(Carta.Palo.Bastos, spritesBastos);
    }

    void GenerarPalo(Carta.Palo p, Sprite[] sprites)
    {
        for (int i = 0; i < 10; i++) 
        {
            GameObject nuevaCarta = Instantiate(prefabCarta);
            Carta scriptCarta = nuevaCarta.GetComponent<Carta>();
            
            int numeroReal = (i < 7) ? i + 1 : i + 3;
            int puntos = AsignarPuntos(numeroReal);
            
            scriptCarta.Configurar(sprites[i], reverso, p, numeroReal, puntos);
            mazo.Add(nuevaCarta);
            nuevaCarta.transform.position = new Vector3(20, 0, 0); 
        }
    }

    int AsignarPuntos(int n)
    {
        switch (n) {
            case 1: return 11; 
            case 3: return 10; 
            case 12: return 4; 
            case 11: return 3; 
            case 10: return 2; 
            default: return 0; 
        }
    }

    void Barajar()
    {
        for (int i = 0; i < mazo.Count; i++)
        {
            GameObject temp = mazo[i];
            int randomIndex = Random.Range(i, mazo.Count);
            mazo[i] = mazo[randomIndex];
            mazo[randomIndex] = temp;
        }
    }
}
