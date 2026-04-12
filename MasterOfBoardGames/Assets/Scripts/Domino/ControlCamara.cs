using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamara : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float sensibilidadArrastre = 1f;
    private Vector3 origenClic;

    [Header("Ajustes de Zoom")]
    public float sensibilidadZoom = 2f;
    public float zoomMinimo = 2f;
    public float zoomMaximo = 15f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        ManejarMovimiento();
        ManejarZoom();
    }

    void ManejarMovimiento()
    {
        // Usamos el botón derecho (1) para mover la cámara y no interferir con las fichas (0)
        if (Input.GetMouseButtonDown(1))
        {
            origenClic = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 diferencia = origenClic - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += diferencia;
        }
    }

    void ManejarZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float nuevoZoom = cam.orthographicSize - (scroll * sensibilidadZoom);
            cam.orthographicSize = Mathf.Clamp(nuevoZoom, zoomMinimo, zoomMaximo);
        }
    }
}
