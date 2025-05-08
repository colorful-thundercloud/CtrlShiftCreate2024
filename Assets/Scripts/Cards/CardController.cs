using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class CardController: MonoBehaviour
{
    [Header("Поля для данных карт")]
    [SerializeField] public TMP_Text title;

    [SerializeField] SpriteRenderer image;
    [SerializeField] SpriteRenderer outliner;
    [SerializeField] Vector3 CardSize = new Vector3(1.5f, 1.5f, 1);

    [SerializeField] public AudioClip CastSound, SelectSound;

    [SerializeField] private BasicCard basicCard;
    public BasicCard GetBasicCard { get { return basicCard; } }
    [SerializeField] CardStats stats;
    public CardStats GetStats => stats;
    public Stat GetStat(string name) => stats.GetStat(name);
    Camera cam;
    
    public static CardController otherCard { get; private set; }
    private static CardController selected;
    public int cardID;
    public static CardController Selected 
    { 
        set 
        { 
            if (value == null)
            {
                selected?.outline(false, Color.white);
                selected?.hover(false);
            }
            else
            {
                if (!value.GetBasicCard.CheckAction(value)) return;
                selected?.outline(false, Color.white);
                selected?.hover(false);

                SoundPlayer.Play.Invoke(value.SelectSound);
                value.outline(true, Color.white, false);
            }
            selected = value;
        }
        get { return selected; }
    }
    public bool isCasted = false, isCasting = false;
    GameManager field = null;
    Coroutine runningFunc;
    Vector3 startPosition, startScale;
    public void SavePosition()=> startPosition = transform.position;
    public void SaveScale()=> startScale = transform.localScale;
    private void Start()
    {
        cam = Camera.main;
        startScale = transform.localScale;
        startPosition = transform.position;
    }
    public void SetCard(BasicCard newCard)
    {
        basicCard = newCard;
        basicCard.initialize(this);
        stats = new(basicCard.GetBasicStats(this));
        if (CompareTag("myCard")) Show(true);


        GameManager.OnEndTurn.AddListener(myTurn =>
        {
            if (CompareTag("enemyCard")) return;
            if (!isCasted) return;
            if (myTurn) outline(GetBasicCard.CheckAction(this), Color.yellow, true);
            else outline(false, Color.white);
        });

        title.text = basicCard.Title.ToString();
        image.sprite = basicCard.GetAvatar;
    }
    Coroutine hoverCoroutine;
    void hover(bool enabled)
    {
        if (isCasting) return;
        outline(false, Color.white);
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(Mover.SmoothSizeChange((enabled) ? transform.localScale * 1.1f : startScale, transform, 0.1f));
    }
    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Selected != this || Selected == null) hover(true);
    }
    private void OnMouseExit()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Selected != this)
        {
            hover(false);
            if (isCasted && GameManager.myTurn) outline(basicCard.CheckAction(this), Color.yellow, true);
        }
    }
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (runningFunc != null) StopCoroutine(runningFunc);

        // все карты
        if (isCasted)
        {
            CardUI.OnOpenCard.Invoke(this);
            if (selected != null)
            {
                if (GetBasicCard.OnClick(this))
                {
                    GameManager.UpdateTurns.Invoke(new TurnData(false, Selected.cardID, TurnData.CardAction.directed, cardID,Selected.GetBasicCard.Title));
                    Selected = null;
                    return;
                }
            }
        }

        if (gameObject.CompareTag("enemyCard")) return;
        // только карты игрока

        CardUI.OnOpenCard.Invoke(this);

        if (isCasted)
        {
            if (selected != this && GameManager.myTurn)
            {
                Selected = this;
                GetBasicCard.OnSelect(this);

                GameManager.UpdateTurns.Invoke(new TurnData(false, cardID, TurnData.CardAction.undirected, 0,basicCard.Title));
            }
        }
        else if (GameManager.myTurn) runningFunc = StartCoroutine(Mover.SmoothSizeChange(CardSize, transform, 0.1f));
    }
    public void cast()
    {
        isCasting = false;
        isCasted = true;
        SoundPlayer.Play.Invoke(CastSound);
        if (!gameObject.CompareTag("enemyCard"))
            GameManager.UpdateTurns.Invoke(new TurnData(true, cardID, TurnData.CardAction.cast, otherCard != null ? otherCard.cardID : 0, basicCard.Title));
        GameManager.OnCast?.Invoke(this);
    }
    public void Show(bool enabled)
    {
        SoundPlayer.Play.Invoke(SelectSound);
        transform.Find("shirt").gameObject.SetActive(!enabled);
        transform.Find("name").gameObject.SetActive(enabled);
        transform.Find("image").gameObject.SetActive(enabled);
        stats.ShowAll(enabled);
    }
    void backToHand()
    {
        isCasting = false;
        if (runningFunc != null) StopCoroutine(runningFunc);
        runningFunc = StartCoroutine(Mover.SmoothSizeChange(startScale,transform, 0.1f));
        transform.position = startPosition;
    }
    public void OnMouseUp()
    {
        if (CompareTag("enemyCard")) return;
        if (isCasted) return;
        if(field==null||(!field.CheckCount()&&!GetBasicCard.isIngoringFieldCapacity)) backToHand();
        else
        {
            if (basicCard.cast(this))
            {
                cast();
            }
            else backToHand();
        }
    }
    private void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (CompareTag("enemyCard")) return;
        if (!isCasted && GameManager.myTurn)
        {
            if (!isCasting)
            {
                isCasting = true;
                SoundPlayer.Play.Invoke(SelectSound);
            }
        }
    }
    private void FixedUpdate()
    {
        if (isCasting && CompareTag("myCard"))
        {
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            transform.position = pos;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) field = collision.GetComponent<GameManager>();
        if (collision.TryGetComponent<CardController>(out CardController card)) if(card.isCasted) otherCard = card;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) field = null;
        if (collision.TryGetComponent<CardController>(out CardController card)) if (card==otherCard) otherCard = null;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("field")) field = collision.GetComponent<GameManager>();
        if (collision.TryGetComponent<CardController>(out CardController card)) if (card.isCasted) otherCard = card;
    }
    void outline(bool isEnabled, Color color, bool twink = false)
    {
        outliner.enabled = isEnabled;
        outliner.material.SetColor("_Color", color);
        outliner.material.SetInt("_Twinkle", twink ? 1 : 0);
    }
}
