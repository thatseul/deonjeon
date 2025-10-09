using UnityEngine;

/// <summary>
/// SpriteRenderer 2장으로 오른쪽으로 무한 스크롤되는 배경
/// </summary>
public class InfiniteBGScroll : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bg1;
    [SerializeField] private SpriteRenderer bg2;
    [SerializeField] private float speed = 2f; // 오른쪽으로 흐르는 속도

    private float width;

    void Start()
    {
        if (!bg1 || !bg2)
        {
            Debug.LogError("BG1, BG2 SpriteRenderer 연결 필요!");
            enabled = false;
            return;
        }

        width = bg1.bounds.size.x;

        // BG2를 BG1의 '왼쪽'에 배치 (오른쪽으로 흘러야 하므로 반대쪽에서 시작)
        Vector3 p1 = bg1.transform.position;
        bg2.transform.position = new Vector3(p1.x - width, p1.y, p1.z);
    }

    void Update()
    {
        float move = speed * Time.deltaTime;

        // 두 장을 동시에 오른쪽으로 이동
        bg1.transform.Translate(move, 0, 0);
        bg2.transform.Translate(move, 0, 0);

        // BG1이 오른쪽으로 완전히 벗어나면, 왼쪽 끝으로 재배치
        if (bg1.transform.position.x >= width)
        {
            bg1.transform.position -= new Vector3(width * 2f, 0, 0);
        }

        // BG2도 마찬가지
        if (bg2.transform.position.x >= width)
        {
            bg2.transform.position -= new Vector3(width * 2f, 0, 0);
        }
    }
}
