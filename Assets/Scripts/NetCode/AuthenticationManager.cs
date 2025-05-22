using System.Threading.Tasks;
using TMPro;
using UnityEngine;
public class AuthenticationManager : MonoBehaviour {
    [SerializeField] MenuController controller;
    [SerializeField] GameObject playerData;
    [SerializeField] TMP_Text loadingText;
    [SerializeField] Animator cards;
    [SerializeField] Transform lobbyMenu;
    public async void Login() => await loginAnonymously();
    private async Task loginAnonymously() {
        Loading.OnStart.Invoke("Подключение");
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
            loadingText.text = "Нет подключения";
        }
        await Task.Delay(500);
        Loading.OnStart.Invoke("");
    }
}