using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CardController: MonoBehaviour
{
    [Header("Поля для данных карт")]
    [SerializeField] public TMP_Text title;

    [SerializeField] SpriteRenderer image;
    [SerializeField] public Light2D lighting;
    [SerializeField] Light2D signalLight;

    [SerializeField] public AudioClip CastSound, SelectSound;

    [SerializeField] private BasicCard basicCard;
    public BasicCard GetBasicCard { get { return basicCard; } }
    [SerializeField] CardStats stats;
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

                SoundPlayer.Play(value.SelectSound);
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
        if (Selected != this || Selected == null) hover();
    }
    private void OnMouseExit()
    {
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
        if (runningFunc != null) StopCoroutine(runningFunc);
        if (TurnBasedGameplay.myTurn) runningFunc = StartCoroutine(SmoothSizeChange(new Vector3(1, 1, 1)));

        // все карты
        if (isCasted)
        {
            CardUI.OnOpenCard.Invoke(this);
            if (selected != null) if (GetBasicCard.OnClick(this)) return;
        }

        if (gameObject.CompareTag("enemyCard")) return;
        // только карты игрока

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
        SoundPlayer.Play(CastSound);
        Field.OnCast?.Invoke(this);
        transform.localScale = Vector3.one;
        foreach (Transform t in transform) t.gameObject.SetActive(true);
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
        if (CompareTag("enemyCard")) return;
        if (!isCasted && TurnBasedGameplay.myTurn)
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

    IEnumerator SmoothSizeChange(Vector3 targetScale, bool grow = false)
    {
        if (TurnBasedGameplay.myTurn)
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
