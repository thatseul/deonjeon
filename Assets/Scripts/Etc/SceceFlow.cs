using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlow : MonoBehaviour
{
    [SerializeField] string mainSceneName = "deonjeon2020";
    [SerializeField] string bossSceneName = "BossScene";
    bool busy;

    public void GoBoss()
    {
        if (!busy) StartCoroutine(CoGoBoss());
    }
    public void GoMain()
    {
        if (!busy) StartCoroutine(CoGoMain());
    }

    IEnumerator CoGoBoss()
    {
        busy = true;

        // 1) (선택) 보스 가기 직전, 현재 메인 수익/초를 계산해 전달
        var hook = FindObjectOfType<MainEconomyHook>(); // 아래 3번 참고
        if (hook) GameState.I.PassiveIncomePerSecond = hook.CalcIncomePerSecond();

        // 2) 보스 로드(Additive) → 활성화
        if (!SceneManager.GetSceneByName(bossSceneName).isLoaded)
            yield return SceneManager.LoadSceneAsync(bossSceneName, LoadSceneMode.Additive);
        var boss = SceneManager.GetSceneByName(bossSceneName);
        if (boss.IsValid()) SceneManager.SetActiveScene(boss);

        // 3) 메인 언로드(겹침 제거)
        var main = SceneManager.GetSceneByName(mainSceneName);
        if (main.IsValid() && main.isLoaded)
            yield return SceneManager.UnloadSceneAsync(main);

        busy = false;
    }

    IEnumerator CoGoMain()
    {
        busy = true;

        // 1) 메인 로드(Additive) → 활성화
        if (!SceneManager.GetSceneByName(mainSceneName).isLoaded)
            yield return SceneManager.LoadSceneAsync(mainSceneName, LoadSceneMode.Additive);
        var main = SceneManager.GetSceneByName(mainSceneName);
        if (main.IsValid()) SceneManager.SetActiveScene(main);

        // 2) 보스 언로드
        var boss = SceneManager.GetSceneByName(bossSceneName);
        if (boss.IsValid() && boss.isLoaded)
            yield return SceneManager.UnloadSceneAsync(boss);

        busy = false;
    }
}
