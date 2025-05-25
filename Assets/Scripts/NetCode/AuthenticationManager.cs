using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class AuthenticationManager : MonoBehaviour {
    [SerializeField] MenuController controller;
    [SerializeField] GameObject playerData;
    [SerializeField] Animator cards;
    [SerializeField] Transform lobbyMenu;
    public async void Login() => await loginAnonymously();
    private async Task loginAnonymously() {
        try
        {
            await Authentication.Login();
            controller.ToggleWindow(lobbyMenu);
            controller.MoveCamera(-4.36f);
            cards.SetTrigger("ClosePlay");
            playerData.SetActive(false);
        }
        catch
        {
            string notConection = LocalizationSettings.StringDatabase.GetTableEntry("LocaleTable", "NotConection").Entry.Value;
            Loading.OnStart.Invoke(notConection);
        }
        await Task.Delay(500);
        Loading.OnStart.Invoke("");
    }
}