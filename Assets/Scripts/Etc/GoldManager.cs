using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    void Update()
    {
        if (text && GameState.I)
            text.text = Mathf.FloorToInt((float)GameState.I.Gold).ToString("N0");
    }
}
