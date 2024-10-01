using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;
using ModeDetectionService.Class.ModbusTCP.ClientTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.Test
{
    public class CSVfile
    {
        private static string path;
        private static string directoryValueIntervals = $@"FileSave\ValueIntervals";
        private static string directoryValueAll = $@"FileSave\ValueAll";
        private static string fileExtension = ".csv";

        public string Number {  get; set; }
        public List<CSVobject> CSVobjectList { get; set; }
        public List<CSVobjectAll> CSVobjectAllList { get; set; }

        public ClientTimeOut TimeWrite { get; set; }

        public ClientTimeOut TimeCreateFile { get; set; }

        public string NameFile {  get; set; }

        static CSVfile()
        {
            path = AppDomain.CurrentDomain.BaseDirectory;
            CreateDirectory();
        }

        public CSVfile(int number) 
        {
            Number = "Crate - " + number.ToString();
            CSVobjectList = new List<CSVobject>();
            CSVobjectAllList = new List<CSVobjectAll>();
            NameFile = CreateNameFile();
            TimeWrite = new ClientTimeOut(10);
            TimeCreateFile = new ClientTimeOut(3600);
        }

        private string CreateNameFile()
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff");
            return time.Replace(":", "-") + "_" + Number;
        }


        private static void CreateDirectory()
        {
            string[] pathTemp = new string[] { $@"{path}\{directoryValueIntervals}", $@"{path}\{directoryValueAll}" };

            foreach (string path in pathTemp)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            
        }

        public async void WriteFileCVSobject()
        {
            if (CSVobjectList.Count == 0)
            {
                return;
            }

            List<CSVobject> csvobjectList = new List<CSVobject>();
            csvobjectList.AddRange(CSVobjectList);
            CSVobjectListClear();


            string fileNane = csvobjectList[0].Time.Replace(":", "-") + "_" + Number;

            string pathTemp = $@"{path}\{directoryValueIntervals}\{fileNane}{fileExtension}";

            string textHead = $@"Время;Виброускорение, 10..1000 Гц;Частота вращения";

            try
            {
                using (StreamWriter sw = new StreamWriter(pathTemp, false, System.Text.Encoding.UTF8))
                {               
                    await sw.WriteLineAsync(Number);
                    await sw.WriteLineAsync(textHead);

                    foreach (var cvs in csvobjectList)
                    {
                        await sw.WriteLineAsync(cvs.ToString());
                    }                             
                }
            }
            catch (Exception ex)
            {
                new FileLogging().WriteLogAdd($"Ошибка записи в файл {fileNane}! {ex.Message}", LoggingStatus.ERRORS);
            }
        }

        private async void CreateFile()
        {
            string pathTemp = $@"{path}\{directoryValueAll}\{NameFile}{fileExtension}";
            string textHead = $@"Время;Виброускорение, 10..1000 Гц;Частота вращения;Уставка: ХХ;Уставка: Прокат;Режим работы";

            using (StreamWriter sw = new StreamWriter(pathTemp, false, System.Text.Encoding.UTF8))
            {
                await sw.WriteLineAsync(Number);
                await sw.WriteLineAsync(textHead);
            }
        }

        public async void WriteFileCVSobjectAll()
        {
            if (CSVobjectAllList.Count == 0 || TimeWrite.TimeOut)
            {
                return;
            }
            else if (!TimeCreateFile.TimeOut)
            {
                NameFile = CreateNameFile();
                CreateFile();
            }

            List<CSVobjectAll> csvobjectList = new List<CSVobjectAll>();
            csvobjectList.AddRange(CSVobjectAllList);
            CSVobjectAllListClear();

            string pathTemp = $@"{path}\{directoryValueAll}\{NameFile}{fileExtension}";

            try
            {
                using (StreamWriter sw = new StreamWriter(pathTemp, true, System.Text.Encoding.UTF8))
                {              
                    foreach (var cvs in csvobjectList)
                    {
                        await sw.WriteLineAsync(cvs.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                new FileLogging().WriteLogAdd($"Ошибка записи в файл {NameFile}! {ex.Message}", LoggingStatus.ERRORS);
            }
        }


        public void CSVobjectListAdd(string value, string rotation)
        {
            CSVobjectList.Add(new CSVobject(value, rotation));
        }

        public void CSVobjectAllListAdd(string value, string rotation, string setPointCrateIdling, string setPointCrateRolling, string setModeCrate)
        {
            CSVobjectAllList.Add(new CSVobjectAll(value, rotation, setPointCrateIdling, setPointCrateRolling, setModeCrate));
        }

        public void CSVobjectListClear()
        {
            CSVobjectList.Clear();
        }

        public void CSVobjectAllListClear()
        {
            CSVobjectAllList.Clear();
        }
    }
}
