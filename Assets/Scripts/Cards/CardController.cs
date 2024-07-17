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
    [SerializeField] public Light2D lighting;
    [SerializeField] Light2D signalLight;
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
    public static CardController Selected 
    { 
        set 
        { 
            if (value == null)
            {
                selected?.turnOfLight();
            }
            else
            {
                if (!value.GetBasicCard.CheckAction(value)) return;
                selected?.turnOfLight();

                SoundPlayer.Play.Invoke(value.SelectSound);
                value.lighting.color = Color.green;
                value.StartCoroutine(SmoothLight.smoothLight(value.lighting, 0.25f));
                value.twinckle(false);
            }
            selected = value;
        }
        get { return selected; }
    }

    bool isCasting = false;
    public bool isCasted = false;
    Field field = null;
    Coroutine runningFunc;
    Vector3 startPosition, startScale;
    public void SavePosition()=> startPosition = transform.position;
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


        TurnBasedGameplay.OnEndTurn.AddListener(isEnemyTurn =>
        {
            if (CompareTag("enemyCard")) return;
            if (!isEnemyTurn) twinckle(GetBasicCard.CheckAction(this));
            else twinckle(false);
        });

        title.text = basicCard.Title.ToString();
        image.sprite = basicCard.GetAvatar;
    }
    private void hover()
    {
        twinckle(false);
        lighting.color = (gameObject.tag == "enemyCard") ? Color.red : Color.blue;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f));
    }
    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Selected != this || Selected == null) hover();
    }
    private void OnMouseExit()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Selected != this)
        {
            turnOfLight();
            if (isCasted && TurnBasedGameplay.myTurn) twinckle(basicCard.CheckAction(this));
        }
    }
    public void turnOfLight() 
    {
        if (lighting == null) return;
        StartCoroutine(SmoothLight.smoothLight(lighting, 0.25f,false));
    }
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (runningFunc != null) StopCoroutine(runningFunc);

        // все карты
        if (isCasted)
        {
            CardUI.OnOpenCard.Invoke(this);
            if (selected != null) if (GetBasicCard.OnClick(this)) return;
        }

        if (gameObject.CompareTag("enemyCard")) return;
        // только карты игрока

        if (TurnBasedGameplay.myTurn) runningFunc = StartCoroutine(Mover.SmoothSizeChange(CardSize, transform,0.1f));
        CardUI.OnOpenCard.Invoke(this);

        if (isCasted)
        {
            if (selected != this && TurnBasedGameplay.myTurn)
            {
                Selected = this;
                GetBasicCard.OnSelect();
            }
        }
    }
    public void cast()
    {
        isCasting = false;
        isCasted = true;
        SoundPlayer.Play.Invoke(CastSound);
        Field.OnCast?.Invoke(this);
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
        if (!isCasted && TurnBasedGameplay.myTurn)
        {
            if (!isCasting)
            {
                isCasting = true;
                SoundPlayer.Play.Invoke(SelectSound);
            }
        }
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
        if (collision.CompareTag("field")) field = collision.GetComponent<Field>();
        if (collision.TryGetComponent<CardController>(out CardController card)) if(card.isCasted) otherCard = card;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("field")) field = null;
        if (collision.TryGetComponent<CardController>(out CardController card)) if (card==otherCard) otherCard = null;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("field")) field = collision.GetComponent<Field>();
        if (collision.TryGetComponent<CardController>(out CardController card)) if (card.isCasted) otherCard = card;
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
