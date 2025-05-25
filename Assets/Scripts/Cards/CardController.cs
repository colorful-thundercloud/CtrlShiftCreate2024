using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController: MonoBehaviour
{
    [Header("Поля для данных карт")]
    [SerializeField] public TMP_Text Title;
    [SerializeField] SpriteRenderer Icon, Outliner;
    [SerializeField] Vector3 DragCardSize = new Vector3(1.5f, 1.5f, 1);
    [SerializeField] public AudioClip CastSound, SelectSound;

    [SerializeField] private BasicCard basicCard;
    public BasicCard GetBasicCard { get { return basicCard; } }
    [SerializeField] CardStats stats;
    public CardStats GetStats => stats;
    public Stat GetStat(string name) => stats.GetStat(name);
    
    public static CardController otherCard { get; private set; }
    private static CardController selected;
    public int cardID { get; private set; }
    public static CardController Selected 
    { 
        set 
        {
            if (value != default)
            {
                if (!value.GetBasicCard.CheckAction(value)) return;
                selected?.outline(false, Color.white);
                selected?.hover(false);

                SoundPlayer.Play.Invoke(value.SelectSound);
                value.outline(true, Color.white, false);
            }
            else if(selected != default)
            {
                selected?.outline(false, Color.white);
                selected?.hover(false);
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
        startScale = transform.localScale;
        startPosition = transform.position;
    }
    public void SetCard(BasicCard newCard, int id)
    {
        cardID = id;
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

        Title.text = basicCard.Title.ToString();
        Icon.sprite = basicCard.GetAvatar;
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
            if (selected != default)
            {
                if (GetBasicCard.OnClick(this))
                {
                    GameManager.UpdateTurns.Invoke(new TurnData(false, Selected.cardID, TurnData.CardAction.directed, cardID,Selected.GetBasicCard.Title));
                    Selected = default;
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
            }
        }
        else if (GameManager.myTurn) runningFunc = StartCoroutine(Mover.SmoothSizeChange(DragCardSize, transform, 0.1f));
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
        transform.Find("Shirt").gameObject.SetActive(!enabled);
        transform.Find("Name").gameObject.SetActive(enabled);
        transform.Find("Icon").gameObject.SetActive(enabled);
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
            Vector3 pos = InputManager.ScreenToWorld(Input.mousePosition);
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) field = collision.GetComponent<GameManager>();
        if (collision.TryGetComponent(out CardController card)) 
            if (card.isCasted && basicCard.GetAction().CheckAlies(this, card)) otherCard = card;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) field = null;
        if (collision.TryGetComponent(out CardController card)) if (card==otherCard) otherCard = null;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("field")) field = collision.GetComponent<GameManager>();
        if (collision.TryGetComponent(out CardController card)) 
            if (card.isCasted && basicCard.GetAction().CheckAlies(this, card)) otherCard = card;
    }
    void outline(bool isEnabled, Color color, bool twink = false)
    {
        Outliner.enabled = isEnabled;
        Outliner.material.SetColor("_Color", color);
        Outliner.material.SetInt("_Twinkle", twink ? 1 : 0);
    }
}
