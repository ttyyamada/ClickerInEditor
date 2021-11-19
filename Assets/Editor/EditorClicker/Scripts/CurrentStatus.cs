namespace Editor.EditorClicker.Scripts
{
    public class CurrentAutoGetStatus
    {
        public int num;
        public double autoRate;
        public float getTime;
        private double currentTime;

        /// <summary>
        /// 自動取得を実行する
        /// </summary>
        public double ExecuteCookie(double addTime)
        {
            currentTime += addTime;
            if (currentTime >= getTime)
            {
                currentTime = 0f;
                return autoRate;
            }

            return 0;
        }
    }
}
