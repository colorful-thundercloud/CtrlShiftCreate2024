using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    [Header("Hover")]
    [SerializeField] Vector2 HoverPos;
    [SerializeField] float HoverSpeed;

    [SerializeField] bool Interactable = true;
    [SerializeField] AudioClip ClickSound;
    [SerializeField] UnityEvent OnClick = new();
    Coroutine hover;
    Vector3 startPos;
    private void Start()
    {
        startPos= transform.position;
    }
    private void OnEnable()
    {
        GetComponentInParent<Animator>().enabled = false;
    }
    private void OnDisable()
    {
        this.enabled = false;
    }
    private void OnMouseEnter()
    {
        if (!Interactable || !this.enabled) return;
        if(hover!=null) StopCoroutine(hover);
        hover = StartCoroutine(Mover.MoveCard(transform, new Vector3(HoverPos.x, HoverPos.y, transform.position.z), HoverSpeed));
    }
    private void OnMouseExit()
    {
        if (!Interactable || !this.enabled) return;
        if (hover != null) StopCoroutine(hover);
        hover = StartCoroutine(Mover.MoveCard(transform, startPos, HoverSpeed));
    }
    private void OnMouseDown()
    {
        if (!Interactable) return;
        SoundPlayer.Play.Invoke(ClickSound);
    }
    private void OnMouseUp()
    {
        if (!Interactable) return;
        StopAllCoroutines();
        OnClick.Invoke();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, HoverPos);
    }
}
