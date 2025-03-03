using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayGameButton : MonoBehaviour, IPointerClickHandler
{
    public Transform player;
    public Transform spawnPoint;
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
        Time.timeScale = 1; // Ensure time is running
        //SceneManager.LoadScene("LevelScene");
         
        player.transform.position = spawnPoint.transform.position;
        player.transform.rotation = spawnPoint.transform.rotation;

    }
}
