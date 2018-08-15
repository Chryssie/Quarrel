﻿using DiscordPipeImpersonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace NamedPipeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting NamedPipeServer");
            Start();
            Console.ReadKey();
        }
        static QuarrelAppService service = new QuarrelAppService();
        private static async void Start()
        {
            await service.TryConnectAsync(true);
            DiscordPipeServer server = new DiscordPipeServer();
            server.MessageReceived += Server_MessageReceived;
            server.ConnectionUpdate += Server_ConnectionUpdate;
            server.Start();
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Console.WriteLine("App service closed");
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Console.WriteLine("App service request received");
        }

        private static async void Server_ConnectionUpdate(object sender, DiscordPipeServer.ConnectionState e)
        {
        }

        private static async void Server_MessageReceived(object sender, string e)
        {
            service.SetActivity(e);
            throw new NotImplementedException();
        }
    }
}