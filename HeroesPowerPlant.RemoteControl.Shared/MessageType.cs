using System;

namespace HeroesPowerPlant.RemoteControl.Shared
{
    public enum MessageType : byte
    {
        SwapCollision,

        // Exceptions (Responses) | 200 - 255 
        Acknowledgement = 200
    }
}
