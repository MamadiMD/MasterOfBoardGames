using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repartidor : MonoBehaviour
{
    [Header("Prefabs de Fichas")]
    public List<GameObject> prefabsFichasDominó = new List<GameObject>();

    [Header("Configuración de Escena")]
    public Transform contenedorManoJugador;
    public float espacioEntreFichasMano = 1.2f;

    private List<GameObject> instanciasFichas = new List<GameObject>();

    void Start()
    {
        if (prefabsFichasDominó.Count != 28)
        {
            Debug.LogWarning("¡Cuidado! Deberías tener 28 prefabs en la lista.");
        }

        CrearEInstanciarFichas();
        BarajarYRepartir();
    }

    void CrearEInstanciarFichas()
    {
        foreach (GameObject prefab in prefabsFichasDominó)
        {
            GameObject nuevaFicha = Instantiate(prefab, new Vector3(100, 100, 0), Quaternion.identity);
            instanciasFichas.Add(nuevaFicha);
        }
    }

    void BarajarYRepartir()
    {
        // Mezclamos las instancias
        for (int i = 0; i < instanciasFichas.Count; i++)
        {
            GameObject temp = instanciasFichas[i];
            int randomIndex = Random.Range(i, instanciasFichas.Count);
            instanciasFichas[i] = instanciasFichas[randomIndex];
            instanciasFichas[randomIndex] = temp;
        }

        // Repartir 7 al Jugador
        for (int i = 0; i < 7; i++)
        {
            GameObject fichaGO = instanciasFichas[0];
            instanciasFichas.RemoveAt(0);

            LogicaFicha logica = fichaGO.GetComponent<LogicaFicha>();
            DominoManager.Instance.manoJugador.Add(logica);
            
            // Configuración visual y de control
            fichaGO.transform.SetParent(contenedorManoJugador);
            fichaGO.transform.localPosition = new Vector3(i * espacioEntreFichasMano, 0, 0); 
            fichaGO.GetComponent<FichaInteractiva>().esDelJugador = true;
        }

        // Repartir 7 a la CPU
        for (int i = 0; i < 7; i++)
        {
            GameObject fichaGO = instanciasFichas[0];
            instanciasFichas.RemoveAt(0);

            LogicaFicha logica = fichaGO.GetComponent<LogicaFicha>();
            DominoManager.Instance.manoCPU.Add(logica);
            
            // Ocultamos la ficha de la CPU 
            fichaGO.GetComponent<SpriteRenderer>().enabled = false;
        }

        // El resto van al pozo (DominoManager)
        foreach (GameObject fichaRestante in instanciasFichas)
        {
            DominoManager.Instance.pozo.Add(fichaRestante.GetComponent<LogicaFicha>());
            // También ocultamos las del pozo
            fichaRestante.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
