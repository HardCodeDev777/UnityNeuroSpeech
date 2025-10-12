using System.Reflection;

namespace UnityNeuroSpeech.Utils
{
    internal static class ReflectionUtils
    {
        public static void ChangePrivateField(this object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(obj, value);
        }
    }
}
