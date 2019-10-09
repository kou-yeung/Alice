using System;

namespace Zoo.IO
{
    public interface ILoader
    {
        /// <summary>
        /// Preloadを実行します
        /// </summary>
        void Preload(string[] paths, Action onloaded);

        /// <summary>
        /// Preloadしたアセットを返す
        /// </summary>
        T Load<T>(string path) where T : class;

        /// <summary>
        /// 非同期ロード
        /// </summary>
        void LoadAsync<T>(string path, Action<T> onloaded) where T : class;
    }

    public class LoaderService : ServiceLocator<ILoader>{}

}
