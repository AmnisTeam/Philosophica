using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


public class BoolCondition : Condition
{
    public bool state = false;
    public override bool CheckCondition()
    {
        return state;
    }
}

public class GameplayManager : MonoBehaviour
{
    private PlayersManager playersManager;
    public QuestionManager questionManager;
    public RegionsSystem regionSystem;
    public TextMeshProUGUI nextQuestionTimeText;
    public ToastShower toast;

    public StateMachine gameStateMachine = new StateMachine();

    public double sessionElapsedTime = 0;

    public int step = 0;
    public int maxStep = 25;

    public double timeToNextQuestion = 10;
    public double timeToChooseTerretory = 10;

    public int currentQuestion = 1;

    private double viewResultsTimer = 0;
    public double viewResultsTime = 7;

    private double regionSelectionTimer = 0;
    public double regionSelectionMaxTime = 10;

    private double preparationTimer = 0;
    public double preparationTime = 10;

    public BoolToastMessage regionSelectionToast;
    public BoolToastMessage preparationToast;

    public BoolCondition askQuestionStateIsEnded;
    public BoolCondition viewResultsStateIsEnded;
    public BoolCondition regionSelectionStateIsEnded;
    public BoolCondition preparationStateIsEnded;

    public void AskQuestionStart()
    {
        askQuestionStateIsEnded.state = false;
        questionManager.setQuestion(currentQuestion);
    }

    public void AskQuestionUpdate()
    {
        if (questionManager.timerToQuestion <= 0)
            askQuestionStateIsEnded.state = true;
    }

    public void ViewResultsStart()
    {
        viewResultsTimer = 0;
        viewResultsStateIsEnded.state = false;
    }

    public void ViewResultsUpdate()
    {
        viewResultsTimer += Time.deltaTime;
        if (viewResultsTimer >= viewResultsTime)
        {
            viewResultsStateIsEnded.state = true;
            questionManager.CloseQuestionMenu();
        }
    }

    public void RegionSelectionStart()
    {
        string winnerNickname = questionManager.tableCompiler.table[0].nickname;
        regionSelectionToast = new BoolToastMessage(winnerNickname + " выбирает территорию");
        toast.showText(regionSelectionToast);
        regionSelectionTimer = 0;
        regionSelectionStateIsEnded.state = false;
    }

    public void RegionSelectionUpdate()
    {
        regionSelectionTimer += Time.deltaTime;

        if (regionSelectionTimer >= regionSelectionMaxTime * 0.9)
            regionSelectionToast.isDone = true;

        if (regionSelectionTimer >= regionSelectionMaxTime)
        {
            regionSelectionStateIsEnded.state = true;          
        }
    }

    public void PreparationStart()
    {
        preparationToast = new BoolToastMessage("Подготовка к следующему вопросу");
        toast.showText(preparationToast);
        preparationTimer = 0;
        preparationStateIsEnded.state = false;
    }

    public void PreparationUpdate()
    {
        preparationTimer += Time.deltaTime;

        if (preparationTimer >= preparationTime * 0.9)
            preparationToast.isDone = true;

        if (preparationTimer >= preparationTime)
        {
            preparationStateIsEnded.state = true;
            currentQuestion = (currentQuestion + 1) % questionManager.questions.Count();
        }
    }

    public void Awake()
    {
        playersManager = GetComponent<PlayersManager>();

        State askQuestionState = new State();
        askQuestionState.startEvents += AskQuestionStart;
        askQuestionState.updateEvents += AskQuestionUpdate;
        gameStateMachine.states.Add(askQuestionState);

        State viewResultsState = new State();
        viewResultsState.startEvents += ViewResultsStart;
        viewResultsState.updateEvents += ViewResultsUpdate;
        gameStateMachine.states.Add(viewResultsState);

        askQuestionStateIsEnded = new BoolCondition();
        Transition fromAskQuestionToViewResults = new Transition(askQuestionStateIsEnded, 0, 1);
        gameStateMachine.transitions.Add(fromAskQuestionToViewResults);

        State regionSelectionState = new State();
        regionSelectionState.startEvents += RegionSelectionStart;
        regionSelectionState.updateEvents += RegionSelectionUpdate;
        gameStateMachine.states.Add(regionSelectionState);

        viewResultsStateIsEnded = new BoolCondition();
        Transition fromViewResultsToRegionSelection = new Transition(viewResultsStateIsEnded, 1, 2);
        gameStateMachine.transitions.Add(fromViewResultsToRegionSelection);

        State preparationState = new State();
        preparationState.startEvents += PreparationStart;
        preparationState.updateEvents += PreparationUpdate;
        gameStateMachine.states.Add(preparationState);

        regionSelectionStateIsEnded = new BoolCondition();
        Transition fromRegionSelectionToPreparation = new Transition(regionSelectionStateIsEnded, 2, 3);
        gameStateMachine.transitions.Add(fromRegionSelectionToPreparation);

        preparationStateIsEnded = new BoolCondition();
        Transition fromPreparationToAskQuestion = new Transition(preparationStateIsEnded, 3, 0);
        gameStateMachine.transitions.Add(fromPreparationToAskQuestion);
    }

    public void Start()
    {
        playersManager.connected(playersManager.config.me);
        playersManager.connected(new Player(0, 0, new Color(255, 0, 0), "SpectreSpect"));
        playersManager.connected(new Player(1, 1, new Color(0, 255, 0), "DotaKot"));
        playersManager.connected(new Player(2, 2, new Color(0, 0, 255), "ThEnd"));

        GrantPlayersStartingRegions();

        gameStateMachine.Start(0);
    }

    public void Update()
    {
        sessionElapsedTime += Time.deltaTime;
        timeToNextQuestion -= Time.deltaTime;

        gameStateMachine.UpdateEvents();
        gameStateMachine.UpdateConditions();

        SetNextQuestionTimeText(timeToNextQuestion);

        UpdateRegionColors();

        //if (timeToNextQuestion <= 0)
        //{
        //    questionManager.setQuestion(currentQuestion);
        //    currentQuestion = (currentQuestion + 1) % questionManager.questions.Count();
        //    timeToNextQuestion = 10;
        //}
    }

    private void GrantPlayersStartingRegions()
    {
        for (int i = 0; i < playersManager.players.count; i++)
            playersManager.players.get(i).ClaimRegion(regionSystem.regionSerds[i].region);
    }

    private void UpdateRegionColors()
    {
        for (int i = 0; i < playersManager.players.count; i++)
        {
            for(int j = 0; j < playersManager.players.get(i).claimedRegions.Count; j++)
            {
                playersManager.players.get(i).claimedRegions[j].SetColor(playersManager.players.get(i).color);
            }
        }
    }
    
    public void SetNextQuestionTimeText(double seconds)
    {
        int minutes = (int)seconds / 60;
        int restOfSeconds = (int)seconds % 60;

        string minutesNonsignificantZero = "";
        string secondsNonsignificantZero = "";

        if (Math.Abs(minutes) < 10)
            minutesNonsignificantZero = "0";
        if (Math.Abs(restOfSeconds) < 10)
            secondsNonsignificantZero = "0";

        nextQuestionTimeText.text = minutesNonsignificantZero + minutes + ":" + secondsNonsignificantZero + restOfSeconds;
    }
}
