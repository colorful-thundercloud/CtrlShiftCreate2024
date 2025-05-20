using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Serializable]
    public class Stage
    {
        [TextArea(3,3)] public string _text;
        public UnityEvent _event;
        public float _delay;
    }
    [SerializeField] GameManager gameManager;
    [SerializeField] Stage[] stages;
    int stage = 0;
    private void Start()
    {
        Timer.LobbySettings = false;
        GameManager.startGame(true);
        InputManager.Input.Player.AnyKey.performed += onClick;
        GameManager.OnCast.AddListener(onCast);
        GameManager.OnEndTurn.AddListener(onEndTurn);
        GameManager.UpdateTurns.AddListener(onSkill);
        Loading.OnStart.Invoke("Cейчас будет выдан первый ход");
    }
    Coroutine tutorial;
    public void HighlightCard(int id) => StartCoroutine(Highlight(id));
    IEnumerator Highlight(int id)
    {
        yield return new WaitForSeconds(id == 2 ? 1.5f : 0.25f);
        CardController target = gameManager.FindCard(id);
        Vector3 pos = target.transform.position;
        pos.z = pos.z == -20 ? 0 : (id == 1) ? -22 : -20;
        target.transform.position = pos;
        target.SavePosition();
    }
    private void OnDisable()
    {
        InputManager.Input.Player.AnyKey.performed -= onClick;
    }
    private void onClick(InputAction.CallbackContext ctx)
    {
        if (stage > 1 || tutorial != default) return;
        tutorial = StartCoroutine(s0());
    }
    private void onCast(CardController card)
    {
        if (card.cardID > 1 || tutorial != default) return;
        tutorial = StartCoroutine(s0());
    }
    private void onEndTurn(bool myTurn)
    {
        if (stage != 4 || tutorial != default) return;
        tutorial = StartCoroutine(s0());
    }
    private void onSkill(TurnData data)
    {
        if (tutorial != default) return;
        if(data.Action == TurnData.CardAction.directed || data.Action == TurnData.CardAction.user)
            tutorial = StartCoroutine(s0());
    }
    IEnumerator s0()
    {
        Loading.OnStart.Invoke("");
        stages[stage]._event.Invoke();
        yield return new WaitForSeconds(stages[stage]._delay);
        Loading.OnStart.Invoke(stages[stage]._text);
        stage++;
        tutorial = default;
    }
    public void End()
    {
        PlayerPrefs.SetInt("Tutorial", 1);
        GameManager.OnGameOver.Invoke(true);
    }
}
