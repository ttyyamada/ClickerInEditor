using System;
using System.IO;
using Editor.EditorClicker.Data;
using UnityEngine;

namespace Editor.EditorClicker.Scripts
{
    /// <summary>
    /// データをセーブする
    /// </summary>
    public class SaveService
    {
        public void Save(UserData data)
        {
            string dataJson = JsonUtility.ToJson(data);
           
            using (StreamWriter streamWriter = new StreamWriter(DefaultData.savePath, false))
            {
                streamWriter.Write(dataJson);
            }
        }

        public UserData Load()
        {
            // セーブデータがつくられてなければnullを返す
            if (!File.Exists(DefaultData.savePath)) return null;
            using var streamReader = new StreamReader(DefaultData.savePath);
            try
            {
                var dataJson = streamReader.ReadToEnd();
                var data = JsonUtility.FromJson<UserData>(dataJson);
                if (data != null)
                {
                    return data;
                }

            }
            catch (Exception e)
            {
                Debug.LogError("LoadError: " + e);
            }

            return null;
        }
    }
}
