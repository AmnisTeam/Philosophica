using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

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
    public TextMeshProUGUI stepsText;
    public ToastShower toast;
    public Camera cam;

    public IconsContent iconsContent;

    public GameObject stageTwoAnnoucment;

    public GameObject offenseAnnouncement;
    public GameObject offenseAnnouncementAvatar;
    public TextMeshProUGUI offenseAnnouncementText;
    public TextMeshProUGUI offenseAnnouncementTimerText;
    public GameObject attackAnnouncement;
    public TextMeshProUGUI attackAnnouncementText;

    public GameObject opponentsAnnouncement;
    public UnityEngine.UI.Image opponentsAnnouncementOpponent1Icon;
    public TextMeshProUGUI opponentsAnnouncementOpponent1Nickname;
    public UnityEngine.UI.Image opponentsAnnouncementOpponent2Icon;
    public TextMeshProUGUI opponentsAnnouncementOpponent2Nickname;

    public GameObject QuestionNumberAnnouncement;

    public StateMachine gameStateMachine = new StateMachine();

    public double sessionElapsedTime = 0;

    public int steps = 0;
    public int maxSteps = 25;

    public double timeToNextQuestion = 10;
    public double timeToChooseTerretory = 10;

    public int currentQuestion = 1;

    private double viewResultsTimer = 0;
    public double viewResultsTime = 7;

    private double regionSelectionTimer = 0;
    public double regionSelectionMaxTime = 10;

    private double preparationTimer = 0;
    public double preparationTime = 10;

    private double stageTwoAnnouncmentTimer = 0;
    public double stageTwoAnnouncmentTime = 0;

    private double offensivePlayerSelectionTimer = 0;
    public double offensivePlayerSelectionTime = 0;

    private double attackAnnouncementTimer = 0;
    public double attackAnnouncementTime;

    private double OpponentsAnnouncementTimer = 0;
    public double OpponentsAnnouncementTime;

    private double questionNumberAnnouncementTimer = 0;
    public double questionNumberAnnouncementTime;

    private double waitTimer = 0;
    private double waiteTime = 0;

    private int winnerRegionsCountAtStartOfSelection;
    private Player winner;
    private Player offensePlayer;

    public Battle battle;

    public BoolToastMessage regionSelectionToast;
    public BoolToastMessage preparationToast;

    public BoolCondition askQuestionStateIsEnded;
    public BoolCondition viewResultsStateIsEnded;
    public BoolCondition regionSelectionStateIsEnded;
    public BoolCondition preparationStateIsEnded;
    public BoolCondition firstStageIsEnded;
    public BoolCondition stageTwoAnnouncementIsEnded;
    public BoolCondition offensivePlayerSelectionIsEnded;
    public BoolCondition attackAnnouncementIsEnded;
    public BoolCondition opponentsAnnouncementIsEnded;
    public BoolCondition questionNumberAnnouncementIsEnded;

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
        winner = questionManager.tableCompiler.table[0];
        winnerRegionsCountAtStartOfSelection = winner.claimedRegions.Count;

        regionSelectionToast = new BoolToastMessage(winner.nickname + " выбирает территорию");
        toast.showText(regionSelectionToast);

        regionSelectionTimer = 0;
        regionSelectionStateIsEnded.state = false;
    }

    public void RegionSelectionUpdate()
    {
        regionSelectionTimer += Time.deltaTime;

        regionSelectionToast.message = winner.nickname + " выбирает территорию: " +
            ((int)(regionSelectionMaxTime - regionSelectionTimer));

        if (winner.id == 4575635)
            GrantRegionToWinnerByMouseClick();

        bool stateEnded = false;
        if (winner.claimedRegions.Count > winnerRegionsCountAtStartOfSelection)
            stateEnded = true;

        if (regionSelectionTimer >= regionSelectionMaxTime)
        {
            GrantRandomFreeRegionToPlayer(winner);
            stateEnded = true;
        }

        if (stateEnded)
        {
            steps++;
            regionSelectionToast.isDone = true;
            if (GetFreeRegionsCount() > 0)
                regionSelectionStateIsEnded.state = true;
            else
                firstStageIsEnded.state = true;
            Wait(0.8);
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

        preparationToast.message = "Подготовка к следующему вопросу: " + ((int)(preparationTime - preparationTimer));

        if (preparationTimer >= preparationTime * 0.9)
            preparationToast.isDone = true;

        if (preparationTimer >= preparationTime)
        {
            preparationStateIsEnded.state = true;
            currentQuestion = (currentQuestion + 1) % questionManager.questions.Count();
        }
    }

    public void stageTwoAnnouncementStart()
    {
        stageTwoAnnoucment.SetActive(true);
        stageTwoAnnoucment.GetComponent<CanvasGroup>().LeanAlpha(1, 0.3f).setEaseLinear();
        stageTwoAnnouncmentTimer = 0;
    }

    public void stageTwoAnnouncementUpdate()
    {
        stageTwoAnnouncmentTimer += Time.deltaTime;

        if (stageTwoAnnouncmentTimer >= stageTwoAnnouncmentTime)
        {
            stageTwoAnnoucment.GetComponent<CanvasGroup>().LeanAlpha(0, 0.3f).setEaseLinear();
            stageTwoAnnouncementIsEnded.state = true;
            Wait(0.5f);
        }
    }

    public void offensivePlayerSelectionStart()
    {
        offensivePlayerSelectionTimer = 0;
        offenseAnnouncement.SetActive(true);
        offenseAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, 0.3f).setEaseLinear();

        offensePlayer = playersManager.players.get(0);
        offenseAnnouncementAvatar.GetComponent<UnityEngine.UI.Image>().sprite = iconsContent.icons[offensePlayer.iconId].sprite;
        offenseAnnouncementAvatar.GetComponent<UnityEngine.UI.Image>().color = offensePlayer.color;

        if (offensePlayer.id == 4575635)
            offenseAnnouncementText.text = "Ваша очередь нападать. Выберете, на кого хотите напасть.";
        else
            offenseAnnouncementText.text = offensePlayer.nickname + " выбирает на кого напасть.";

        offenseAnnouncementTimerText.text = GetTimeStr(offensivePlayerSelectionTime);
        battle = null;
    }

    public void offensivePlayerSelectionUpdate()
    {
        int roundsCount = 3;
        double maxPlayersHealth = 100;
        bool stateEnded = false;
        offensivePlayerSelectionTimer += Time.deltaTime;
        offenseAnnouncementTimerText.text = GetTimeStr(offensivePlayerSelectionTime - offensivePlayerSelectionTimer);

        if (offensePlayer.id == 4575635)
            StartBattleByMouseClick(roundsCount, maxPlayersHealth);

        if (battle != null)
            stateEnded = true;

        if (offensivePlayerSelectionTimer >= offensivePlayerSelectionTime)
        {
            StartBattleWithRandomPlayer(roundsCount, maxPlayersHealth);
            stateEnded = true;
        }
        
        if (stateEnded)
        {
            offensivePlayerSelectionIsEnded.state = true;
            offenseAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, 0.3f).setEaseLinear();
            Wait(0.5f);
        }
    }

    public void AttackAnnouncementStart()
    {
        attackAnnouncement.SetActive(true);
        attackAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, 0.3f).setEaseLinear();
        attackAnnouncementText.text = battle.opponent1.player.nickname + " напал на " + battle.opponent2.player.nickname + ".";
        attackAnnouncementTimer = 0;
    }

    public void AttackAnnouncementUpdate()
    {
        attackAnnouncementTimer += Time.deltaTime;

        if (attackAnnouncementTimer >= attackAnnouncementTime)
        {
            attackAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, 0.3f).setEaseLinear();
            attackAnnouncementIsEnded.state = true;
            Wait(0.5f);
        }
    }

    public void OpponentsAnnouncementStart()
    {
        OpponentsAnnouncementTimer = 0;
        opponentsAnnouncement.SetActive(true);
        opponentsAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, 0.3f).setEaseLinear();

        Player player1 = battle.opponent1.player;
        Player player2 = battle.opponent2.player;


        opponentsAnnouncementOpponent1Icon.sprite = iconsContent.icons[player1.iconId].sprite;
        opponentsAnnouncementOpponent1Icon.color = player1.color;

        opponentsAnnouncementOpponent1Nickname.text = player1.nickname;
        opponentsAnnouncementOpponent1Nickname.color = player1.color;


        opponentsAnnouncementOpponent2Icon.sprite = iconsContent.icons[player2.iconId].sprite;
        opponentsAnnouncementOpponent2Icon.color = player2.color;

        opponentsAnnouncementOpponent2Nickname.text = player2.nickname;
        opponentsAnnouncementOpponent2Nickname.color = player2.color;     
    }

    public void OpponentsAnnouncementUpdate()
    {
        OpponentsAnnouncementTimer += Time.deltaTime;

        if (OpponentsAnnouncementTimer >= OpponentsAnnouncementTime)
        {
            opponentsAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, 0.3f).setEaseLinear();
            opponentsAnnouncementIsEnded.state = true;
            Wait(0.5);
        }
    }


    public void QuestionNumberAnnouncementStart()
    {
        questionNumberAnnouncementTimer = 0;
        QuestionNumberAnnouncement.SetActive(true);
        QuestionNumberAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, 0.3f).setEaseLinear();
    }

    public void QuestionNumberAnnouncementUpdate()
    {
        questionNumberAnnouncementTimer += Time.deltaTime;

        if (questionNumberAnnouncementTimer >= questionNumberAnnouncementTime)
        {     
            QuestionNumberAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, 0.3f).setEaseLinear();
            questionNumberAnnouncementIsEnded.state = true;
            Wait(0.5f);
        }
    }

    public void AskQuestionInBattleStart()
    {
        Debug.Log("AskQuestionInBattleStart");
    }

    public void AskQuestionInBattleUpdate()
    {

    }

    public void Awake()
    {
        playersManager = GetComponent<PlayersManager>();

        State askQuestionState = new State(); // 0
        askQuestionState.startEvents += AskQuestionStart;
        askQuestionState.updateEvents += AskQuestionUpdate;
        gameStateMachine.states.Add(askQuestionState);

        State viewResultsState = new State(); // 1
        viewResultsState.startEvents += ViewResultsStart;
        viewResultsState.updateEvents += ViewResultsUpdate;
        gameStateMachine.states.Add(viewResultsState);

        askQuestionStateIsEnded = new BoolCondition();
        Transition fromAskQuestionToViewResults = new Transition(askQuestionStateIsEnded, 0, 1);
        gameStateMachine.transitions.Add(fromAskQuestionToViewResults);

        State regionSelectionState = new State(); // 2
        regionSelectionState.startEvents += RegionSelectionStart;
        regionSelectionState.updateEvents += RegionSelectionUpdate;
        gameStateMachine.states.Add(regionSelectionState);

        viewResultsStateIsEnded = new BoolCondition();
        Transition fromViewResultsToRegionSelection = new Transition(viewResultsStateIsEnded, 1, 2);
        gameStateMachine.transitions.Add(fromViewResultsToRegionSelection);

        State preparationState = new State(); // 3
        preparationState.startEvents += PreparationStart;
        preparationState.updateEvents += PreparationUpdate;
        gameStateMachine.states.Add(preparationState);

        regionSelectionStateIsEnded = new BoolCondition();
        Transition fromRegionSelectionToPreparation = new Transition(regionSelectionStateIsEnded, 2, 3);
        gameStateMachine.transitions.Add(fromRegionSelectionToPreparation);

        preparationStateIsEnded = new BoolCondition();
        Transition fromPreparationToAskQuestion = new Transition(preparationStateIsEnded, 3, 0);
        gameStateMachine.transitions.Add(fromPreparationToAskQuestion);

        State stageTwoAnnouncementState = new State(); // 4
        stageTwoAnnouncementState.startEvents += stageTwoAnnouncementStart;
        stageTwoAnnouncementState.updateEvents += stageTwoAnnouncementUpdate;
        gameStateMachine.states.Add(stageTwoAnnouncementState);

        firstStageIsEnded = new BoolCondition();
        Transition fromRegionSelectionToStageTwoAnnouncement = new Transition(firstStageIsEnded, 2, 4);
        gameStateMachine.transitions.Add(fromRegionSelectionToStageTwoAnnouncement);

        State offensivePlayerSelectionState = new State(); // 5
        offensivePlayerSelectionState.startEvents += offensivePlayerSelectionStart;
        offensivePlayerSelectionState.updateEvents += offensivePlayerSelectionUpdate;
        gameStateMachine.states.Add(offensivePlayerSelectionState);

        stageTwoAnnouncementIsEnded = new BoolCondition();
        Transition fromStageTwoAnnouncementToOffensivePlayerSelection = new Transition(stageTwoAnnouncementIsEnded, 4, 5);
        gameStateMachine.transitions.Add(fromStageTwoAnnouncementToOffensivePlayerSelection);

        State attackAnnouncementState = new State(); // 6
        attackAnnouncementState.startEvents += AttackAnnouncementStart;
        attackAnnouncementState.updateEvents += AttackAnnouncementUpdate;
        gameStateMachine.states.Add(attackAnnouncementState);

        offensivePlayerSelectionIsEnded = new BoolCondition();
        Transition fromOffensivePlayerSelectionToAttackAnnouncement = new Transition(offensivePlayerSelectionIsEnded, 5, 6);
        gameStateMachine.transitions.Add(fromOffensivePlayerSelectionToAttackAnnouncement);

        State opponentsAnnouncementState = new State(); // 7
        opponentsAnnouncementState.startEvents += OpponentsAnnouncementStart;
        opponentsAnnouncementState.updateEvents += OpponentsAnnouncementUpdate;
        gameStateMachine.states.Add(opponentsAnnouncementState);

        attackAnnouncementIsEnded = new BoolCondition();
        Transition fromAttackAnnouncementToOpponentsAnnouncement = new Transition(attackAnnouncementIsEnded, 6, 7);
        gameStateMachine.transitions.Add(fromAttackAnnouncementToOpponentsAnnouncement);

        State questionNumberAnnouncementState = new State(); // 8
        questionNumberAnnouncementState.startEvents += QuestionNumberAnnouncementStart;
        questionNumberAnnouncementState.updateEvents += QuestionNumberAnnouncementUpdate;
        gameStateMachine.states.Add(questionNumberAnnouncementState);

        opponentsAnnouncementIsEnded = new BoolCondition();
        Transition fromopponentsAnnouncementToquestionNumberAnnouncement = new Transition(opponentsAnnouncementIsEnded, 7, 8);
        gameStateMachine.transitions.Add(fromopponentsAnnouncementToquestionNumberAnnouncement);

        State askQuestionInBattleState = new State(); // 9
        askQuestionInBattleState.startEvents += AskQuestionInBattleStart;
        askQuestionInBattleState.updateEvents += AskQuestionInBattleUpdate;
        gameStateMachine.states.Add(askQuestionInBattleState);

        questionNumberAnnouncementIsEnded = new BoolCondition();
        Transition fromQuestionNumberAnnouncementToAskQuestionInBattle = new Transition(questionNumberAnnouncementIsEnded, 8, 9);
        gameStateMachine.transitions.Add(fromQuestionNumberAnnouncementToAskQuestionInBattle);
    }

    public void Start()
    {
        playersManager.connected(playersManager.config.me);
        playersManager.connected(new Player(0, 0, new UnityEngine.Color(1f, 0.3725f, 0.396f), "SpectreSpect"));
        playersManager.connected(new Player(1, 1, new UnityEngine.Color(0.372f, 0.4745f, 1f), "DotaKot"));
        playersManager.connected(new Player(2, 2, new UnityEngine.Color(0.549f, 1f, 0.372f), "ThEnd"));

        GrantPlayersStartingRegions();

        gameStateMachine.Start(0);
    }

    public void Update()
    {
        sessionElapsedTime += Time.deltaTime;
        timeToNextQuestion -= Time.deltaTime;
        waitTimer += Time.deltaTime;

        if (waitTimer >= waiteTime)
        {
            gameStateMachine.UpdateConditions();
            gameStateMachine.UpdateEvents();        
        }

        SetNextQuestionTimeText(timeToNextQuestion);
        SetStepsText(steps, maxSteps);

        UpdateRegionColors();
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

    public string GetTimeStr(double seconds)
    {
        int minutes = (int)seconds / 60;
        int restOfSeconds = (int)seconds % 60;

        string minutesNonsignificantZero = "";
        string secondsNonsignificantZero = "";

        if (Math.Abs(minutes) < 10)
            minutesNonsignificantZero = "0";
        if (Math.Abs(restOfSeconds) < 10)
            secondsNonsignificantZero = "0";

        return minutesNonsignificantZero + minutes + ":" + secondsNonsignificantZero + restOfSeconds;
    }

    public void Wait(double seconds)
    {
        waiteTime = seconds;
        waitTimer = 0;
    }

    public void GrantRandomFreeRegionToPlayer(Player player)
    {
        for (int r = 0; r < regionSystem.regionSerds.Count; r++)
        {
            Region region = regionSystem.regionSerds[r].region;

            bool found = false;
            for (int p = 0; p < playersManager.players.count; p++)
            {
                for (int c = 0; c < playersManager.players.get(p).claimedRegions.Count; c++)
                {
                    if (region == playersManager.players.get(p).claimedRegions[c])
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            if (!found)
            {
                player.ClaimRegion(region);
                break;
            }       
        }
    }

    public void GrantRegionToWinnerByMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(new Ray(mouseWorldPos, Vector3.forward), out hit))
            {
                if (hit.collider.gameObject.GetComponent<Region>())
                {
                    Region region = hit.collider.gameObject.GetComponent<Region>();
                    bool isAlreadyClaimed = false;
                    for (int i = 0; i < playersManager.players.count; i++)
                    {
                        for (int j = 0; j < playersManager.players.get(i).claimedRegions.Count; j++)
                        {
                            if (region == playersManager.players.get(i).claimedRegions[j])
                            {
                                isAlreadyClaimed = true;
                                break;
                            }
                        }
                        if (isAlreadyClaimed)
                            break;
                    }

                    if (!isAlreadyClaimed)
                        winner.ClaimRegion(region);
                }
            }
        }
    }

    public void SetStepsText(int steps, int maxSteps)
    {
        stepsText.text = steps + "/" + maxSteps;
    }

    public int GetFreeRegionsCount()
    {
        int claimedRegions = 0;
        for (int p = 0; p < playersManager.players.count; p++)
            claimedRegions += playersManager.players.get(p).claimedRegions.Count;
        int freeRegions = regionSystem.regionSerds.Count - claimedRegions;
        return freeRegions;
    }

    public Battle StartBattle(Player player1, Player player2, int roundsCount, double playersMaxHealth)
    {
        Opponent opponent1 = new Opponent(player1, playersMaxHealth, playersMaxHealth, 0);
        Opponent opponent2 = new Opponent(player2, playersMaxHealth, playersMaxHealth, 0);
        Battle newBattle = new Battle(opponent1, opponent2);

        if (roundsCount > questionManager.questions.Count())
            roundsCount = questionManager.questions.Count();

        int idsCount = questionManager.questions.Count();
        int[] ids = new int[idsCount];
        for (int i = 0; i < idsCount; i++)
            ids[i] = i;

        System.Random rnd = new System.Random();
        for (int i = 0; i < roundsCount; i++)
        {
            int randInt = rnd.Next(0, idsCount - 1);
            QuestionManager.Question randQuestion = questionManager.questions[ids[randInt]];

            ids[randInt] = ids[idsCount - 1];
            idsCount--;

            newBattle.questions.Add(randQuestion);
        }

        return newBattle;
    }

    public void StartBattleByMouseClick(int roundsCount, double playersMaxHealth)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(new Ray(mouseWorldPos, Vector3.forward), out hit))
            {
                if (hit.collider.gameObject.GetComponent<Region>())
                {
                    Region region = hit.collider.gameObject.GetComponent<Region>();

                    bool regionIsAlreadyClaimed = false;
                    for (int i = 0; i < offensePlayer.claimedRegions.Count; i++)
                    {
                        if (region == offensePlayer.claimedRegions[i])
                        {
                            regionIsAlreadyClaimed = true;
                            break;
                        }
                    }

                    if (!regionIsAlreadyClaimed)
                    {
                        Player regionHost = null;
                        for (int p = 0; p < playersManager.players.count; p++)
                        {
                            for (int r = 0; r < playersManager.players.get(p).claimedRegions.Count; r++)
                            {
                                if (region == playersManager.players.get(p).claimedRegions[r])
                                {
                                    regionHost = playersManager.players.get(p);
                                    break;
                                }
                            }
                            if (regionHost != null)
                                break;
                        }
                        battle = StartBattle(offensePlayer, regionHost, roundsCount, playersMaxHealth);
                    }

                }
            }
        }
    }

    public void StartBattleWithRandomPlayer(int roundsCount, double playersMaxHealth)
    {
        int player1Id = -1;
        for (int i = 0; i < playersManager.players.count; i++)
        {
            if (offensePlayer == playersManager.players.get(i))
            {
                player1Id = i;
                break;
            }
        }

        int idsCount = playersManager.players.count;
        int[] ids = new int[idsCount];
        for (int i = 0; i < idsCount; i++)
            ids[i] = i;

        (ids[player1Id], ids[idsCount - 1]) = (ids[idsCount - 1], ids[player1Id]);
        idsCount--;

        System.Random rnd = new System.Random();

        int randomPlayerId = rnd.Next(0, idsCount);
        Player randomPlayer = playersManager.players.get(ids[randomPlayerId]);

        battle = StartBattle(offensePlayer, randomPlayer, roundsCount, playersMaxHealth);
    }
}