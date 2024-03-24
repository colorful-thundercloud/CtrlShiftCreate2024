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
    [SerializeField] Light2D signalLight;
    [SerializeField] TMP_Text damage, hp, title;

    [SerializeField] private BasicCard card;
    public BasicCard GetBasicCard { get { return card; } }
    public bool isCasted = false;
    Camera cam;
    Animator anim;
    Vector3 startPosition, startScale;
    public void SavePosition()=> startPosition = transform.position;
    public bool canBuff = false;
    private bool Used = true;
    public Field field = null;
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
        title.text = card.Title.ToString();

        spriter.sprite = card.GetAvatar;
        updText();
    }
    public void SetCard(BasicCard newCard) => card = newCard;
    bool isCasting = false;
    public bool canDrag = false;
    Coroutine runningFunc;
    private void OnMouseEnter()
    {
        if (Field.SelectedCard == this) return;
        twinckle(false);
        lighting.color = (gameObject.tag == "enemyCard") ? Color.red : Color.blue;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f));
    }
    private void OnMouseExit()
    {
        if(Field.SelectedCard!=this) turnOfLight();
    }
    public void turnOfLight() 
    { 
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f,false));
        twinckle(!used);
    }
    private void OnMouseDown()
    {
        //CardUI.OnOpenCard(card);
        if (isCasted)
        {
            if (gameObject.tag == "myCard" )
            {
                if (used) return;
                if (Field.SelectedCard != this) Field.SelectedCard?.turnOfLight();
                twinckle(false);
                Field.SelectedCard = this;
                lighting.color = Color.green;
                StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f));
            }
            else if (gameObject.tag == "enemyCard")
            {
                Field.SelectedCard?.attack(this);
                Field.SelectedCard?.turnOfLight();
                Field.SelectedCard = null;
            }
        }
        if (runningFunc != null ) StopCoroutine(runningFunc);
        if (canDrag) runningFunc = StartCoroutine(SmoothSizeChange(new Vector3(1, 1, 1)));
    }
    public void OnMouseUp()
    {
        isCasting = false;
        if (isCasted) return;

        if (card.Type == BasicCard.cardType.Unit && field?.GetCards(gameObject.CompareTag("enemyCard")).Count < 3)
        {
            isCasted = true;
            if (gameObject.tag != "enemyCard") Field.OnCast?.Invoke(this);
            foreach (Transform t in transform) t.gameObject.SetActive(true);
            transform.localScale = Vector3.one;
        }
        else if (canBuff)
        {
            canBuff = false;
            canDrag = false;
            otherCard.StatsChange(currentAtk, currentHP);
            Field.OnCardBeat?.Invoke(this);
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
        if (isCasted && !canDrag && gameObject.CompareTag("enemyCard")) isCasting = false;
        else if (!isCasted && canDrag) isCasting = true;
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
        if (collision.CompareTag("field")) field = collision.GetComponent<Field>();
        if (collision.CompareTag(gameObject.tag) && card.Type == BasicCard.cardType.Buff)
        {
            otherCard = collision.gameObject.GetComponent<Card>();
            if (otherCard.GetBasicCard.Type != BasicCard.cardType.Buff && otherCard.field != null)
                canBuff = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) field = null;
        if (collision.CompareTag(gameObject.tag) && card.Type == BasicCard.cardType.Buff)
        {
            otherCard = collision.gameObject.GetComponent<Card>();
            if (otherCard.GetBasicCard.Type != BasicCard.cardType.Buff && otherCard.field != null)
                canBuff = false;
            otherCard = null;
        }
    }
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(gameObject.tag) && card.Type == BasicCard.cardType.Buff)
        {
            otherCard = collision.gameObject.GetComponent<Card>();
            if (otherCard.GetBasicCard.Type != BasicCard.cardType.Buff && otherCard.field != null)
                canBuff = true;
        }
    }

    IEnumerator SmoothSizeChange(Vector3 targetScale, bool grow = false)
    {
        if (canDrag)
        {
            if (grow && transform.localScale.x < targetScale.x) transform.localScale += new Vector3(0.05f, 0.05f, 0);
            else if (!grow && transform.localScale.x > targetScale.x) transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            else yield break;

            yield return new WaitForSeconds(0.005f);
            runningFunc = StartCoroutine(SmoothSizeChange(targetScale, grow));
        }
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
        this.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Field.OnCardBeat(this);
    }
    void updText()
    {
        hp.text = currentHP.ToString();
        damage.text = currentAtk.ToString();
    }
    public void attack(Card toAttack)
    {
        used = true;
        StartCoroutine(attackAnimation(0.5f, toAttack));
    }
    IEnumerator attackAnimation(float smoothTime, Card toAttack)
    {
        startPosition = transform.position;
        Vector3 target = toAttack.gameObject.transform.position;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, smoothTime);
            yield return new WaitForFixedUpdate();
        }
        toAttack.StatsChange(0, -currentAtk);
        target = startPosition;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, smoothTime);
            yield return new WaitForFixedUpdate();
        }
    }
    public bool used
    {
        get { return Used; }
        set
        {
            Used = value;
            twinckle(!used);
        }
    }
    Coroutine twink;
    private void twinckle(bool isEnabled)
    {
        if (gameObject.tag == "enemyCard") return;
        if (isEnabled) twink = StartCoroutine(SmoothLight.twinckle(signalLight, 0.75f));
        else if(twink!=null)
        {
            StopCoroutine(twink);
            signalLight.falloffIntensity = 1f;
        }
    }
}
