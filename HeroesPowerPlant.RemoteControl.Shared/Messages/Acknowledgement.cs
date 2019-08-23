using System;
using System.Collections.Generic;
using System.Text;
using Reloaded.Messaging.Compression;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Serialization;
using Reloaded.Messaging.Serializer.MessagePack;

namespace HeroesPowerPlant.RemoteControl.Shared.Messages
{
    public struct Acknowledgement : IMessage<MessageType>
    {
        public MessageType GetMessageType() => MessageType.Acknowledgement;
        public ISerializer GetSerializer() => new MsgPackSerializer(true);
        public ICompressor GetCompressor() => null;

        /// <summary>
        /// This member is a dummy, it has no meaning.
        /// </summary>
        public byte Null { get; set; }
    }
}
