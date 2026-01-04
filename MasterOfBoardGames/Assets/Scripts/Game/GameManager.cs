using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int puntosJugador = 0;
    public int puntosCPU = 0;
    public int puntosParaGanar = 3;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GanaJugador()
    {
        puntosJugador++;
        ComprobarGanador();
    }

    public void GanaCPU()
    {
        puntosCPU++;
        ComprobarGanador();
    }

    // cosas que tengo que hacer : crear las escenas de derrota y vitoria para finalizar el juego
    void ComprobarGanador()
    {
        if (puntosJugador >= puntosParaGanar)
        {
            SceneManager.LoadScene("VictoriaJugador");
        }
        else if (puntosCPU >= puntosParaGanar)
        {
            SceneManager.LoadScene("VictoriaCPU");
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void ResetearPuntuacion()
    {
        puntosJugador = 0;
        puntosCPU = 0;
    }
}
