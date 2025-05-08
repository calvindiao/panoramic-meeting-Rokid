using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Rokid.UXR.Demo
{
    public class LogManager : MonoBehaviour
    {
        public static LogManager Instance { get; private set; }
        
        [SerializeField] private Text logDisplayText; // 可选：用于在应用内显示日志
        [SerializeField] private ScrollRect logScrollRect; // 可选：如果有滚动视图
        
        private StringBuilder logBuilder = new StringBuilder();
        private string logFilePath;
        private bool isLogFileInitialized = false;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitLogFile();
            }
            else
            {
                Destroy(gameObject);
            }
            
            // 注册日志回调，捕获所有Debug.Log调用
            Application.logMessageReceived += HandleLog;
        }
        
        private void InitLogFile()
        {
            try
            {
                // 确保有权限写入外部存储
                if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
                {
                    UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
                }
                
                // 创建日志文件路径
                string folderPath = Path.Combine(Application.persistentDataPath, "Logs");
                
                // 确保目录存在
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                
                // 创建带时间戳的日志文件名
                string fileName = $"Log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
                logFilePath = Path.Combine(folderPath, fileName);
                
                // 写入初始日志头
                File.WriteAllText(logFilePath, $"=== 日志开始 - {DateTime.Now} ===\n");
                
                isLogFileInitialized = true;
                Log("日志系统初始化成功，日志文件路径: " + logFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError("初始化日志文件失败: " + e.Message);
            }
        }
        
        public void HandleLog(string logString, string stackTrace, LogType type)
        {
            string formattedLog = $"[{DateTime.Now:HH:mm:ss}][{type}] {logString}";
            
            // 添加到字符串构建器
            logBuilder.AppendLine(formattedLog);
            
            // 更新UI（如果有）
            if (logDisplayText != null)
            {
                logDisplayText.text = logBuilder.ToString();
                // 自动滚动到底部
                if (logScrollRect != null)
                    Canvas.ForceUpdateCanvases();
                    logScrollRect.verticalNormalizedPosition = 0f;
            }
            
            // 写入到文件
            if (isLogFileInitialized)
            {
                try
                {
                    File.AppendAllText(logFilePath, formattedLog + "\n");
                    
                    // 如果是错误或异常，还可以写入堆栈信息
                    if (type == LogType.Error || type == LogType.Exception)
                    {
                        File.AppendAllText(logFilePath, stackTrace + "\n");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("写入日志文件失败: " + e.Message);
                }
            }
        }
        
        public void Log(string message)
        {
            Debug.Log(message);
        }
        
        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }
        
        public void LogError(string message)
        {
            Debug.LogError(message);
        }
        
        // 手动保存日志文件的方法
        public void SaveLogToFile()
        {
            try
            {
                if (isLogFileInitialized)
                {
                    File.WriteAllText(logFilePath, logBuilder.ToString());
                    Debug.Log("日志已保存到: " + logFilePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("保存日志失败: " + e.Message);
            }
        }
        
        private void OnDestroy()
        {
            // 移除回调
            Application.logMessageReceived -= HandleLog;
            
            // 在应用关闭时添加结束标记
            if (isLogFileInitialized)
            {
                File.AppendAllText(logFilePath, $"\n=== 日志结束 - {DateTime.Now} ===\n");
            }
        }
    }
}