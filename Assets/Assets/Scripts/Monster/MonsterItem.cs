// MonsterItem.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Create New Monster")]
public class MonsterItem : ScriptableObject
{
    public string monsterName;
    public Sprite monsterIcon;
    public GameObject monsterPrefab;
    public int cost;
    public float statScaleRatio; //몬스터별 능력치 반영 비율 다르게
}
