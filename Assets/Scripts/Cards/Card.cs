using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Card: MonoBehaviour
{
    [SerializeField] public TMP_Text damage;
    [SerializeField] public TMP_Text health;
    [SerializeField] public TMP_Text title;

    [SerializeField] SpriteRenderer spriter;
    [SerializeField] public Light2D lighting;
    [SerializeField] Light2D signalLight;

    [SerializeField] public AudioClip CastSound, SelectSound;

    [SerializeField] private BasicCard basicCard;
    public BasicCard GetBasicCard { get { return basicCard; } }
    Camera cam;
    
    public static Card otherCard { get; private set; }
    private static Action selected;
    public static Action Selected 
    { 
        set 
        { 
            if (value == null)
            {
                selected?.card.turnOfLight();
            }
            else
            {
                if (!value.card.basicCard.CheckAction()) return;
                if(selected!=value) selected?.card.turnOfLight();

                SoundPlayer.Play(value.card.SelectSound);
                value.card.lighting.color = Color.green;
                value.card.StartCoroutine(SmoothLight.smoothLight(value.card.lighting, 0.25f));
                value.card.twinckle(false);
            }
            selected = value;
        }
        get { return selected; }
    }

    bool isCasting = false;
    public bool isCasted = false;
    Field field = null;
    bool myTurn = true;
    Coroutine runningFunc;
    Vector3 startPosition, startScale;
    public void SavePosition()=> startPosition = transform.position;
    private void Start()
    {
        cam = Camera.main;
        startScale = transform.localScale;
        startPosition = transform.position;
        basicCard.initialize(this);
        TurnBasedGameplay.OnEndTurn.AddListener(isEnemyTurn =>
        {
            myTurn = !isEnemyTurn;
            if (myTurn) twinckle(basicCard.CheckAction());
            else twinckle(false);
        });
    }
    public void SetCard(BasicCard newCard)
    {
        basicCard = newCard;
        title.text = basicCard.Title.ToString();
        spriter.sprite = basicCard.GetAvatar;
    }
    private void hover()
    {
        twinckle(false);
        lighting.color = (gameObject.tag == "enemyCard") ? Color.red : Color.blue;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f));
    }
    private void OnMouseEnter()
    {
        if (Selected == null || Selected.card != this) hover();
    }
    private void OnMouseExit()
    {
        if (Selected == null || Selected.card != this)
        {
            turnOfLight();
            if (isCasted && myTurn) twinckle(basicCard.CheckAction());
        }
    }
    public void turnOfLight() 
    {
        if (lighting == null) return;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f,false));
        if (selected?.card == this) twinckle(selected.CheckAviability());
    }
    private void OnMouseDown()
    {
        if (runningFunc != null) StopCoroutine(runningFunc);
        if (myTurn) runningFunc = StartCoroutine(SmoothSizeChange(new Vector3(1, 1, 1)));

        // все карты
        if (isCasted)
        {
            CardUI.OnOpenCard(basicCard);
            if (selected != null) if (selected.card.basicCard.OnClick()) return;
        }

        if (gameObject.CompareTag("enemyCard")) return;
        // только карты игрока

        CardUI.OnOpenCard(basicCard);

        if (isCasted)
        {
            if (selected?.card != this && myTurn)//////////условие говно
            {
                basicCard.OnSelect();
            }
        }
    }
    void cast()
    {
        isCasting = false;
        isCasted = true;
        SoundPlayer.Play(CastSound);
        Field.OnCast?.Invoke(this);
        transform.localScale = Vector3.one;
        //foreach (Transform t in transform) t.gameObject.SetActive(true);
    }
    void backToHand()
    {
        isCasting = false;
        if (runningFunc != null) StopCoroutine(runningFunc);
        runningFunc = StartCoroutine(SmoothSizeChange(startScale, true));
        transform.position = startPosition;
    }
    public void OnMouseUp()
    {
        if (isCasted) return;
        if(field==null||!field.CheckCount()) backToHand();
        else
        {
            if(basicCard.cast())cast();
            else backToHand();
        }
    }
    private void OnMouseDrag() //настроить доступность по экшену
    {
        //if (isCasted && !myTurn && gameObject.CompareTag("enemyCard")) isCasting = false;
        if (!isCasted && myTurn)
        {
            if (!isCasting)
            {
                isCasting = true;
                SoundPlayer.Play(SelectSound);
            }
        }
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
        if (collision.TryGetComponent<Card>(out Card card)) if(card.isCasted) otherCard = card;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) field = null;
        if (collision.TryGetComponent<Card>(out Card card)) if (card==otherCard) otherCard = null;
    }
    private void OnTriggerStay2D(Collider2D collision) {
        /*if (collision.CompareTag(gameObject.tag) && card.Type == BasicCard.cardType.Buff)
        {
            otherCard = collision.gameObject.GetComponent<Card>();
            if (otherCard.GetBasicCard.Type != BasicCard.cardType.Buff && otherCard.field != null)
                canBuff = true;
        }*/
    }

    IEnumerator SmoothSizeChange(Vector3 targetScale, bool grow = false)
    {
        if (myTurn)
        {
            if (grow && transform.localScale.x < targetScale.x) transform.localScale += new Vector3(0.05f, 0.05f, 0);
            else if (!grow && transform.localScale.x > targetScale.x) transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            else yield break;

            yield return new WaitForSeconds(0.005f);
            runningFunc = StartCoroutine(SmoothSizeChange(targetScale, grow));
        }
    }
    Coroutine twink;
    public void twinckle(bool isEnabled)
    {
        if (this == null) return;
        if (gameObject.tag == "enemyCard") return;
        if (isEnabled)
        {
            if(twink!=null) StopCoroutine(twink);
            twink = StartCoroutine(SmoothLight.twinckle(signalLight, 0.75f));
        }
        else if(twink!=null)
        {
            StopCoroutine(twink);
            signalLight.falloffIntensity = 1f;
        }
    }
}
