using ModeDetectionService.Class.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.ClientTCP
{
    public class RepositoryDatabase
    {
        /// <summary>
        /// Словарь устройств МВК по IP адресам
        /// </summary>
        public static Dictionary<string, List<MVKDevice>> DatabaseDictionary { get; set; }

        /// <summary>
        /// Список ключей (IP адресов) устройств в словаре DatabaseDictionary
        /// </summary>
        public static List<int> KeysDictionary { get; set; }

        /// <summary>
        /// Временный список устройств МВК после опроса
        /// </summary>
        public static List<MVKDevice> WriteListDB { get; set; }

        

        static RepositoryDatabase()
        {
            DatabaseDictionary = GetDatabaseDictionary();
            WriteListDB = new List<MVKDevice>();
        }

        /// <summary>
        /// Метод заполнения словаря данными МВК по IP адресам
        /// </summary>
        /// <returns>Словарь МВК по номерам</returns>
        private static Dictionary<string, List<MVKDevice>> GetDatabaseDictionary()
        {
            Dictionary<string, List<MVKDevice>> dict = new Dictionary<string, List<MVKDevice>>();

            for (int i = 0; i < MVKDevice.MVKDevicesList.Count; i++)
            {
                if (!dict.ContainsKey(MVKDevice.MVKDevicesList[i].IP) || dict.Count == 0)
                {
                    dict.Add(MVKDevice.MVKDevicesList[i].IP, new List<MVKDevice>() { MVKDevice.MVKDevicesList[i] });
                }
                else
                {
                    dict[MVKDevice.MVKDevicesList[i].IP].Add(MVKDevice.MVKDevicesList[i]);
                }
            }

            return dict;
        }

        /// <summary>
        /// Метод получения списка ключей словаря DatabaseDictionary
        /// </summary>
        /// <returns>Список ключей словаря</returns>
        private static List<string> GetKeysDictionary()
        {
            List<string> keys = new List<string>();

            foreach (var key in DatabaseDictionary)
                keys.Add((string)key.Key);

            return keys;
        }

        /// <summary>
        /// Метод удаления записей из списка WriteListDB
        /// </summary>
        public static void WriteListDBClear()
        {
            WriteListDB.Clear();
        }


    }
}
