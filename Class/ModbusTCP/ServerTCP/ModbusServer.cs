using ModeDetectionService.Class.Constants;
using ModeDetectionService.Class.Devices;
using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;
using ModeDetectionService.Class.OperatingMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.ServerTCP
{
    public class ModbusServer
    {
        private Socket serverSocket;
        private readonly int port;
        //private bool isConnect;

        public ModbusServer() 
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.port = SettingsProcess.PORT_SERVER; 
            //isConnect = false;
        }

        public void StartServer()
        {
            //if (!isConnect)
            //{
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(Constant.BACKLOG);
                //isConnect = true;

                new FileLogging().WriteLogAdd($"Сервер запущен!", LoggingStatus.NOTIFY);

                while (true)
                {
                    try
                    {
                        Socket clientSocket = serverSocket.Accept();
                        new Thread(() => ProcessClient(clientSocket)).Start();
                    }
                    catch (Exception ex)
                    {
                        //isConnect = false;
                        new FileLogging().WriteLogAdd($"Ошибка запуска сервера! {ex.Message}", LoggingStatus.NOTIFY);
                    }
                }
            //}       
        }


        private void ProcessClient(Socket clientSocket)
        {
            new FileLogging().WriteLogAdd($"Клиент подключен!", LoggingStatus.NOTIFY);
            while (true)
            {
                try
                {
                    //byte[] bufferReceive = new byte[clientSocket.ReceiveBufferSize];
                    byte[] bufferReceive = new byte[256];

                    int size = clientSocket.Receive(bufferReceive);

                    byte[] bufferSend = bufferReceive;

                    bufferSend[5] = (byte)128;

                    bufferSend[11] = (byte)(0);
                    bufferSend[12] = (byte)(0);

                    for (int k = 0; k < Crate.CratesList.Count; k++)
                    {
                        int count = 8 + Crate.CratesList[k].NumberCrate * 2;

                        if (count < bufferSend.Length)
                        {                           
                            bufferSend[count - 1] = (byte)(0);
                            bufferSend[count] = (byte)Crate.CratesList[k].SetModeCrate;
                        }
                    }

                    clientSocket.Send(bufferSend);
                }
                catch(Exception ex) 
                {
                    new FileLogging().WriteLogAdd($"Ошибка подключения клиента! {ex.Message}", LoggingStatus.NOTIFY);
                    clientSocket.Disconnect(true);
                    //clientSocket.Shutdown(SocketShutdown.Both);
                    //clientSocket.Close();
                    //isConnect = false;
                    return;
                }
            }
        }
    }
}
