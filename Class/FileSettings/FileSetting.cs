using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ModeDetectionService.Class.Devices;
using ModeDetectionService.Class.FileLoggings;

namespace ModeDetectionService.Class.FileSettings
{
    public static class FileSetting
    {
        private static string path;
        private static readonly string directory = "Settings";
        private static readonly string mvkSettings = "MVKSettings.xml";
        private static readonly string serviceSettings = "ServiceSettings.xml";

        static FileSetting()
        {
            //path = Directory.GetCurrentDirectory();
            path = AppDomain.CurrentDomain.BaseDirectory;
            FileDirectory.CreateDirectory(path, directory);
        }
                
        /// <summary>
        /// Метод загрузки настроек устройств МВК из файла
        /// </summary>
        /// <returns>Список устройств МВК</returns>
        public static List<MVKDevice> MVKSettingLoad()
        {
            string pathFull = $@"{path}{directory}\{mvkSettings}";

            List<MVKDevice> list = new List<MVKDevice>();

            if (File.Exists(pathFull))
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(pathFull);
                    var root = xmlDocument.DocumentElement;

                    if (root != null)
                    {
                        foreach (XmlElement elem in root)
                        {
                            list.Add(new MVKDevice(
                            ip: elem.ChildNodes[0].InnerText,
                            port: elem.ChildNodes[1].InnerText,
                            endian: elem.ChildNodes[2].InnerText,
                            crate: elem.ChildNodes[3].InnerText,
                            numberMVK: elem.ChildNodes[4].InnerText,
                            channel: elem.ChildNodes[5].InnerText,
                            frequency: elem.ChildNodes[6].InnerText,
                            parameter: elem.ChildNodes[7].InnerText,
                            address: elem.ChildNodes[8].InnerText));
                        }
                    }
                }
                catch(Exception ex)
                {
                    new FileLogging().WriteLogAdd($"Ошибка чтения файла {mvkSettings} настроек устройств МВК! {ex.Message}", Enums.LoggingStatus.ERRORS);
                }               
            }

            return list;
        }

        /// <summary>
        /// Метод загрузки настроек параметров для расчета режима работы клетей
        /// </summary>
        /// <returns>Список настроек параметров</returns>
        public static List<string> ServiceSettingLoad()
        {
            string pathFull = $@"{path}{directory}\{serviceSettings}";

            List<string> list = new List<string>();

            if (File.Exists(pathFull))
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(pathFull);
                    var root = xmlDocument.DocumentElement;

                    if (root != null)
                    {
                        foreach (XmlElement element in root)
                            list.Add(element.InnerText);
                    }
                }
                catch (Exception ex)
                {
                    new FileLogging().WriteLogAdd($"Ошибка чтения файла {serviceSettings} настроек службы! {ex.Message}", Enums.LoggingStatus.ERRORS);
                }               
            }

            return list;
        }
    }
}
