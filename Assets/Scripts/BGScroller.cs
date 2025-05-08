using UnityEngine;

public class BGScroller : MonoBehaviour
{
    [SerializeField] Vector2 ScrollSpeed;
    Vector2 offset = Vector2.zero;
    Material mat;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset += ScrollSpeed * Time.deltaTime / 10f;
        mat.SetTextureOffset("_BaseMap", offset);
    }
}
