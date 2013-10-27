using System;
using System.Collections.Generic;
using System.Text;

using log4net;

public static class AppLog
{
    private const string LOG_REPOSITORY = "Default"; // this should likely be set in the web config.
    private static ILog m_log;

    public static void Init()
    {
        log4net.Config.XmlConfigurator.Configure();
    }

    public static void Write(string message, LogMessageType messageType)
    {
        DoLog(message, messageType, null, Type.GetType("System.Object"));
    }

    public static void Write(string message, LogMessageType messageType, Type type)
    {
        DoLog(message, messageType, null, type);
    }

    public static void Write(string message, LogMessageType messageType, Exception ex)
    {
        DoLog(message, messageType, ex, Type.GetType("System.Object"));
    }

    public static void Write(string message, LogMessageType messageType, Exception ex, Type type)
    {
        DoLog(message, messageType, ex, type);
    }

    public static void Assert(bool condition, string message)
    {
        Assert(condition, message, Type.GetType("System.Object"));
    }

    public static void Assert(bool condition, string message, Type type)
    {
        if (condition == false)
            Write(message, LogMessageType.Info);
    }

    private static void DoLog(string message, LogMessageType messageType, Exception ex, Type type)
    {
        m_log = LogManager.GetLogger(type);

        switch (messageType)
        {
            case LogMessageType.Debug:
                AppLog.m_log.Debug(message, ex);
                break;

            case LogMessageType.Info:
                AppLog.m_log.Info(message, ex);
                break;

            case LogMessageType.Warn:
                AppLog.m_log.Warn(message, ex);
                break;

            case LogMessageType.Error:
                AppLog.m_log.Error(message, ex);
                break;

            case LogMessageType.Fatal:
                AppLog.m_log.Fatal(message, ex);
                break;
        }
    }

    public enum LogMessageType
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
