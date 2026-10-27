namespace HeroesPowerPlant.RemoteControl.Shared
{
    public enum MessageType : sbyte
    {
        SwapCollision,

        // Exceptions (Responses) | 100 - 127
        Acknowledgement = 100
    }
}
