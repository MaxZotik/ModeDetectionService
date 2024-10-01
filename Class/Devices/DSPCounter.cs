using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.Devices
{
    public class DSPCounter
    {
        public string IPAddress { get; }

        public string Port { get; }
        public string Endians { get; }
        public int StartAddress { get; }
        public UInt32 Parameters { get; set; }
        //public int NumberMVK { get; }
        //public int Crate { get; }

        //public static List<DSPCounter> CounterList { get; set; }

        public static List<DSPCounter> MVKCounterList { get; set; }

        static DSPCounter()
        {
            //CounterList = GetCounterList();
            MVKCounterList = GetMVKCounterList();
        }


        //public DSPCounter(string ipAddress, string port, string endians, int crate, int numberMVK, int startAddress = 4132, UInt32 parameter = 0)
        //{
        //    IPAddress = ipAddress;
        //    Port = port;
        //    Endians = endians;
        //    NumberMVK = numberMVK;
        //    Crate = crate;
        //    StartAddress = startAddress;
        //    Parameters = parameter;
        //}

        public DSPCounter(string ipAddress, string port, string endians, int startAddress = 4132, UInt32 parameter = 0)
        {
            IPAddress = ipAddress;
            Port = port;
            Endians = endians;
            StartAddress = startAddress;
            Parameters = parameter;
        }

        //private static List<DSPCounter> GetCounterList()
        //{
        //    List<DSPCounter> list = new List<DSPCounter>();

        //    List<MVKDevice> newList = new List<MVKDevice>();

        //    newList.InsertRange(0, MVKDevice.MVKDevicesList);

        //    for (int i = 0; i < newList.Count; i++)
        //    {
        //        for (int k = (i + 1); k < newList.Count; k++)
        //        {
        //            if (newList[i].Crate == newList[k].Crate && newList[i].NumberMVK == newList[k].NumberMVK)
        //            {
        //                newList.RemoveAt(k);
        //            }
        //        }
        //    }

        //    for (int i = 0; i < newList.Count; i++)
        //    {
        //        list.Add(new DSPCounter(
        //            MVKDevice.MVKDevicesList[i].IP,
        //            MVKDevice.MVKDevicesList[i].Port,
        //            MVKDevice.MVKDevicesList[i].Endian,
        //            int.Parse(MVKDevice.MVKDevicesList[i].Crate),
        //            int.Parse(MVKDevice.MVKDevicesList[i].NumberMVK)));
        //    }

        //    return list;
        //}


        private static List<DSPCounter> GetMVKCounterList()
        {
            List<DSPCounter> list = new List<DSPCounter>();

            List<MVKDevice> newList = new List<MVKDevice>();

            newList.InsertRange(0, MVKDevice.MVKDevicesList);

            for (int i = 0; i < newList.Count; i++)
            {
                for (int k = (i + 1); k < newList.Count; k++)
                {
                    if (newList[i].IP == newList[k].IP)
                    {
                        newList.RemoveAt(k);
                        k--;
                    }
                }
            }

            for (int i = 0; i < newList.Count; i++)
            {
                list.Add(new DSPCounter(
                    newList[i].IP,
                    newList[i].Port,
                    newList[i].Endian));
            }

            return list;
        }

        //public static int GetIndexOfKey(int key)
        //{
        //    int index = 0;

        //    for (int i = 0; i < CounterList.Count; i++)
        //    {
        //        if (CounterList[i].NumberMVK == key)
        //        {
        //            index = i;
        //            break;
        //        }
        //    }

        //    return index;
        //}
    }
}
