using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        controller.ActivateSpeedBoost();
        controller.PlaySound(collectedClip);

        Destroy(gameObject);
    }
}
