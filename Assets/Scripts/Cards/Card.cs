using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriter;
    [SerializeField] TMP_Text damage, hp;


    private BasicCard card;
    public BasicCard GetBasicCard { get { return card; } }
    bool isCasted = false;
    public bool IsCasted { get { return isCasted; } }
    Camera cam;
    Animator anim;
    Vector3 startPosition;
    public void SavePosition()=> startPosition = transform.position;
    bool inField = false;
    private void Start()
    {
        startPosition = transform.position;
        cam = Camera.main;
        anim = GetComponent<Animator>();
        // spriter.sprite = card.GetAvatar;
        // hp.text = card.HP.ToString();
    }
    public void SetCard(BasicCard newCard) => card = newCard;
    bool isCasting = false;
    public bool canDrag = true;
    private void OnMouseDown()
    {
        //CardUI.OnOpenCard(card);
        card.OnClick();
    }
    private void OnMouseUp()
    {
        isCasting = false;
        if (IsCasted) return;
        if (inField)
        {
            Debug.Log("casted");
            isCasted = true;
            Field.OnCast?.Invoke(this);
        }
        else
        {
            transform.position = startPosition;
        }
    }
    private void OnMouseDrag()
    {
        if (isCasted || !canDrag) return;
        isCasting = true;
    }
    private void Update()
    {
        if (isCasting)
        {
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            transform.position = pos;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) inField = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) inField = false;
    }
}
