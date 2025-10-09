using UnityEngine;
using TMPro;

public class GoldTextUI_Smooth : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private string prefix = "Gold: ";
    [SerializeField] private float smooth = 12f;
    double shown, target;

    void Reset() { if (!goldText) goldText = GetComponent<TMP_Text>(); }

    void Update()
    {
        if (!goldText || GameState.I == null) return;

        target = GameState.I.Gold;
        shown = Mathf.Lerp((float)shown, (float)target, Time.deltaTime * smooth);
        goldText.text = prefix + Mathf.FloorToInt((float)shown).ToString("N0");
    }
}
