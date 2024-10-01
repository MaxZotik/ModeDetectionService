using ModeDetectionService.Class.FileSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModeDetectionService.Class.Enums;

namespace ModeDetectionService.Class.FileLoggings
{
    public class FileLogging
    {
        private static string path;
        private static readonly string directory = $@"Loggings\LogModeDetectionService";
        private static string nameFileService = "";
        private static readonly string fileExtension = ".log";
        private static readonly object _lockList = new object();

        public static List<string> FileLoggingsList {  get; set; }

        static FileLogging()
        {
            path = AppDomain.CurrentDomain.BaseDirectory;
            FileDirectory.CreateDirectory(path, directory);
            FileLoggingsList = new List<string>();
        }

        /// <summary>
        /// Метод записывает коллекцию логов в файл
        /// </summary>
        public static async void WriteLogFile()
        {
            try
            {
                CheckFile();

                string pathTemp = $@"{path}{directory}\{nameFileService}";

                if (FileLoggingsList.Count == 0)
                {
                    return;
                }

                List<string> list = new List<string>();
                //list.AddRange(FileLoggingsList);
                list = CopyList();
                FileLoggingsListClear();

                using (StreamWriter writer = new StreamWriter(pathTemp, true))
                {
                    try
                    {
                        foreach (string str in list)
                        {
                            await writer.WriteLineAsync(str);
                        }
                    }
                    catch (Exception ex)
                    {
                        new FileLogging().WriteLogAdd($"Ошибка записи в файл {nameFileService} логов службы! {ex.Message}", Enums.LoggingStatus.ERRORS);
                    }
                }
            }
            catch (Exception ex)
            {
                new FileLogging().WriteLogAdd($"Ошибка записи в файл {nameFileService} логов службы! Файл не записан! {ex.Message}", Enums.LoggingStatus.ERRORS);
            }

                                   
        }

        /// <summary>
        /// Метод записывает логи приложения в коллекцию
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="status">Статус сообщения</param>
        public void WriteLogAdd(in string message, LoggingStatus status = LoggingStatus.NOTIFY)
        {
            lock (_lockList)
            {
                string str = $"|{status}| {DateTime.Now} {message}";

                FileLoggingsList.Add(str);
            }           
        }

        private static List<string> CopyList()
        {
            lock ( _lockList)
            {
                List<string> list = new List<string>();

                list.AddRange(FileLoggingsList);

                return list;
            }          
        }


        /// <summary>
        /// Метод удаления записей из списка WriteListDB
        /// </summary>
        private static void FileLoggingsListClear()
        {
            lock (_lockList)
            {
                FileLoggingsList.Clear();
            }          
        }

        /// <summary>
        /// Метод конфигурирует и создает файл логов 
        /// </summary>
        private static void CheckFile()
        {
            string temp = DateTime.Now.ToString("yyyy-MM-dd") + fileExtension;

            if (nameFileService != temp)
            {
                nameFileService = temp;
            }
        }

    }
}
