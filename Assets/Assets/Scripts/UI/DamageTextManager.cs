using UnityEngine;
using TMPro;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;
    public GameObject damageTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(Vector3 position, float damage)
    {
        GameObject obj = Instantiate(damageTextPrefab, position, Quaternion.identity);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
        Destroy(obj, 1.2f); // 1.2초 후 자동 삭제
    }
}
