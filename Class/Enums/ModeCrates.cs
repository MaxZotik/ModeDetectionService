using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.Enums
{
    public enum ModeCrates : byte
    {
        NoData = 0,         //Нет данных
        NoMode = 1,         //Вне режима
        Rolling = 2,        //Прокат
        Idling = 3,         //Холостой ход
        Stop = 4            //Останов
    }
}
