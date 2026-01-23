using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour
{
    public GameObject canvasOpciones;
    public GameObject canvasAjedrez;
    public AudioSource audioClick;
    public void VolverMenu()
    {
        audioClick.Play();
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(0);
    }

    public void CerrarOpciones()
    {
        audioClick.Play();
        canvasOpciones.SetActive(false);
    }

    public void AbrirOpciones()
    {
        audioClick.Play();
        canvasOpciones.SetActive(true);
    }

    // Ajedrez Options

    public void CerrarAjedrez()
    {
        audioClick.Play();
        canvasAjedrez.SetActive(false);
    }

    public void JugarAjedrez()
    {
        audioClick.Play();
        SceneManager.LoadScene(2);
    }

    public void AbrirAjedrez()
    {
        audioClick.Play();
        canvasAjedrez.SetActive(true);
    }
}
