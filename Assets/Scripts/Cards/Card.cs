using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private BasicCard card;
    bool isCasted = false;
    public bool IsCasted { get { return isCasted; } }
    Camera cam;
    Vector3 startPosition;
    bool inField = false;
    private void Start()
    {
        startPosition = transform.position;
        cam = Camera.main;
    }
    public void SetCard(BasicCard newCard) => card = newCard;
    bool isCasting = false;
    private void OnMouseDown()
    {
        //CardUI.OnOpenCard(card);
    }
    private void OnMouseUp()
    {
        isCasting = false;
        if (inField)
        {
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
        if (isCasted) return;
        isCasting= true;
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
        if (collision.CompareTag("field")) inField= true;
    }
}
