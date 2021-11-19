using System.Collections.Generic;
using System.Linq;
using Editor.EditorClicker.Scripts.Utils;
using UnityEngine;

namespace Editor.EditorClicker.Data
{
    /// <summary>
    /// ショップのデータ
    /// </summary>
    public class ShopData
    {
        private List<SkillData> skillList;
        
        /// <summary>
        /// スキルのリストをロードする
        /// </summary>
        public void LoadSkillData()
        {
            skillList = Resources.LoadAll<SkillData>(ResourcesNames.SkillPathFolder).ToList();
            if (skillList == null)
            {
                Debug.LogError("スキルデータがみつかりません！");
            }
        }

        /// <summary>
        /// 表示するべきショップのデータを返す
        /// </summary>
        public List<SkillData> CheckCurrentShowSkillData(List<SkillData> openSkillData)
        {
            var targetData = new List<SkillData>();
            foreach (var openSkill in openSkillData)
            {
                if(openSkill?.releaseIDs == null) continue;
                foreach (var openSkillReleaseID in openSkill.releaseIDs)
                {
                    var skill = skillList.Find(x => x.skillNo == openSkillReleaseID);
                    // 見つからない、もしくは解放済みの場合はcontinue
                    if (skill == null || openSkillData.Contains(skill) || targetData.Contains(skill))
                    {
                        continue;
                    }
                    targetData.Add(skill);
                }
                
            }

            return targetData;
        }

        /// <summary>
        /// 対象の番号のスキルデータを取得する
        /// </summary>
        public SkillData GetSkillData(int num)
        {
            return skillList.Find(x => x.skillNo == num);
        }
    }
}
