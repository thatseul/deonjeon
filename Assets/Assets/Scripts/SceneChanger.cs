using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("씬 이름은 빌드 세팅에 등록되어 있어야 함")]
    [SerializeField] private string mainSceneName = "deonjeon2020";  // 메인 전투 씬 이름
    [SerializeField] private string bossSceneName = "BossScene";     // 보스 육성 씬 이름

    /// <summary>
    /// 보스 육성 씬을 Additive로 로드하여 메인 씬 유지
    /// </summary>
    public void GoToBossScene()
    {
        if (!SceneManager.GetSceneByName(bossSceneName).isLoaded)
        {
            SceneManager.LoadScene(bossSceneName, LoadSceneMode.Additive);
            Debug.Log($"✅ 보스씬 {bossSceneName} Additive로 로드 완료");
        }
        else
        {
            Debug.Log($"⚠️ 보스씬 {bossSceneName} 이미 로드되어 있음");
        }
    }

    /// <summary>
    /// 보스 육성 씬을 언로드하여 메인 씬만 남김
    /// </summary>
    public void GoToMainScene()
    {
        if (SceneManager.GetSceneByName(bossSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(bossSceneName);
            Debug.Log($"🏃 보스씬 {bossSceneName} 언로드 완료, 메인씬 유지됨");
        }
        else
        {
            Debug.Log($"⚠️ 보스씬 {bossSceneName}은 현재 로드되지 않았음");
        }
    }
}
