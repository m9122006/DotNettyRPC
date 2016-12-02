﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Transport.Channels
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ChannelException : Exception
    {
        public ChannelException()
        {
        }

        public ChannelException(string message)
            : base(message)
        {
        }

        public ChannelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ChannelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ChannelException(Exception innerException)
            : base(null, innerException)
        {
        }
    }
}