using System;
using System.Collections.Generic;

namespace Editor.EditorClicker.Data
{
    /// <summary>
    /// ユーザのデータ
    /// </summary>
    [Serializable]
    public class UserData
    {
        // 今持ってるクッキーの数
        public double currentCookies = 0;
        // 解放しているスキルの数
        public List<int> openSkillIndex = new List<int>();
        // 今オープンしているスキルの実際のデータ
        [NonSerialized] public List<SkillData> openSkillData = new List<SkillData>();
    }
}
