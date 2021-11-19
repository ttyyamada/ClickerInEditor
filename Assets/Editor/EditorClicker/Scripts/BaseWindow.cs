using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Editor.EditorClicker.Data;
using Editor.EditorClicker.Scripts.Utils;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Editor.EditorClicker.Scripts
{
    public class BaseWindow : EditorWindow
    {
        private Texture cookieButton;
        private BaseWindow currentWindow;
        private ConfiguredData config;

        private UserData userData = new UserData();
        private readonly ShopData shopData = new ShopData();
        private readonly SaveService saveService = new SaveService();

        private List<SkillData> currentShopSkillList;
        private readonly List<CurrentAutoGetStatus> autoGetList = new List<CurrentAutoGetStatus>();
        private double currentClickCookie = 1;

        private readonly List<CookieAnimation> currentAnimationList = new List<CookieAnimation>();
        private double lastSaveTime = 0d;
        private double lastExecuteTime = 0d;

        [MenuItem("Clicker/BaseWindow")]
        private static void ShowWindow()
        {
            // ウィンドウを表示！
            GetWindow<BaseWindow>(StringList.title);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, currentWindow.position.size.x, currentWindow.position.size.y));
            // ショップの情報を表示する
            SetShopUI();
            // 現在の情報を並べる
            ShowCurrentInfo();

            // 画像をクリックしたらクッキーが増える
            if( GUILayout.Button(cookieButton) )
            {
                userData.currentCookies += ExecuteClick();
            }
            GUILayout.EndArea();

            // 経過時間の判定
            var elapsedTime = EditorApplication.timeSinceStartup - lastExecuteTime;
            lastExecuteTime = EditorApplication.timeSinceStartup;

            // 経過時間で文字のアニメーションを再生
            ExecuteNumberAnimation(elapsedTime);
            // 自動取得判定
            var autoGet = ExecuteAuto(elapsedTime);
            if (autoGet > 0)
            {
                // 位置は適当
                var pos = new Rect(Input.mousePosition, new Vector2(currentWindow.position.width, currentWindow.position.height));
                pos.x = currentWindow.position.width * 0.6f;
                var rnd = Random.Range(-pos.x * 0.5f, pos.x * 0.5f);
                pos.x += rnd;
                userData.currentCookies += autoGet;
                CreateCookieAnimation(pos, autoGet);
            }
            CheckAndDoSave();
        }

        /// <summary>
        /// 自動セーブをチェックしてセーブする
        /// </summary>
        private void CheckAndDoSave()
        {
            if (EditorApplication.timeSinceStartup - lastSaveTime >= config.saveTime)
            {
                lastSaveTime = EditorApplication.timeSinceStartup;
                saveService.Save(userData);
            }
        }

        /// <summary>
        /// ショップの内容を表示する
        /// </summary>
        private void SetShopUI()
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.Label(GetCurrentResourcesLabel());
                GUILayout.FlexibleSpace();
                
                GUILayout.BeginVertical("box");
                {
                    // ショップのアイテムを並べる
                    foreach (var skillData in currentShopSkillList)
                    {
                        var skill = skillData;
                        if (!GUILayout.Button($"{skill.skillName} {skill.buyCost}")) continue;
                        if (!(userData.currentCookies >= skill.buyCost)) continue;
                        userData.currentCookies -= skill.buyCost;
                        var boughtSkillNo = new List<int> {skill.skillNo};
                        SetSkillData(boughtSkillNo);
                        currentShopSkillList = shopData.CheckCurrentShowSkillData(userData.openSkillData);
                        RefreshData();
                    }
                }
                
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// 現在の情報を表示する
        /// </summary>
        private void ShowCurrentInfo()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Label($"クリック１回: {currentClickCookie} {StringList.resourcesName}");
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 文字のアニメーションを進める
        /// </summary>
        private void ExecuteNumberAnimation(double deltaTime)
        {
            var style = new GUIStyle();
            style.fontSize = 30;
            style.normal.textColor = Color.white;
            for(var i = currentAnimationList.Count - 1; i >= 0; i--)
            {
                var animation = currentAnimationList[i];
                GUI.Label(animation.NextStep(deltaTime), animation.number.ToString(CultureInfo.InvariantCulture), style);

                if (animation.IsComplete())
                {
                    currentAnimationList.Remove(animation);
                }
            }
        }

        /// <summary>
        /// 自動実行の処理を進める
        /// </summary>
        private double ExecuteAuto(double deltaTime)
        {
            double cookie = 0;
            autoGetList.ForEach(x => { cookie += x.ExecuteCookie(deltaTime); });
            return cookie;
        }

        /// <summary>
        /// クリックの実行
        /// </summary>
        private double ExecuteClick()
        {
            // 位置は適当
            var pos = new Rect(Input.mousePosition, new Vector2(currentWindow.position.width, currentWindow.position.height))
                {
                    x = currentWindow.position.width / 2f
                };
            // 2割ぐらいクリック一からずらす
            var rnd = Random.Range(-pos.x * 0.2f, pos.x * 0.2f);
            pos.x += rnd;
            CreateCookieAnimation(pos, currentClickCookie);
            return currentClickCookie;
        }
        

        private void Update()
        {
            Repaint();
        }

        private void OnEnable()
        {

            shopData.LoadSkillData();
            LoadUserData();
            SetResources();
            
            currentShopSkillList = shopData.CheckCurrentShowSkillData(userData.openSkillData);

            RefreshData();
            lastExecuteTime = EditorApplication.timeSinceStartup;
            lastSaveTime = EditorApplication.timeSinceStartup;
            currentWindow = this;
        }

        private void SetResources()
        {
            cookieButton = Resources.Load<Texture>(ResourcesNames.CookieButtonImagePath);
            config = new ConfiguredData();
        }

        /// <summary>
        /// データの再読み込みを行う
        /// </summary>
        private void RefreshData()
        {
            SetClickCookieRate();
            SetAutoExecuteCookie();
            RefreshShopData();
        }

        private void RefreshShopData()
        {
            shopData.CheckCurrentShowSkillData(userData.openSkillData);
        }

        private void LoadUserData()
        {
            userData = saveService.Load();
            if (userData != null)
            {
                SetSkillData(userData.openSkillIndex);
                return;
            }
            Debug.LogWarning("UserDataがnullなので新規作成します");
            userData = new UserData();
            SetSkillData(DefaultData.skillNo.ToList());
        }

        private void SetSkillData(List<int> skillIdList)
        {
            foreach (var id in skillIdList)
            {
                var skill = shopData.GetSkillData(id);
                if (skill == null)
                {
                    Debug.LogWarning("対象のスキルはnullです！ スキルナンバー：" + id);
                    continue;
                }

                if (!userData.openSkillData.Contains(skill))
                {
                    userData.openSkillData.Add(skill);
                }
                
                if (!userData.openSkillIndex.Contains(skill.skillNo))
                {
                    userData.openSkillIndex.Add(skill.skillNo);
                }
            }
        }

        private string GetCurrentResourcesLabel()
        {
            return $"{StringList.resourcesName} : {userData.currentCookies} {StringList.unit}";
        }

        private void SetAutoExecuteCookie()
        {
            userData.openSkillData.ForEach(x =>
            {
                if (autoGetList.Find(y => y.num == x.skillNo) == null)
                {
                    autoGetList.Add(x.ConvertToAutoGetStatus());
                }
            });
        }

        /// <summary>
        /// クッキーのアニメーションを生成しリストに登録する
        /// </summary>
        private void CreateCookieAnimation(Rect fromRect, double number)
        {
            var targetY = fromRect.y + currentWindow.position.yMax / 3.5f;
            var targetRect = new Rect(fromRect.x, targetY, fromRect.width, fromRect.height);
            var cookieAnimation = new CookieAnimation(fromRect, targetRect, number, config.animationSpeed);
            
            currentAnimationList.Add(cookieAnimation);
            
        }

        /// <summary>
        /// クッキークリック一回のレートを設定する
        /// </summary>
        private void SetClickCookieRate()
        {
            currentClickCookie = 0;
            userData.openSkillData.ForEach(x =>
            {
                currentClickCookie += x.clickRate;
            });
        }
    }
}