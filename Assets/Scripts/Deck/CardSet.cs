using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSet", menuName = "CardSet", order = 0)]
public class CardSet : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField] Sprite _icon;
    [SerializeField] int _price;
    [SerializeField] string _description;
    [SerializeField] List<BasicCard> _cards;
    public string Name => _name;
    public Sprite Icon => _icon;
    public int Price => _price;
    public string Description => _description;
    public List<BasicCard> Cards => _cards;
}