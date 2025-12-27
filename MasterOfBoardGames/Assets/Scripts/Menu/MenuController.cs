using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject canvasMenu;
    public GameObject canvasJugar;
    public GameObject canvasOpciones;
    public AudioSource audioClick;

    // Botón jugar desde el menu inicial
    public void IrAJugar()
    {
        audioClick.Play();
        canvasMenu.SetActive(false);
        canvasJugar.SetActive(true);
    }

    // Botón opciones desde el menu inicial
    public void IrAOpcionesMenu()
    {
        audioClick.Play();
        canvasMenu.SetActive(false);
        canvasOpciones.SetActive(true);
    }

    // Botón opciones desde el menu inicial
    public void IrAOpcionesJugar()
    {
        audioClick.Play();
        canvasJugar.SetActive(false);
        canvasOpciones.SetActive(true);
    }

    // Volver al menú
    public void VolverMenu()
    {
        audioClick.Play();
        canvasJugar.SetActive(false);
        canvasOpciones.SetActive(false);
        canvasMenu.SetActive(true);
    }

    // Iniciar una nueva partida
    public void NuevaPartida()
    {
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual + 1);
    }

    // Botón Salir
    public void Salir()
    {
        audioClick.Play();
        Application.Quit();
        Debug.Log("Saliendo del juego"); // útil en el editor
    }
}
