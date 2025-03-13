using UnityEngine;
public class AuthenticationManager : MonoBehaviour {
    [SerializeField] mainMenuController controller;
    [SerializeField] Transform lobbyMenu;
    public async void LoginAnonymously() {
        await Authentication.Login();
        controller.ToggleWindow(lobbyMenu);
    }
}