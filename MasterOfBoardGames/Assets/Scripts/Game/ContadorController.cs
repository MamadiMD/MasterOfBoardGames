using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContadorController : MonoBehaviour
{
    public TMP_Text contadorJugador;
    public TMP_Text contadorCPU;
    void Start()
    {
        ActualizarContador();
    }

    void Update()
    {
        ActualizarContador();
    }

    void ActualizarContador()
    {
        contadorJugador.text = $"Jugador: {GameManager.instance.puntosJugador}";
        contadorCPU.text = $"CPU: {GameManager.instance.puntosCPU}";
    }
}
