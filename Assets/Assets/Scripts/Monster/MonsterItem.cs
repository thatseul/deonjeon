// MonsterItem.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Create New Monster")]
public class MonsterItem : ScriptableObject
{
    public string monsterName;
    public Sprite monsterIcon;
    public GameObject monsterPrefab;
    public int cost;
    public float statScaleRatio;

    [Range(0f, 100f)]
    public float probability; // 뽑기 확률 (0~100%)
}
