using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    [Header("Optional")]
    public AudioClip clickSfx; 
    private Button btn;
    private AudioSource audioSrc;

    void Awake()
    {
        btn = GetComponent<Button>();
        audioSrc = gameObject.AddComponent<AudioSource>();
    }

    void OnEnable()
    {
        btn.onClick.AddListener(OnClick);
    }
    void OnDisable()
    {
        btn.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        // 살짝 눌렸다 돌아오는 팝
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOScale(0.9f, 0.05f)
                .OnComplete(() => transform.DOScale(1f, 0.05f));

        if (clickSfx) audioSrc.PlayOneShot(clickSfx);
    }
}
