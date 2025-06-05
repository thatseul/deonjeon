using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGameController : MonoBehaviour
{
    public GameObject goodTargetPrefab;
    public GameObject badTargetPrefab;
    public Transform spawnArea;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject resultPanel;
    public TMP_Text resultText;

    public int currentDungeonLevel = 5; // 기본값 5
    private int score = 0;
    private float timeLimit = 15f; // 단계 1
    private int round = 1;
    private int successPerRound = 3;
    private BossSkill currentSkill;
    private SkillUI linkedUI;

    public void StartMiniGame(BossSkill skill, SkillUI ui = null)
    {
        gameObject.SetActive(true);
        currentSkill = skill;
        linkedUI = ui;
        score = 0;
        round = 1;
        StartCoroutine(RunRound());
    }


    IEnumerator RunRound()
    {
        while (round <= 3)
        {
            float timeLeft = timeLimit - ((round - 1) * 5);
            score = 0;
            timerText.text = $"Time: {timeLeft}";
            float timer = timeLeft;

            while (timer > 0)
            {
                SpawnTargets();
                timer -= 1f;
                timerText.text = $"Time: {timer:F1}";
                yield return new WaitForSeconds(1f);
            }

            if (score >= successPerRound)
            {
                currentSkill.LevelUp(currentDungeonLevel);
                round++;
            }
            else break;
        }

        if (linkedUI != null)
        {
            linkedUI.RefreshUI(); // ← 강화된 레벨 반영!
        }


        resultPanel.SetActive(true);
        resultText.text = $"Skill 강화됨: {currentSkill.level}";
    }

    void SpawnTargets()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(Random.value > 0.3f ? goodTargetPrefab : badTargetPrefab, spawnArea);
            obj.GetComponent<ClickTarget>().Init(this);
        }
    }

    public void AddScore()
    {
        score++;
        scoreText.text = $"Score: {score}";
    }
}
