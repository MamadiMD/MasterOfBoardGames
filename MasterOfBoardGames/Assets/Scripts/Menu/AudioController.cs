using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Button botonSonido;
    public Button botonMusica;
    public Sprite sonidoOn;
    public Sprite sonidoOff;
    public Sprite musicaOn;
    public Sprite musicaOff;
    public AudioSource audioSonido; 
    public AudioSource audioMusica;

    private bool sonidoActivo = true;
    private bool musicaActiva = true;

    void Start()
    {
        ActualizarSonido();
        ActualizarMusica();
    }

      public void ToggleSonido()
    {
        sonidoActivo = !sonidoActivo;
        audioSonido.mute = !sonidoActivo;
        ActualizarSonido();
    }

    public void ToggleMusica()
    {
        musicaActiva = !musicaActiva;
        audioMusica.mute = !musicaActiva;
        ActualizarMusica();
    }

    void ActualizarSonido()
    {
        if (sonidoActivo){
            botonSonido.image.sprite = sonidoOn;
        }
        else{
            botonSonido.image.sprite = sonidoOff;
        }
    }

    void ActualizarMusica()
    {
        if (musicaActiva){
            botonMusica.image.sprite = musicaOn;
        }
        else{
            botonMusica.image.sprite = musicaOff;
        }
    }
}
