using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour
{
    public GameObject canvasOpciones;
    public GameObject canvasAjedrez;
    public AudioSource audioClick;
    public GameObject canvasDamas;
    public GameObject canvasDomino;
    public GameObject canvasBrisca;
    public GameObject canvasShisima;
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

    // Damas Options
    public void CerrarDamas()
    {
        audioClick.Play();
        canvasDamas.SetActive(false);
    }

    public void JugarDamas()
    {
        audioClick.Play();
        SceneManager.LoadScene(3);
    }

    public void AbrirDamas()
    {
        audioClick.Play();
        canvasDamas.SetActive(true);
    }

    // Domino Options
    public void CerrarDomino()
    {
        audioClick.Play();
        canvasDomino.SetActive(false);
    }

    public void JugarDomino()
    {
        audioClick.Play();
        SceneManager.LoadScene(4);
    }

    public void AbrirDomino()
    {
        audioClick.Play();
        canvasDomino.SetActive(true);
    }

    // Brisca Options

    public void CerrarBrisca()
    {
        audioClick.Play();
        canvasBrisca.SetActive(false);
    }

    public void JugarBrisca()
    {
        audioClick.Play();
        SceneManager.LoadScene(5);
    }

    public void AbrirBrisca()
    {
        audioClick.Play();
        canvasBrisca.SetActive(true);
    }

    // Shisima options
    public void CerrarShisima()
    {
        audioClick.Play();
        canvasShisima.SetActive(false);
    }

    public void JugarShisima()
    {
        audioClick.Play();
        SceneManager.LoadScene(6);
    }

    public void AbrirShisima()
    {
        audioClick.Play();
        canvasShisima.SetActive(true);
    }
}
