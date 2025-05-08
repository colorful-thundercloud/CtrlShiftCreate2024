using System.Collections.Generic;

public class Constants {
    public const string JoinKey = "j";
    public const string PasswordKey = "p";
    public const string TimerKey = "t";
    public const string FirstTurnKey = "f";
    public const int MaxCardsInHand = 5;
    public const int MaxCardsInField = 7;
    public enum FirstTurn
    {
        Random,
        Host,
        Client
    }
}
