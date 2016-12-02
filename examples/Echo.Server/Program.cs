﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Echo.Server
{
    using System;
    using System.Diagnostics.Tracing;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using DotNetty.Codecs;
    using DotNetty.Common.Internal.Logging;
    using DotNetty.Handlers.Logging;
    using DotNetty.Handlers.Tls;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;


    class Program
    {
        static async Task RunServer()
        {
          
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100)
                    .Handler(new LoggingHandler(LogLevel.INFO))
                    .ChildHandler(new ActionSendChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        if (EchoServerSettings.IsSsl)
                        {
                            pipeline.AddLast(TlsHandler.Server(new X509Certificate2("dotnetty.com.pfx", "password")));
                        }
                        pipeline.AddLast(new LengthFieldPrepender(2));
                        pipeline.AddLast(new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                        pipeline.AddLast(new EchoServerHandler());
                    }));

                IChannel bootstrapChannel = await bootstrap.BindAsync(EchoServerSettings.Port);

                Console.ReadLine();

                await bootstrapChannel.CloseAsync();
            }
            finally
            {
                Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
              
            }
        }

        static void Main(string[] args)
        {
            Task.Run(() => RunServer()).Wait();
        }
    }
}