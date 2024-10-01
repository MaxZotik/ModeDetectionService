using ModeDetectionService.Class.Devices;
using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.ClientTCP
{
    public class RepositoryCrate
    {
        public List<MVKDevice> MVKDevicesList;

        public RepositoryCrate()
        {
            MVKDevicesList = new List<MVKDevice>();
        }

        public void SetModeCrate()
        {            
            for (int i = 0; i < Crate.CratesList.Count; i++)
            {
                int indexCKO = -1;
                int indexSpeed = -1;

                if (MVKDevicesList.Count != 0)
                {
                    for (int k = 0; k < MVKDevicesList.Count; k++)
                    {
                        if (Crate.CratesList[i].NumberCrate == int.Parse(MVKDevicesList[k].Crate))
                        {
                            if (MVKDevicesList[k].Parameter == "Частота вращения")
                            {
                                indexSpeed = k;
                            }
                            else
                            {
                                indexCKO = k;
                            }
                        }
                    }

                    if (indexCKO != -1 && indexSpeed != -1)
                    {
                        Crate.CratesList[i].DefineModeCrate(MVKDevicesList[indexCKO].Value, MVKDevicesList[indexSpeed].Value);
                    }
                }
            }            
        }

        public void SetMVKDevicesList()
        {
            if (RepositoryDatabase.WriteListDB.Count != 0)
            {
                MVKDevicesList.AddRange(RepositoryDatabase.WriteListDB);
            }           
        }

        public void MVKDevicesListClear()
        {
            MVKDevicesList.Clear();
        }
    }
}
