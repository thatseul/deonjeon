using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillDatabase : MonoBehaviour
{
   public List<SkillData> allSkillData;

    private void Awake()
    {
        allSkillData = Resources.LoadAll<SkillData>("Skills").ToList();

        if (allSkillData.Count == 0)
        {
            Debug.LogWarning("SkillDatabase: 불러온 스킬 데이터가 없습니다. Resources/Skills 경로에 SkillData 파일을 확인하세요.");
        }
    }

    public List<SkillData> GetRandomSkills(int count)
    {
        if (allSkillData == null || allSkillData.Count == 0)
            return new List<SkillData>();

        return allSkillData.OrderBy(x => Random.value).Take(count).ToList();
    }
}
