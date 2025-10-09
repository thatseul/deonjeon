// MainEconomyHook.cs (완성판: B안)
using UnityEngine;

public class MainEconomyHook : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private HeroSpawner spawner;
    [SerializeField] private Dungeon dungeon;

    [Header("보정 파라미터")]
    [Tooltip("던전 DPS 기반 처치속도와 웨이브 추정치 사이를 보간 (0=웨이브 추정만, 1=DPS만)")]
    [Range(0f, 1f)] [SerializeField] private float dpsBlend = 0.6f;

    [Tooltip("던전 DPS에서 계산한 kills/sec의 상한 배수(동시 타겟 제한 반영 전)")]
    [SerializeField] private float dpsSoftCapMultiplier = 2.0f;

    /// <summary> 보스 씬 체류 중, 초당 수익(골드/초)을 계산해 GameState로 전달할 값 </summary>
    public double CalcIncomePerSecond()
    {
        if (spawner == null || dungeon == null || GameState.I == null) return 0;

        // 1) 웨이브 기반 추정: 한 웨이브에 N명 / 평균 T초 -> kills/sec
        float kps_wave = spawner.GetKillsPerSecondWaveBased();

        // 2) 던전 DPS 기반 추정: DPS / 평균 HP -> kills/sec
        float dps = Mathf.Max(0f, dungeon.AttackPower) * Mathf.Max(0.01f, dungeon.AttackSpeed);
        float avgHp = Mathf.Max(1f, spawner.GetAverageHeroHpEstimate());
        float kps_dps = dps / avgHp;

        // 3) 던전 DPS 기반 추정치에 소프트 캡(너무 크게 튀는 값 억제)
        //    예: 웨이브 추정의 dpsSoftCapMultiplier배 정도까지만 유효하게 인정
        float softCap = kps_wave * Mathf.Max(1f, dpsSoftCapMultiplier);
        kps_dps = Mathf.Min(kps_dps, softCap);

        // 4) 두 추정치를 블렌딩
        float kps_blended = Mathf.Lerp(kps_wave, kps_dps, Mathf.Clamp01(dpsBlend));

        // 5) 동시 타겟(스킬/공격 판정) 제한으로 하드 캡
        int simultaneousCap = spawner.GetMaxTargetsSimultaneously();
        float kps_final = Mathf.Min(kps_blended, simultaneousCap);

        // 6) 최종 골드/초
        double gps = kps_final * spawner.GoldPerKill;
        return gps;
    }
}
