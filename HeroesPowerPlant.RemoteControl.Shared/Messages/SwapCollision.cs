using MessagePack.Resolvers;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Interfaces.Utilities;
using Reloaded.Messaging.Serializer.MessagePack;

namespace HeroesPowerPlant.RemoteControl.Shared.Messages
{
    public struct SwapCollision : IMessage<SwapCollision, MessagePackSerializer<SwapCollision>, NullCompressor>
    {
        public sbyte GetMessageType() => (sbyte)MessageType.SwapCollision;
        public MessagePackSerializer<SwapCollision> GetSerializer() => new MessagePackSerializer<SwapCollision>(ContractlessStandardResolver.Instance);
        public NullCompressor GetCompressor() => null;

        /// <summary>
        /// Name of the file in the collisions folder minus the name of the extension.
        /// </summary>
        public string CollisionFileName { get; set; }

        public SwapCollision(string collisionFileName)
        {
            CollisionFileName = collisionFileName;
        }
    }
}
