///=========================================
/// UnityEditor.UI.MenuOptions の HACK です
/// メニューからUI を配置したら、イベントを受け取るように自前処理をしたいときに使用します
/// 
/// 参考 : https://github.com/tenpn/unity3d-ui/blob/master/UnityEditor.UI/UI/MenuOptions.cs
///=========================================
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.IO;
using System;
using System.Linq;
using Xyz.AnzFactory.UI;

namespace UnityEditor.UI
{
    public static class MenuOptions
    {
#if UNITY_2019_3_OR_NEWER
        static readonly string Path2DLL = Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), @"Data\Resources\PackageManager\ProjectTemplates\libcache\com.unity.template.2d-3.3.0\ScriptAssemblies\UnityEditor.UI.dll");
#elif UNITY_2019_1_OR_NEWER
        static readonly string Path2DLL = Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), @"Data\Resources\PackageManager\ProjectTemplates\libcache\com.unity.template.2d-3.2.0\ScriptAssemblies\UnityEditor.UI.dll");
#else
        static readonly string Path2DLL = Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), @"Data\UnityExtensions\Unity\GUISystem\Editor\UnityEditor.UI.dll");
#endif
        static readonly string TypeName = @"UnityEditor.UI.MenuOptions";

        // ボタン作成時のコールバック
        public static event Action<GameObject> OnCreatedButton = (obj) => { };
        public static event Action<GameObject> OnCreatedScrollRect = (obj) => { };

        static Type Type
        {
            get { return Assembly.LoadFile(Path2DLL).GetType(TypeName); }
        }

        [MenuItem("GameObject/UI/Button", false, 2001)]
        static void AddButton(MenuCommand menuCommand)
        {
            /// MEMO : 元の実装を呼び出す
            var method = Type.GetMethod("AddButton");
            method.Invoke(null, new object[] { menuCommand });

            /// MEMO : ヒエラルキーに選択されているオブジェクトを取得し、先頭オブジェクトのコンポーネントを取得する
            var go = Selection.gameObjects.First();
            OnCreatedButton(go);
        }

        [MenuItem("GameObject/UI/Table View", false, 2050)]
        static void AddTableView(MenuCommand menuCommand)
        {
            /// MEMO : 元の実装を呼び出す
            var method = Type.GetMethod("AddScrollView");
            method.Invoke(null, new object[] { menuCommand });

            /// MEMO : ヒエラルキーに選択されているオブジェクトを取得し、先頭オブジェクトのコンポーネントを取得する
            var go = Selection.gameObjects.First();
            go.name = "Table View";
            go.GetComponent<ScrollRect>().horizontal = false;   // 水平方向のスクロール制限
            go.AddComponent<ANZListView>(); // ANZListView追加しておく

            OnCreatedScrollRect(go);
        }

        [MenuItem("GameObject/UI/Cell View", false, 2051)]
        static void AddCellView(MenuCommand menuCommand)
        {
            /// MEMO : 元の実装を呼び出す
            var method = Type.GetMethod("AddScrollView");
            method.Invoke(null, new object[] { menuCommand });

            /// MEMO : ヒエラルキーに選択されているオブジェクトを取得し、先頭オブジェクトのコンポーネントを取得する
            var go = Selection.gameObjects.First();
            go.name = "Cell View";
            go.GetComponent<ScrollRect>().horizontal = false;   // 水平方向のスクロール制限
            go.AddComponent<ANZCellView>(); // ANZCellView追加しておく

            OnCreatedScrollRect(go);
        }
    }
}