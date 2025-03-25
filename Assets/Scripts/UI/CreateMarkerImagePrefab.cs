using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR
/// <summary>
/// Editor utility for creating a marker image prefab
/// </summary>
public class CreateMarkerImagePrefab : MonoBehaviour
{
    [MenuItem("GameObject/UI/Objective Marker Image")]
    public static void CreateMarkerImage()
    {
        // Create a new game object
        GameObject markerObject = new GameObject("ObjectiveMarkerImage");
        
        // Add a rect transform (required for UI elements)
        RectTransform rectTransform = markerObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(32, 32);
        
        // Add an image component
        Image image = markerObject.AddComponent<UnityEngine.UI.Image>();
        
        // Try to find a built-in marker sprite
        Sprite sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        if (sprite != null)
        {
            image.sprite = sprite;
        }
        
        // Set the parent to the currently selected object if it's a canvas or has a canvas component
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            Canvas canvas = selectedObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                markerObject.transform.SetParent(selectedObject.transform, false);
            }
            else if (selectedObject.transform.parent != null)
            {
                Canvas parentCanvas = selectedObject.transform.parent.GetComponent<Canvas>();
                if (parentCanvas != null)
                {
                    markerObject.transform.SetParent(selectedObject.transform.parent, false);
                }
            }
        }
        
        // Select the newly created object
        Selection.activeGameObject = markerObject;
        
        // Position in center of screen
        markerObject.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        Debug.Log("Created marker image prefab. You can now drag this to your project folder to make it a prefab.");
    }
}
#endif 