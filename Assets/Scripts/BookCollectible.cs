using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if(controller.books <= controller.booksHeld)
            {
                controller.GrabBook(1);
                controller.BookText();
                Destroy(gameObject);

                controller.PlaySound(collectedClip);
            }
        }
    }
}
