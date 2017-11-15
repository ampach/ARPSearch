using System;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;

namespace ARPSearch.Logging
{
    public static class Log
    {
        private static volatile ILogService _instance;
        private static object syncRoot = new Object();

        private static ILogService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new LogService();
                    }
                }

                return _instance;
            }
        }

        public static void Debug(string message){
            
            Instance.Debug(message);
        }

        public static void Debug(string message, Exception exception)
        {
            Instance.Debug(message, exception);
        }

        public static void Info(string message)
        {
            Instance.Info(message);
        }

        public static void Info(string message, Exception exception)
        {
            Instance.Info(message, exception);
        }

        public static void Warn(string message)
        {
            Instance.Warn(message);
        }

        public static void Warn(string message, Exception exception)
        {
            Instance.Warn(message, exception);
        }

        public static void Error(string message)
        {
            Instance.Error(message);
        }

        public static void Error(string message, Exception exception)
        {
            Instance.Error(message, exception);
        }

        public static void Fatal(string message)
        {
            Instance.Fatal(message);
        }

        public static void Fatal(string message, Exception exception)
        {
            Instance.Fatal(message, exception);
        }

    }
}