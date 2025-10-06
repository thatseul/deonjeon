using System.Collections.Generic;
using UnityEngine;

public class SkillTransferManager : MonoBehaviour
{
    public static SkillTransferManager Instance;
    public List<BossSkill> selectedSkills = new List<BossSkill>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
