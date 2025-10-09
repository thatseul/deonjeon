// Assets/Editor/FindAllSpritesInScene.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FindAllSpritesInScene
{
    [MenuItem("Tools/Find All Sprites in Scene")]
    public static void FindSprites()
    {
        var renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        var images = Object.FindObjectsByType<Image>(FindObjectsSortMode.None);

        Debug.Log("=== SpriteRenderer Sprites ===");
        foreach (var r in renderers)
        {
            if (r.sprite != null)
                Debug.Log($"{r.gameObject.name} → Sprite: {r.sprite.name}");
        }

        Debug.Log("=== UI Images ===");
        foreach (var img in images)
        {
            if (img.sprite != null)
                Debug.Log($"{img.gameObject.name} → Sprite: {img.sprite.name}");
        }

        Debug.Log("==== DONE ====");
    }
}
