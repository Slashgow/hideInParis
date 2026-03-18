using UnityEngine;

namespace inkolorgames
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] private bool showLogs;
        [SerializeField] private string prefix;
        [SerializeField] private Color prefixColor;

        private string hexColor;

        private void OnValidate()
        {
            hexColor = "#" + ColorUtility.ToHtmlStringRGBA(prefixColor);
        }

        public void Log(object message, Object sender)
        {
            if (!showLogs)
                return;

            Debug.Log($"<color={hexColor}>{prefix}</color>: {message}", sender);
        }

        public void LogError(object message, Object sender)
        {
            if (!showLogs)
                return;

            Debug.LogError($"<color={hexColor}>{prefix}</color>: {message}", sender);
        }

        public void LogWarning(object message, Object sender)
        {
            if (!showLogs)
                return;

            Debug.LogWarning($"<color={hexColor}>{prefix}</color>: {message}", sender);
        }
    }
}