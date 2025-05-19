using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 씬 이름을 받아서 씬 전환
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 예시: 보스씬으로 이동
    public void GoToBossScene()
    {
        ChangeScene("BossScene"); // 빌드 세팅에 등록된 보스씬 이름
    }

    // 예시: 메인씬으로 이동
    public void GoToMainScene()
    {
        ChangeScene("deonjeon2020"); // 빌드 세팅에 등록된 메인씬 이름
    }
}
