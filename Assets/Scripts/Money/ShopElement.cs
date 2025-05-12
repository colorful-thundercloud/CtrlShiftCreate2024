using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopElement : MonoBehaviour
{
    [SerializeField] AudioClip byuSound, NEMoneySound;
    [SerializeField] TMP_Text nameField, priceField;
    CardSet cardSet;
    int price;
    public void Init(CardSet set)
    {
        cardSet = set;
        nameField.text = set.Name;
        GetComponent<Image>().sprite = set.Icon;
        priceField.text = set.Price.ToString();
        price = set.Price;
    }
    public void TryBuy()
    {
        if(Wallet.PawCoins >= price)
        {
            SoundPlayer.Play.Invoke(byuSound);
            Wallet.AddCoins.Invoke(-price);

            List<string> MySets = PlayerPrefs.HasKey("MySets") ? PlayerPrefs.GetString("MySets").Split(", ").ToList() : new();
            MySets.Add(cardSet.name);
            PlayerPrefs.SetString("MySets", string.Join(", ", MySets));
            GetComponent<Button>().interactable = false;
        }
        else
        {
            SoundPlayer.Play.Invoke(NEMoneySound);

        }
    }
}
