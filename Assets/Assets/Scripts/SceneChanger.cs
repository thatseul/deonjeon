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

        // 👉 메인 씬의 모든 시각 요소 비활성화
        GameObject mainUI = GameObject.Find("MainUI"); // 너 UI 이름에 맞게 수정
        if (mainUI != null) mainUI.SetActive(false);

        Camera mainCam = Camera.main;
        if (mainCam != null) mainCam.gameObject.SetActive(false);
    }

    /// <summary>
    /// 보스 육성 씬을 언로드하여 메인 씬만 남김
    /// </summary>
    public void GoToMainScene()
    {
        if (SceneManager.GetSceneByName(bossSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(bossSceneName);
            Debug.Log($"🏃 보스씬 {bossSceneName} 언로드 완료, 메인씬 UI 복원");
        }

        GameObject mainUI = GameObject.Find("MainUI"); // 실제 오브젝트 이름으로 수정
        if (mainUI != null) mainUI.SetActive(true);

        Camera mainCam = Camera.main;
        if (mainCam != null) mainCam.gameObject.SetActive(true);
    }
}
