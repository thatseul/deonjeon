using UnityEditor;
using UnityEngine;
using System.IO;

public class SkillDataGenerator
{
    [MenuItem("Tools/Generate Default SkillData Assets")]
    public static void GenerateSkillDataAssets()
    {
        string rootFolder = "Assets/Resources";
        string skillFolder = $"{rootFolder}/Skills";

        // Resources 폴더가 없으면 생성
        if (!AssetDatabase.IsValidFolder(rootFolder))
            AssetDatabase.CreateFolder("Assets", "Resources");

        // Skills 폴더가 없으면 생성
        if (!AssetDatabase.IsValidFolder(skillFolder))
            AssetDatabase.CreateFolder(rootFolder, "Skills");

        // 예시 스킬 데이터
        var skillTemplates = new[]
        {
            new { name = "Fireball", attack = 120f, cooldown = 3f, range = 5f, grade = SkillGrade.Normal },
            new { name = "Ice Spike", attack = 100f, cooldown = 2f, range = 6f, grade = SkillGrade.Normal },
            new { name = "Thunderstorm", attack = 200f, cooldown = 5f, range = 7f, grade = SkillGrade.Rare },
            new { name = "Healing Light", attack = 0f, cooldown = 4f, range = 5f, grade = SkillGrade.Rare },
            new { name = "Dark Slash", attack = 250f, cooldown = 6f, range = 3f, grade = SkillGrade.Epic },
            new { name = "Meteor", attack = 300f, cooldown = 7f, range = 8f, grade = SkillGrade.Epic },
            new { name = "Divine Judgment", attack = 400f, cooldown = 8f, range = 9f, grade = SkillGrade.Legendary },
            new { name = "Time Stop", attack = 0f, cooldown = 10f, range = 10f, grade = SkillGrade.Legendary },
            new { name = "Poison Cloud", attack = 80f, cooldown = 3.5f, range = 6f, grade = SkillGrade.Normal },
            new { name = "Lightning Spear", attack = 180f, cooldown = 4.5f, range = 7f, grade = SkillGrade.Rare },
        };

        foreach (var template in skillTemplates)
        {
            SkillData asset = ScriptableObject.CreateInstance<SkillData>();
            asset.skillName = template.name;
            asset.baseAttackPercent = template.attack;
            asset.baseCooldown = template.cooldown;
            asset.baseRange = template.range;
            asset.grade = template.grade;

            string assetPath = $"{skillFolder}/{template.name}.asset";
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ SkillData 10개가 자동 생성되었습니다.");
    }
}
