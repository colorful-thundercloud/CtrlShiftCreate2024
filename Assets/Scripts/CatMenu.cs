using UnityEngine;

public class CatMenu : MonoBehaviour
{
    [SerializeField] Transform Eyes;
    private Vector2 eyesStart;
    float R = 0.2f;
    Camera cam;
    void Awake()
    {
        cam = Camera.main;
        eyesStart = Eyes.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Eyes.localPosition = eyesStart;
        Vector2 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = Vector2.ClampMagnitude(mouse - (Vector2)Eyes.position, R);
        Eyes.position = (Vector2)Eyes.position + direction;
    }
}
