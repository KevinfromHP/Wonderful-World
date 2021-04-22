using BepInEx.Logging;
using System.Runtime.CompilerServices;


namespace ForgottenFoes.Utils
{
    public static class LogCore
    {
        public static ManualLogSource logger = null;
        public static void LogD(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogDebug(string.Format("ForgottenFoes :: {0} :: Line: {1}, Method {2}", data, i, member));
        }
        public static void LogE(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogError(string.Format("ForgottenFoes :: {0} :: Line: {1}, Method {2}", data, i, member));
        }
        public static void LogF(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogFatal(string.Format("ForgottenFoes ::{0} :: Line: {1}, Method {2}", data, i, member));
        }
        public static void LogI(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogInfo(string.Format("ForgottenFoes :: {0} :: Line: {1}, Method {2}", data, i, member));
        }
        public static void LogM(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogMessage(string.Format("ForgottenFoes :: {0} :: Line: {1}, Method {2}", data, i, member));
        }
        public static void LogW(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogWarning(string.Format("ForgottenFoes :: {0} :: Line: {1}, Method {2}", data, i, member));
        }
    }
}
