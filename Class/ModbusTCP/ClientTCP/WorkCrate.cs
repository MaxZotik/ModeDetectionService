using ModeDetectionService.Class.Devices;
using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;
using ModeDetectionService.Class.OperatingMode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.ClientTCP
{
    public class WorkCrate
    {
        private readonly RepositoryCrate repositoryCrate;

        public WorkCrate() 
        {
            repositoryCrate = new RepositoryCrate();
        }

        public void ActionWorkCrate()
        {
            repositoryCrate.SetMVKDevicesList();
            repositoryCrate.SetModeCrate();
            repositoryCrate.MVKDevicesListClear();
            //new Thread(() => PrintProcess()).Start();

            StartActionProcess();            
        }

        private void StartActionProcess()
        {
            for (int i = 0; i < Crate.CratesList.Count; i++)
            {
                var crate = Crate.CratesList[i] as Crate;

                if (crate.ValuesList.Count > 0 && !crate.CrateStartTime.TimeOut)
                {
                    new ActionProcess(crate);
                    new Thread(() => PrintProcess()).Start();
                }
            }
        }

        public static void PrintProcess()
        {
            foreach (var crate in Crate.CratesList)
            {
                new FileLogging().WriteLogAdd($"Клеть: {crate.NumberCrate} | Режим: {crate.SetModeCrate} | {crate.CrateStartTime.TimeOut} | Значение: {crate.CurrentValue} | ХХ: {crate.SetPointCrateIdling} | Прокат: {crate.SetPointCrateRolling}", LoggingStatus.NOTIFY);
            }
        }
    }
}
