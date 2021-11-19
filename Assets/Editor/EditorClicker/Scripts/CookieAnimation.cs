using UnityEngine;

namespace Editor.EditorClicker.Scripts
{
    public class CookieAnimation
    {
        private Rect targetRect;
        private Rect currentRect;
        public double number;
        private float step;
        private float animationSpeed = 3.0f;

        public CookieAnimation(Rect targetRect,Rect fromRect, double number,float animationSpeed)
        {
            this.targetRect = targetRect;
            this.number = number;
            this.currentRect = fromRect;
            this.animationSpeed = animationSpeed / 10f;
            step = 0;
        }

        /// <summary>
        /// アニメーションのステップを実行する
        /// </summary>
        public Rect NextStep(double deltaTime)
        {
            step += (float)deltaTime * animationSpeed;
           
            var y = Mathf.Lerp(currentRect.y, targetRect.y, step);
            return new Rect(currentRect.x, y, currentRect.width, currentRect.height);
        }

        /// <summary>
        /// アニメーションの終了判定
        /// </summary>
        public bool IsComplete()
        {
            return step >= 0.98f;
        }
    }
}
