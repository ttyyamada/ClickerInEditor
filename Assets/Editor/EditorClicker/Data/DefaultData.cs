using UnityEngine;

namespace Editor.EditorClicker.Data
{
    /// <summary>
    /// デフォルトで作られるデータ
    /// </summary>
    public static class DefaultData
    {
        // 最初に解放されるスキルナンバー
        public static readonly int[] skillNo = {0};
    
        // セーブデータのフォルダパス
        public static readonly string savePath = Application.dataPath + "/Editor/EditorClicker/SaveData/SaveData.txt";
    }
}
