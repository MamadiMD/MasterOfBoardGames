using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BriscaOptions : MonoBehaviour
{
    public GameObject canvasOpciones;
    public GameObject canvasInstrucciones;
    public AudioSource audioClick;

    public void Rendirse()
    {
        audioClick.Play();
        GameManager.instance.GanaCPU();
        SceneManager.LoadScene(1);
    }

    public void Reiniciar()
    {
        audioClick.Play();
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual);
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

    // Instrucciones

    public void CerrarInstrucciones()
    {
        audioClick.Play();
        canvasInstrucciones.SetActive(false);
    }
    
    public void AbrirInstrucciones()
    {
        audioClick.Play();
        canvasInstrucciones.SetActive(true);
    }
}
