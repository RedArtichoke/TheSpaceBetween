using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class typingAnimation : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float baseTypingSpeed = 0.05f;  // Base delay between letters
    public float randomSpeedFactor = 0.02f; // Random variance
    //public AudioSource typingSound; // Optional: Add a sci-fi beep sound
    public bool useGlitchEffect = true;

    private string fullText;
    private Coroutine typingCoroutine;

    void Start()
    {
        fullText = textComponent.text;
        textComponent.text = "";  // Start with empty text
        StartTyping();
    }

    public void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textComponent.text = "";
        foreach (char letter in fullText)
        {
            // Random typing speed for variation
            float delay = baseTypingSpeed + Random.Range(-randomSpeedFactor, randomSpeedFactor);

            // Apply a glitch effect (random letter flicker)
            if (useGlitchEffect && Random.value > 0.8f)
            {
                textComponent.text += RandomLetter();
                yield return new WaitForSeconds(0.02f);
                textComponent.text = textComponent.text.Substring(0, textComponent.text.Length - 1);
            }

            // Append correct letter
            textComponent.text += letter;

            // Play typing sound
            //if (typingSound != null)
            //  typingSound.Play();

            yield return new WaitForSeconds(delay);
        }

        // Add a blinking cursor effect
        StartCoroutine(BlinkingCursor());
    }

    char RandomLetter()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        return chars[Random.Range(0, chars.Length)];
    }

    IEnumerator BlinkingCursor()
    {
        while (true)
        {
            textComponent.text += "_"; // Cursor on
            yield return new WaitForSeconds(0.5f);
            textComponent.text = textComponent.text.TrimEnd('_'); // Cursor off
            yield return new WaitForSeconds(0.5f);
        }
    }
}

