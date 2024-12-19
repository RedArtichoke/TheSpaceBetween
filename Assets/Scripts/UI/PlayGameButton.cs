using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayGameButton : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI tmpText; // reference to TMP text component

    // Called when the script instance is being loaded
    void Awake()
    {
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }
    }

    // Called when the button is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Loading Game Scene");
        SceneManager.LoadScene("LevelScene"); 
    }
}
