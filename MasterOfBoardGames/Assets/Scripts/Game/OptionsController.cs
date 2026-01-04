using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour
{
    public GameObject canvasOpciones;
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
}
