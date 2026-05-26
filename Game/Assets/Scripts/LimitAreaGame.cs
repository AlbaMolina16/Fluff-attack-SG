using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calcula el area de juego permitido para el puntero y las pelusas enemigas
/// </summary>
public class LimitAreaGame : MonoBehaviour
{
    [Header("Altura de la cabecera")]
    public float headerHeight = 80f; // Como he añadido un header canvas para mostrar la puntuacion y el tiempo, tengo que restar la altura que ocupa en la pantalla para limitar el puntero

    public static Vector3 InstanceMinPantalla { get; private set; }
    public static Vector3 InstanceMaxPantalla { get; private set; }

    private float cameraZ;
    private Vector3 minPantalla;
    private Vector3 maxPantalla;

    // Start is called before the first frame update
    void Start()
    {
        // Obtiene la distancia en Z entre la cámara y el plano de juego
        cameraZ = Math.Abs(Camera.main.transform.position.z - transform.position.z);
        // Convertir esquinas de pantalla a coordenadas del mundo
        minPantalla = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, cameraZ));
        maxPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height - headerHeight, cameraZ));

        // Asignar valores a las propiedades estáticas
        InstanceMinPantalla = minPantalla;
        InstanceMaxPantalla = maxPantalla;
    }
}