using System.IO;
using UnityEngine;
using Whisper;

namespace UnityNeuroSpeech.Runtime
{
    public class SetupWhisperPath : MonoBehaviour
    {
        [SerializeField] private WhisperManager _whisper;
        [SerializeField] private string _modelPath;

        private async void Awake()
        {
            var path = Path.Combine(Application.dataPath, _modelPath);
            _whisper.ModelPath = path.Replace("\\", "/");
            await _whisper.InitModel();
        }
    }
}
