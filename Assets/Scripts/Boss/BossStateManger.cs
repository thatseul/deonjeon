using UnityEngine;

public class BossStateManager : MonoBehaviour
{
    public static BossStateManager Instance;

    public float bonusAtk = 0f;
    public float bonusCooldown = 0f;
    public float bonusRange = 0f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyUpgrade(UpgradeType type, int value)
    {
        switch (type)
        {
            case UpgradeType.AttackPercent:
                bonusAtk += value * 0.1f;
                break;

            case UpgradeType.CooldownReduce:
                bonusCooldown -= value * 0.2f;
                bonusCooldown = Mathf.Max(-4.5f, bonusCooldown); // 쿨타임 0.5 이하로 안 내려가도록 보호
                break;

            case UpgradeType.RangeIncrease:
                bonusRange += value * 0.2f;
                break;
        }
    }
}
