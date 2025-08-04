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
    private bool roundCleared = false;

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

                if (roundCleared)
                    break;
            }

            ClearTargets();

            if (roundCleared)
            {
                round++;
            }
            else
            {
                EndGame(false);
                yield break;
            }
        }

    currentSkill.LevelUp(currentDungeonLevel);
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

        GameObject obj = Instantiate(prefab);
        RectTransform objRect = obj.GetComponent<RectTransform>();
        if (objRect == null)
        {
            Debug.LogError("Prefab에 RectTransform이 없습니다.");
            return;
        }

        // 부모 설정 후 사이즈, 좌표 보정
        obj.transform.SetParent(spawnRect, false); 

        objRect.anchorMin = new Vector2(0.5f, 0.5f);
        objRect.anchorMax = new Vector2(0.5f, 0.5f);
        objRect.pivot = new Vector2(0.5f, 0.5f);

        Vector2 randomPos = GetRandomPositionInRect(spawnRect, objRect);
        objRect.anchoredPosition = randomPos;

        var clickTarget = obj.GetComponent<ClickTarget>();
        if (clickTarget != null)
        {
            clickTarget.Init(this);
        }
        else
        {
            Debug.LogWarning("ClickTarget 컴포넌트가 없습니다.");
        }

        Destroy(obj, 2f);
    }

    Vector2 GetRandomPositionInRect(RectTransform parentRect, RectTransform targetRect)
    {
        Vector2 parentSize = parentRect.rect.size;
        Vector2 targetSize = targetRect.rect.size;

        float xRange = (parentSize.x - targetSize.x) / 2f;
        float yRange = (parentSize.y - targetSize.y) / 2f;

        float x = Random.Range(-xRange, xRange);
        float y = Random.Range(-yRange, yRange);

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
            roundCleared = true;
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
            ? $"Success: {currentSkill.level}"
            : "fail";
    }
}