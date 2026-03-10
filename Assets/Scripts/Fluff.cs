using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluff : MonoBehaviour
{
    public GameObject onCollectEffect;

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other object has a PlayerController2D component
        if (other.CompareTag("Player"))
        {
            // Destroy the collectible
            //Destroy(gameObject);
        }
    }
}