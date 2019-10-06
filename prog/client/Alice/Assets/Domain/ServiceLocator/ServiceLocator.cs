/// <summary>
/// サービスロケータ
/// </summary>
namespace Zoo
{
    public abstract class ServiceLocator<T>
    {
        public static T Instance { get; private set; }
        public static void SetLocator(T t) { Instance = t; }
    }

    /*
     * 使用例
     * 
    public interface IAudioService
    {
        void Play(string fn);
        void Stop();
    }

    public class iOSAudio : IAudioService
    {
        public void Play(string fn) { Debug.Log($"iOSAudio : {fn}"); }
        public void Stop() { Debug.Log($"iOSAudio : Stop"); }
    }

    public class AudioService : ServiceLocator<IAudioService> { }
    
    AudioService.SetLocator(new iOSAudio());
    AudioService.Instance.Play("Hello!!");
    */
}
