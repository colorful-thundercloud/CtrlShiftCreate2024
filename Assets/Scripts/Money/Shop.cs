using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject elementPrefab;
    [SerializeField] Transform content;
    private void OnEnable()
    {
        foreach (Transform element in content) Destroy(element.gameObject);

        List<CardSet> sets = Resources.LoadAll<CardSet>("Sets").ToList();
        sets.Sort((x, y) => x.Price.CompareTo(y.Price));
        sets.Remove(sets.First());
        List<string> MySets = PlayerPrefs.HasKey("MySets") ? PlayerPrefs.GetString("MySets").Split(", ").ToList() : new();

        foreach (CardSet set in sets)
        {
            ShopElement element = Instantiate(elementPrefab,content).GetComponent<ShopElement>();
            element.Init(set);
            if (MySets.Contains(set.name)) 
                element.GetComponent<Button>().interactable = false;
        }
    }
}
