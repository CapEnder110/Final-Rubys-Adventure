using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatNPC : MonoBehaviour
{
    public float displayTime = 4.0f;
    public GameObject dialogBox;
    public GameObject thanksDialogBox;
    float timerDisplay;

    AudioSource audioSource;
    public AudioClip interactSound;

    void Start()
    {
        dialogBox.SetActive(false);
        thanksDialogBox.SetActive(false);
        timerDisplay = -1.0f;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
                thanksDialogBox.SetActive(false);
            }
        }
    }

    public void DisplayDialog()
    {
        timerDisplay = displayTime;
        dialogBox.SetActive(true);

        audioSource.clip = interactSound;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void DisplayThanks()
    {
        timerDisplay = displayTime;
        thanksDialogBox.SetActive(true);

        audioSource.clip = interactSound;
        audioSource.loop = false;
        audioSource.Play();
    }
}
