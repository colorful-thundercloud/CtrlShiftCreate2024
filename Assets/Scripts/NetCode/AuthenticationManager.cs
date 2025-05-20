using UnityEngine;
public class AuthenticationManager : MonoBehaviour {
    [SerializeField] MenuController controller;
    [SerializeField] Transform lobbyMenu;
    public async void LoginAnonymously() {
        await Authentication.Login();
        controller.ToggleWindow(lobbyMenu);
    }
}