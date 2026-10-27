using MessagePack.Resolvers;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Interfaces.Utilities;
using Reloaded.Messaging.Serializer.MessagePack;

namespace HeroesPowerPlant.RemoteControl.Shared.Messages
{
    public struct Acknowledgement : IMessage<Acknowledgement, MessagePackSerializer<Acknowledgement>, NullCompressor>
    {
        public sbyte GetMessageType() => (sbyte)MessageType.Acknowledgement;
        public MessagePackSerializer<Acknowledgement> GetSerializer() => new MessagePackSerializer<Acknowledgement>(ContractlessStandardResolver.Instance);
        public NullCompressor GetCompressor() => null;

        /// <summary>
        /// This member is a dummy, it has no meaning.
        /// </summary>
        public byte Null { get; set; }
    }
}
