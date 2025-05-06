// MonsterItem.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Create New Monster")]
public class MonsterItem : ScriptableObject
{
    public string monsterName;
    public Sprite monsterIcon;
    public GameObject monsterPrefab;
    public int cost;
}
