using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.FileSettings
{
    static class FileDirectory
    {
        /// <summary>
        /// Метод проверяет наличие директории в корне программы и создает директорию если ее нет
        /// </summary>
        /// <param name="path">Корневой путь</param>
        /// <param name="directory">Название директории</param>
        public static void CreateDirectory(string path, string directory)
        {
            string pathTemp = $@"{path}\{directory}";

            if (!Directory.Exists(pathTemp))
            {
                Directory.CreateDirectory(pathTemp);
            }
        }

        /// <summary>
        /// Метод проверяет наличие файла в директории и создает файл если его нет
        /// </summary>
        /// <param name="path">Корневой путь</param>
        /// <param name="file">Название файла</param>
        public static void CreateFile(string path, string directory, string file)
        {
            string pathTemp = $@"{path}\{directory}\{file}";

            if (!File.Exists(pathTemp))
            {
                File.Create(pathTemp);
            }
        }

        /// <summary>
        /// Метод проверяет наличие файла в директории и удаляет файл если файл существует
        /// </summary>
        /// <param name="pathName">Путь и название файла</param>
        public static void DeleteFile(string pathName)
        {
            if (File.Exists(pathName))
            {
                File.Delete(pathName);
            }
        }
    }
}
