using Unity.Netcode;

public struct TurnData: INetworkSerializable
{
    public string CardName;
    public bool InHand;
    public int CardId;
    public CardAction Action;
    public int TargetId;
    public TurnData(bool inHand, int cardId, CardAction action, int targetId, string cardName)
    {
        CardName = cardName;
        InHand = inHand;
        CardId = cardId;
        Action = action;
        TargetId = targetId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CardName);
        serializer.SerializeValue(ref InHand);
        serializer.SerializeValue(ref CardId);
        serializer.SerializeValue(ref Action);
        serializer.SerializeValue(ref TargetId);
    }

    public enum CardAction
    {
        cast,
        directed,
        undirected,
        user
    }
}
