namespace Editor.EditorClicker.Scripts.Utils
{
    public static class ResourcesNames
    {
        #if USE_DEBUG_RESOURCE
        // デバッグようのリソース
        private static readonly string resourcesPrefix = "Ignore/";
        #else
        private static readonly string resourcesPrefix = "";
        #endif
        // クッキーの画像のリソースパス
        public static readonly string CookieButtonImagePath = resourcesPrefix + "cookieButton";
        // スキルデータの格納されているフォルダ名
        public const string SkillPathFolder = "SkillData";
    }
}
