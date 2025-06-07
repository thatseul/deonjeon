using System.Collections;
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

    public int currentDungeonLevel = 5; // 기본값
    private int score = 0;
    private float timeLimit = 15f;
    private int round = 1;
    private int successPerRound = 3;
    private BossSkill currentSkill;
    private SkillUI linkedUI;
    private Coroutine gameCoroutine;

    public void StartMiniGame(BossSkill skill, SkillUI ui = null)
    {
        gameObject.SetActive(true);
        currentSkill = skill;
        linkedUI = ui;
        score = 0;
        round = 1;

        if (resultPanel != null)
            resultPanel.SetActive(false);

        gameCoroutine = StartCoroutine(RunRound());
    }

   private bool roundCleared = false;  // 라운드 성공 여부 상태 변수

    IEnumerator RunRound()
    {
        while (round <= 3)
        {
            float timeLeft = timeLimit - ((round - 1) * 5);
            score = 0;
            scoreText.text = $"Score: {score}";
            float timer = timeLeft;
            roundCleared = false;

            ClearTargets();

            while (timer > 0)
            {
                SpawnTargets();
                timerText.text = $"Time: {timer:F1}";

                yield return new WaitForSeconds(1f);
                timer -= 1f;

                if (roundCleared)  // 점수 달성했으면 즉시 종료
                    break;
            }

            ClearTargets();

            if (roundCleared)
            {
                currentSkill.LevelUp(currentDungeonLevel);
                round++;
            }
            else
            {
                EndGame(false);
                yield break;
            }
        }

        EndGame(true);
    }

    void SpawnTargets()
    {
        RectTransform spawnRect = spawnArea as RectTransform;
        if (spawnRect == null)
        {
            Debug.LogError("spawnArea가 RectTransform이 아닙니다.");
            return;
        }

        GameObject prefab = Random.value > 0.3f ? goodTargetPrefab : badTargetPrefab;
        if (prefab == null)
        {
            Debug.LogError("Prefab이 할당되지 않았습니다.");
            return;
        }

        GameObject obj = Instantiate(prefab, spawnArea);
        RectTransform objRect = obj.GetComponent<RectTransform>();
        if (objRect == null)
        {
            Debug.LogError("Prefab에 RectTransform 컴포넌트가 없습니다.");
            return;
        }

        Vector2 randomPos = GetRandomPositionInRect(spawnRect);
        objRect.anchoredPosition = randomPos;

        var clickTarget = obj.GetComponent<ClickTarget>();
        if (clickTarget == null)
        {
            Debug.LogError("ClickTarget 컴포넌트가 없습니다.");
            return;
        }
        clickTarget.Init(this);

        Destroy(obj, 2f);
    }

    Vector2 GetRandomPositionInRect(RectTransform rectTransform)
    {
        Vector2 size = rectTransform.rect.size;
        float x = Random.Range(-size.x / 2f, size.x / 2f);
        float y = Random.Range(-size.y / 2f, size.y / 2f);
        return new Vector2(x, y);
    }


    void ClearTargets()
    {
        foreach (Transform child in spawnArea)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddScore()
    {
        score++;
        scoreText.text = $"Score: {score}";

        if (score >= successPerRound)
        {
            roundCleared = true;  // 성공 플래그 켜서 다음 라운드 준비
        }
    }

    public void EndGame(bool isSuccess)
    {
        if (gameCoroutine != null)
        {
            StopCoroutine(gameCoroutine);
            gameCoroutine = null;
        }

        ClearTargets();

        if (linkedUI != null)
        {
            linkedUI.RefreshUI();
        }

        resultPanel.SetActive(true);
        resultText.text = isSuccess
            ? $"Skill success: {currentSkill.level}"
            : "fail";
    }
}
