using Newtonsoft.Json;
using UnityEngine;
using System;
using System.IO;


public class LogHandler : MonoBehaviour
{
    private string logFilePath;

    void Start()
    {
        Application.logMessageReceived += HandleLog;

        // Define the path where the JSON log file will be saved
        logFilePath = Path.Combine(Application.persistentDataPath, "error_logs.json");
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            LogError(logString, stackTrace);
        }
    }

    void LogError(string message, string stackTrace)
    {
        var errorData = new
        {
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            message = message,
            stackTrace = stackTrace.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
        };

        string errorJson = JsonConvert.SerializeObject(errorData, Formatting.Indented);

        string logFilePath = Path.Combine(Application.persistentDataPath, "error_logs.json");

        using (StreamWriter streamWriter = File.AppendText(logFilePath))
        {
            streamWriter.WriteLine(errorJson);
        }

        Debug.LogError($"Error logged: {errorJson}");
    }

}
