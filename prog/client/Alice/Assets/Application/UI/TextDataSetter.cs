using Alice.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Alice.UI
{
    [RequireComponent(typeof(Text))]
    public class TextDataSetter : MonoBehaviour
    {
        [SerializeField] string TextID;

        private void Start()
        {
            var text = GetComponent<Text>();
            text.text = TextID.TextData();
        }
    }
}
