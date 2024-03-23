using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriter;
    [SerializeField] Light2D lighting;
    [SerializeField] TMP_Text damage, hp, name;

    [SerializeField] private BasicCard card;
    public BasicCard GetBasicCard { get { return card; } }
    bool isCasted = false;
    public bool IsCasted { get { return isCasted; } }
    Camera cam;
    Animator anim;
    Vector3 startPosition, startScale;
    public void SavePosition()=> startPosition = transform.position;
    public bool inField = false, canBuff = false;
    int currentHP, currentAtk;
    public int HP { get { return currentHP; } }
    public int Damage { get { return currentAtk; } }
    Card otherCard;
    private void Start()
    {
        startPosition = transform.position;
        cam = Camera.main;
        anim = GetComponent<Animator>();
        startScale = transform.localScale;

        currentHP = card.HP;
        currentAtk = card.Damage;
        name.text = card.Title.ToString();

        spriter.sprite = card.GetAvatar;
        updText();
    }
    public void SetCard(BasicCard newCard) => card = newCard;
    bool isCasting = false;
    public bool canDrag = true;
    Coroutine runningFunc;
    public void EnemyCast()
    {
        inField = true;
    }
    private void OnMouseEnter()
    {
        if (Field.SelectedCard == this) return;
            lighting.color = (gameObject.tag == "enemyCard") ? Color.red : Color.blue;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.5f));
    }
    private void OnMouseExit()
    {
        if(Field.SelectedCard!=this) StartCoroutine(SmoothLight.smoothLight(lighting, 0.5f,false));
    }
    public void turnOfLight() { StartCoroutine(SmoothLight.smoothLight(lighting, 0.5f,false)); }
    private void OnMouseDown()
    {
        //CardUI.OnOpenCard(card);
        if (IsCasted)
        {
            if (gameObject.tag == "myCard")
            {
                Field.SelectedCard = this;
                lighting.color = Color.green;
                StartCoroutine(SmoothLight.smoothLight(lighting, 0.5f));
            }
            else if (gameObject.tag == "enemyCard")
            {
                Field.SelectedCard?.attack(this);
                Field.SelectedCard?.turnOfLight();
                Field.SelectedCard = null;
            }
        }
        if (runningFunc != null) StopCoroutine(runningFunc);
        runningFunc = StartCoroutine(SmoothSizeChange(new Vector3(1, 1, 1)));
    }
    public void OnMouseUp()
    {
        isCasting = false;
        if (IsCasted) return;
        if (inField)
        {
            if (card.Type == BasicCard.cardType.Unit)
            {
                isCasted = true;
                if(gameObject.tag !="enemyCard") Field.OnCast?.Invoke(this);
                foreach (Transform t in transform) t.gameObject.SetActive(true);
                transform.localScale = Vector3.one;
            }
            else if (canBuff)
            {
                canBuff = false;
                canDrag = false;
                otherCard.StatsChange(currentAtk, currentHP);
                Field.OnBuff?.Invoke(this);
            }
            else
            {
                if (runningFunc != null) StopCoroutine(runningFunc);
                runningFunc = StartCoroutine(SmoothSizeChange(startScale, true));
                transform.position = startPosition;
            }
        }
        else
        {
            if (runningFunc != null) StopCoroutine(runningFunc);
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
        if (collision.CompareTag(gameObject.tag) && card.Type == BasicCard.cardType.Buff)
        {
            otherCard = collision.gameObject.GetComponent<Card>();
            if (otherCard.GetBasicCard.Type != BasicCard.cardType.Buff && otherCard.inField)
            {
                otherCard.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                canBuff = true;
            }
                
            else otherCard = null;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) inField = false;
        if (collision.CompareTag(gameObject.tag) && card.Type == BasicCard.cardType.Buff)
        {
            otherCard = collision.gameObject.GetComponent<Card>();
            if (otherCard.GetBasicCard.Type != BasicCard.cardType.Buff && otherCard.inField)
                {
                    otherCard.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    canBuff = false;
                }
            otherCard = null;
        }
    }
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(gameObject.tag) && card.Type == BasicCard.cardType.Buff)
        {
            otherCard = collision.gameObject.GetComponent<Card>();
            if (otherCard.GetBasicCard.Type != BasicCard.cardType.Buff && otherCard.inField)
            {
                otherCard.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                canBuff = true;
            }
            else otherCard = null;
        }
    }

    IEnumerator SmoothSizeChange(Vector3 targetScale, bool grow = false)
    {
        if (grow && transform.localScale.x < targetScale.x) transform.localScale += new Vector3(0.05f, 0.05f, 0);
        else if (!grow && transform.localScale.x > targetScale.x) transform.localScale -= new Vector3(0.05f, 0.05f, 0);
        else yield break;
        
        yield return new WaitForSeconds(0.005f);
        yield return runningFunc = StartCoroutine(SmoothSizeChange(targetScale, grow));
    }
    public void StatsChange(int atk = 0, int health = 0)
    {
        currentAtk += atk;
        if (currentAtk < 0) currentAtk = 0;
        currentHP += health;
        if (currentHP <= 0) StartCoroutine( death());
        updText();
    }
    IEnumerator death() 
    {
        //play animation
        this.enabled= false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    void updText()
    {
        hp.text = currentHP.ToString();
        damage.text = currentAtk.ToString();
    }
    public void attack(Card toAttack)
    {
        toAttack.StatsChange(0, -currentAtk);
    }
}
