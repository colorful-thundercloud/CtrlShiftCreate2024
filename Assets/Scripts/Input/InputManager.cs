using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static PlayerInput Input;
    static Camera cam;
    private void Awake()
    {
        cam = Camera.main;
        Input = new();
        Input.Enable();
    }
    private void OnDisable() => Input.Disable();
    public static Vector2 WorldToScreen(Vector2 pos) =>
        cam.WorldToScreenPoint(pos);
    public static Vector2 ScreenToWorld(Vector2 pos) =>
        cam.ScreenToWorldPoint(pos);
}
