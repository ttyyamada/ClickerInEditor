using Editor.EditorClicker.Scripts;
using UnityEngine;

namespace Editor.EditorClicker.Data
{
    /// <summary>
    /// スキルのデータ
    /// </summary>
    [CreateAssetMenu(fileName = "Skill", menuName = "Clicker/Data/Skill")]
    public class SkillData : ScriptableObject
    {
        [Header("連番")]
        public int skillNo;
        [Header("名前")]
        public string skillName;
        [Header("値段")]
        public int buyCost;
        [Header("スキル説明")]
        public string skillDessription;
        [Header("1クリックでいくらもらえるか")]
        public double clickRate = 1;
        [Header("クリックしなくてもいくらもらえるか")]
        public double autoRate = 0;
        [Header("オートでもらえる秒数")]
        public float autoGetRate = 1.0f;
        [Header("購入したら解放されるアイテムの番号")]
        public int[] releaseIDs;
        [Header("メモ")]
        [TextArea(4,8)]
        public string memo;

        public CurrentAutoGetStatus ConvertToAutoGetStatus()
        {
            var status = new CurrentAutoGetStatus();
            status.num = skillNo;
            status.getTime = autoGetRate;
            status.autoRate = autoRate;
            return status;
        }
    }
}
