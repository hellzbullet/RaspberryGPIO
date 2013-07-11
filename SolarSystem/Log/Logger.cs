using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSystemDatabase.Database;
using SolarSystemDatabase.Models;

namespace SolarSystem.Log
{
    public class Logger
    {
        public const string LoggerDirectoryPath = "Log/";
        public const string LoggerFilePath = "log";
        public const string LoggerPath = LoggerDirectoryPath + LoggerFilePath;

        public static Lazy<Logger> Instance = new Lazy<Logger>();

        private static object Lock = new object();

        public Logger()
        {
            SetUpFiles();
        }

        private void SetUpFiles()
        {
            lock (Lock) {
                if (!Directory.Exists(LoggerDirectoryPath)) Directory.CreateDirectory(LoggerDirectoryPath);
                if (!File.Exists(LoggerPath))
                {
                    File.Create(LoggerPath);
                    Log("File created.");
                }
            }
        }

        public void Log(string message)
        {
            Log(message, LogType.NORMAL);
        }

        public void Log(string message, LogType type)
        {
            lock (Lock) {
                var strToAppend = DateTime.Now.ToString();

                if (type == LogType.ERROR) strToAppend += " " + type;
                strToAppend += " " + message + Environment.NewLine;

                File.AppendAllText(LoggerPath, strToAppend);

                using (var session = NHibernateConfig.SessionFactory.Value.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.Save(new LogEntry() { Date = DateTime.Now.Ticks, Message = message, Type = type.ToString() });
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
