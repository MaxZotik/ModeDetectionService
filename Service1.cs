using ModeDetectionService.Class.FileLoggings;
using ModeDetectionService.Class.ModbusTCP.ClientTCP;
using ModeDetectionService.Class.ModbusTCP.ServerTCP;
using ModeDetectionService.Class.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModeDetectionService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        private Thread[] threads;
        private readonly int countThread = WorkClient.MyModbusTCPList.Count;
        private readonly ModbusServer modbusServer = new ModbusServer();
        private readonly WorkCrate workCrate = new WorkCrate();


        protected override void OnStart(string[] args)
        {
            Thread threadServer = new Thread(ReadServer);
            threadServer.Start();

            Thread threadClient = new Thread(ReadClient);
            threadClient.Start();

            Thread threadsWrite = new Thread(WriteLog);
            threadsWrite.Start();
        }

        protected override void OnStop()
        {
            new FileLogging().WriteLogAdd($"Служба \"Mode Detection Service\" остановлена!", LoggingStatus.ACTION);
            Thread threadsWrite = new Thread(WriteLog);
            threadsWrite.Start();
        }

        protected void ReadClient()
        {
            while (true)
            {
                RepositoryDatabase.WriteListDBClear();

                Stopwatch timer = Stopwatch.StartNew();

                threads = new Thread[countThread];

                for(int i = 0; i < countThread; i++)
                {

                    threads[i] = new Thread(WorkClient.MyModbusTCPList[i].MVKtempOne)
                    {
                        Name = "Service: " + (i + 1).ToString(),
                        Priority = ThreadPriority.Highest
                    };
                    threads[i].Start();
                }

                foreach (Thread th in threads)
                {
                    th.Join();
                }

                Thread threadWorkCrate = new Thread(workCrate.ActionWorkCrate)
                {
                    Name = "Service ActionWorkCrate",
                    Priority = ThreadPriority.Normal
                };
                threadWorkCrate.Start();
                threadWorkCrate.Join();

                //Thread threadWriteCrate = new Thread(FileLogging.WriteLogFile)
                //{
                //    Name = "Service WriteLogFile",
                //    Priority = ThreadPriority.Normal
                //};
                //threadWriteCrate.Start();
                //threadWriteCrate.Join();

                timer.Stop();
                int time = Convert.ToInt32(timer.ElapsedMilliseconds);

                int timeSleep = 485;

                if (time < timeSleep)
                    timeSleep -= time;

                Thread.Sleep(timeSleep);
            }
        }

        protected void ReadServer()
        {
            modbusServer.StartServer();
        }

        protected void WriteLog() 
        {
            while (true)
            {
                Stopwatch timer = Stopwatch.StartNew();

                Thread threadWriteCrate = new Thread(FileLogging.WriteLogFile)
                {
                    Name = "Service WriteLogFile",
                    Priority = ThreadPriority.Normal
                };
                threadWriteCrate.Start();
                threadWriteCrate.Join();

                timer.Stop();
                int time = Convert.ToInt32(timer.ElapsedMilliseconds);

                int timeSleep = 990;

                if (time < timeSleep)
                    timeSleep -= time;

                Thread.Sleep(timeSleep);
            }
        }
            
    }
}
