using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Wallet : MonoBehaviour
{
    [SerializeField] TMP_Text myMoney;
    public static UnityEvent<int> AddCoins = new();
    public static int PawCoins { get; private set; }
    void Start()
    {
        AddCoins.AddListener(addCoins);
        if (PlayerPrefs.HasKey("Coins"))
            addCoins(PlayerPrefs.GetInt("Coins"));
    }
    public void addCoins(int delta)
    {
        PawCoins += delta;
        PlayerPrefs.SetInt("Coins", PawCoins);
        if (myMoney != default)
            myMoney.text = PawCoins.ToString();
    }
}
