using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Serializer.MessagePack;

namespace HeroesPowerPlant.RemoteControl.Shared.Messages
{
    public struct SwapCollision : IMessage<MessageType>
    {
        public MessageType GetMessageType() => MessageType.SwapCollision;
        public ISerializer GetSerializer()  => new MsgPackSerializer(true);
        public ICompressor GetCompressor()  => null;

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
