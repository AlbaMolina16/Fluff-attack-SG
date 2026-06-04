using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calcula el area de juego permitido para el puntero y las pelusas enemigas
/// </summary>
public class LimitAreaGame : MonoBehaviour
{
    [Header("Cabecera")]
    public RectTransform headerCanvas;

    public static Vector3 InstanceMinPantalla { get; private set; }
    public static Vector3 InstanceMaxPantalla { get; private set; }

    private float cameraZ;
    private Vector3 minPantalla;
    private Vector3 maxPantalla;

    // Start is called before the first frame update
    void Start()
    {
        float headerHeight = headerCanvas != null ?headerCanvas.rect.height : 0f;
        // Obtiene la distancia en Z entre la cámara y el plano de juego
        cameraZ = Math.Abs(Camera.main.transform.position.z - transform.position.z);
        // Convertir esquinas de pantalla a coordenadas del mundo
        minPantalla = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, cameraZ));
        maxPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height - headerHeight, cameraZ));

        // Asignar valores a las propiedades estáticas
        InstanceMinPantalla = minPantalla;
        InstanceMaxPantalla = maxPantalla;
    }

    public static Vector3 SpriteLimit(Vector3 position, float radius)
    {
        return new Vector3(
            Mathf.Clamp(position.x, InstanceMinPantalla.x + radius, InstanceMaxPantalla.x - radius),
            Mathf.Clamp(position.y, InstanceMinPantalla.y + radius, InstanceMaxPantalla.y - radius),
            position.z
        );
    }
}