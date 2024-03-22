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
    Vector3 startPosition, startScale;
    public void SavePosition()=> startPosition = transform.position;
    bool inField = false;
    private void Start()
    {
        startPosition = transform.position;
        cam = Camera.main;
        anim = GetComponent<Animator>();
        startScale = transform.localScale;
        // spriter.sprite = card.GetAvatar;
        // hp.text = card.HP.ToString();
    }
    public void SetCard(BasicCard newCard) => card = newCard;
    bool isCasting = false;
    public bool canDrag = true;
    Coroutine runningFunc;
    public void EnemyCast()
    {
        inField = true;
    }
    private void OnMouseDown()
    {
        //CardUI.OnOpenCard(card);
        card.OnClick();
        runningFunc = StartCoroutine(SmoothSizeChange(new Vector3(1, 1, 1)));
    }
    public void OnMouseUp()
    {
        isCasting = false;
        if (IsCasted) return;
        if (inField)
        {
            Debug.Log("casted");
            isCasted = true;
            Field.OnCast?.Invoke(this);
            foreach (Transform t in transform) t.gameObject.SetActive(true);
        }
        else
        {
            runningFunc = StartCoroutine(SmoothSizeChange(startScale, true));
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
            pos.z = transform.position.z;
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

    IEnumerator SmoothSizeChange(Vector3 targetScale, bool grow = false)
    {
        if (grow && transform.localScale.x < targetScale.x) transform.localScale += new Vector3(0.05f, 0.05f, 0);
        else if (!grow && transform.localScale.x > targetScale.x) transform.localScale -= new Vector3(0.05f, 0.05f, 0);
        else yield break;
        
        yield return new WaitForSeconds(0.005f);
        yield return StartCoroutine(SmoothSizeChange(targetScale, grow));
    }
}
