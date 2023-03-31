using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;

public class BoolCondition : Condition
{
    public bool state = false;

    public virtual void Set(bool state)
    {
        this.state = state;
    }

    public override bool CheckCondition()
    {
        bool result = state;
        if (result)
            state = false;
        return result;
    }
}

public class SynchronizedBoolCondition : BoolCondition
{
    public bool wantToSynchronized = false;
    public List<SynchronizedBoolCondition> playersConditions;
    public PlayersManager playersManager;
    public PhotonView pv;
    public float time = 1;
    public float timer;

    public SynchronizedBoolCondition(PlayersManager playersManager, PhotonView pv, bool wantToSynchronized)
    {
        this.playersManager = playersManager;
        this.pv = pv;
        this.wantToSynchronized = true;
        playersConditions = new List<SynchronizedBoolCondition>();
        timer = time;
    }

    public void UpdateCountConditions()
    {
        if (playersConditions.Count != playersManager.players.count)
        {
            playersConditions = new List<SynchronizedBoolCondition>();
            for (int x = 0; x < playersManager.players.count; x++)
                playersConditions.Add(new SynchronizedBoolCondition(playersManager, pv, true));
        }
    }

    public void Synchronize()
    {
        UpdateCountConditions();
        pv.RPC("RPC_SynchronizedPlayers", RpcTarget.All, state, wantToSynchronized, id);
    }

    public void SetWantToSynchronized(bool wantToSynchronized)
    {
        UpdateCountConditions();
        this.wantToSynchronized = wantToSynchronized;
    }

    public override void Set(bool state)
    {
        UpdateCountConditions();
        this.state = state;
    }

    public override bool CheckCondition()
    {
        timer += Time.deltaTime;
        if(timer >= time)
        {
            Synchronize();
            timer = 0;
        }

        bool result = state;
        if (result && wantToSynchronized)
        {
            for (int x = 0; x < playersConditions.Count; x++)
            {
                if (playersConditions[x].wantToSynchronized)
                {
                    result = playersConditions[x].state;
                    if (!result)
                        break;
                }
            }
        }

        if(result)
            state = false;

        return result;
    }
}

public class FastedWinner
{
    public Player player;
    public string answer;
    public float timeToAnswer;
}

public class GameplayManager : MonoBehaviourPunCallbacks
{
    private PlayersManager playersManager;
    public QuestionManager questionManager;
    public RegionsSystem regionSystem;
    public ScoreTableManager scoreTableManager;
    public TextMeshProUGUI nextQuestionTimeText;
    public TextMeshProUGUI stepsText;
    public ToastShower toast;
    public Camera cam;
    public FastedWinner fastedWinner;

    public Color unclaimedRegionColor;

    public IconsContentHolder iconsContent;
    public ColorsHolder colorsHolderInstance;
    public PhotonView pv;

    public GameObject stageTwoAnnoucment;

    public GameObject offenseAnnouncement;
    public GameObject offenseAnnouncementAvatar;
    public TextMeshProUGUI offenseAnnouncementText;
    public TextMeshProUGUI offenseAnnouncementTimerText;
    public GameObject attackAnnouncement;
    public TextMeshProUGUI attackAnnouncementText;

    public GameObject losePlayerAnnouncement;
    public TextMeshProUGUI losePlayerAnnouncementText;

    public GameObject opponentsAnnouncement;
    public UnityEngine.UI.Image opponentsAnnouncementOpponent1Icon;
    public TextMeshProUGUI opponentsAnnouncementOpponent1Nickname;
    public UnityEngine.UI.Image opponentsAnnouncementOpponent2Icon;
    public TextMeshProUGUI opponentsAnnouncementOpponent2Nickname;


    public GameObject gameBeggining;
    public GameObject firstStageHint;
    public GameObject questionMenu;
    public GameObject questionMenu1;
    public GameObject questionMenuTable;
    public GameObject secondStageHint;
    public GameObject questionNumberAnnouncement;
    public TextMeshProUGUI questionNumberAnnouncementText;
    public GameObject askQuestionInBattle;
    public GameObject battleRoundResults;
    public GameObject battleResultsVictory;
    public GameObject battleResultsDraw;

    public GameObject uiInterface;
    public GameObject endGameAnnouncment;
    public GameObject loadingScreen;
    public GameObject endGameMenu;

    [SerializeField] private PlaySound playSound;

    public StateMachine gameStateMachine = new StateMachine();

    public System.Random random;

    public double sessionElapsedTime = 0;

    public int steps = 0;
    public int maxSteps = 25;

    public int countScoresForRegion;

    public float gameBegginingTime = 5.0f;
    private float gameBegginingTimer = 0.0f;

    public float firstStageHintTime = 10.0f;
    public float firstStageHintTimer = 0.0f;

    public float secondStageHintTime = 10.0f;
    private float secondStageHintTimer = 0.0f;

    public float menusTransitionTime = 0.3f;
    public float menusTransitionDelayTime = 0.2f;

    public float timeToShowCorrectAnswer = 1.5f;
    private float timerToShowCorrectAnswer = 0;

    public float timeToShowTableMenu = 5;
    private float timerToShowTableMenu = 0;

    public double timeToNextQuestion = 10;
    public double timeToChooseTerretory = 10;

    public int currentQuestion = 1;
    public int currentOffensivePlayer;

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

    public double askQuestionInBattleTime;

    private double correctAnsewerRevealingTimer;
    public double correctAnsewerRevealingTime;

    private double BattleRoundResultsTimer;
    public double BattleRoundResultsTime;

    public double battleRoundResultsNotificationDelay;

    private double damageDealingDelayTimer;
    public double damageDealingDelay;

    public float opponentsHealthUpdatingTime = 0.4f;
    public float battleRoundResultsDisappearingTime = 4f;

    private double battleResultsTimer;
    public double battleResultsTime;

    private double closeBattleResultsTimer;
    public double closeBattleResultsTime;

    public double endGameAnnouncmentTime;
    public double endGameLoadingScreenTime;

    public double losePlayerAnnouncmentTime;

    private double waitTimer = 0;
    private double waiteTime = 0;

    private List<int> regionIndexes = new List<int>();
    private bool wasRpcSent = false;

    public bool onceAddSteps = false;
    private int winnerRegionsCountAtStartOfSelection;
    private Player winner;
    private Player offensePlayer;

    public QuestionSession questionSession;
    public Battle battle;

    public BoolToastMessage regionSelectionToast;
    public BoolToastMessage preparationToast;


    //public BoolCondition fromGameBegginingToAskQuestion;
    public SynchronizedBoolCondition fromFirstStageHintToAskQuestion;
    public SynchronizedBoolCondition fromGameBegginingToFirstStageHint;
    public SynchronizedBoolCondition askQuestionStateIsEnded;
    public SynchronizedBoolCondition fromAskQuestionToRightAnswer;
    public SynchronizedBoolCondition fromRightAnswerToShowResultsInAskQuestion;
    public SynchronizedBoolCondition fromShowResultsInAskQuestionToRegionSelection;
    public SynchronizedBoolCondition viewResultsStateIsEnded;
    public SynchronizedBoolCondition regionSelectionStateIsEnded;
    public SynchronizedBoolCondition preparationStateIsEnded;
    public SynchronizedBoolCondition firstStageIsEnded;
    public SynchronizedBoolCondition stageTwoAnnouncementIsEnded;
    public SynchronizedBoolCondition fromSecondStageHintToOffensivePlayerSelection;
    public SynchronizedBoolCondition fromStageTwoAnnouncementToSecondStageHint;
    public SynchronizedBoolCondition offensivePlayerSelectionIsEnded;
    public SynchronizedBoolCondition attackAnnouncementIsEnded;
    public SynchronizedBoolCondition opponentsAnnouncementIsEnded;
    public SynchronizedBoolCondition questionNumberAnnouncementIsEnded;
    public SynchronizedBoolCondition askQuestionInBattleIsEnded;
    public SynchronizedBoolCondition correctAnsewerRevealingInBattleIsEnded;
    public SynchronizedBoolCondition roundIsEnded;
    public SynchronizedBoolCondition battleCond;
    public SynchronizedBoolCondition fromBattleResultsToOffensive;
    public SynchronizedBoolCondition fromBattleResultsToEndGame;
    public SynchronizedBoolCondition fromBattleResultsToLosePlayer;
    public SynchronizedBoolCondition fromLosePlayerToEndGame;
    public SynchronizedBoolCondition fromLosePlayerToOffensive;

    [PunRPC]
    public void RPC_SynchronizedPlayers(bool state, bool wantToSynchronized, int conditionId, PhotonMessageInfo info)
    {
        int playerId = -1;
        for (int x = 0; x < playersManager.players.count; x++)
        {
            if (playersManager.players.get(x).id == info.Sender.ActorNumber - 1)
            {
                playerId = x;
                break;
            }
        }

        SynchronizedBoolCondition condition = null;
        for (int x = 0; x < gameStateMachine.transitions.Count; x++)
        {
            if (gameStateMachine.transitions[x].condition.id == conditionId)
                condition = (SynchronizedBoolCondition)gameStateMachine.transitions[x].condition;
        }

        condition.UpdateCountConditions();
        condition.playersConditions[playerId].Set(state);
        condition.playersConditions[playerId].SetWantToSynchronized(wantToSynchronized);
    }


    public void GameBegginingStart()
    {
        gameBeggining.SetActive(true);
        gameBeggining.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();
        gameBegginingTimer = 0;
    }

    public void GameBegginingUpdate()
    {
        Debug.Log("GameBegginingUpdate");
        gameBegginingTimer += Time.deltaTime;
        if (gameBegginingTimer >= gameBegginingTime)
        {    
            gameBeggining.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() =>
            {
                gameBeggining.SetActive(false);
            });
            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () =>
            {
                fromGameBegginingToFirstStageHint.Set(true);
            });       
        }
    }

    public void FirstStageHintStart()
    {
        if (PhotonNetwork.IsMasterClient)
            //pv.RPC("RPC_InitQuestionSession", RpcTarget.All, 1);
            pv.RPC("RPC_InitQuestionSession", RpcTarget.All, (int)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());

        firstStageHint.SetActive(true);
        firstStageHint.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();

        firstStageHintTimer = 0;
        playSound.SoundPlay("new_stage");
    }

    public void FirstStageHintUpdate()
    {
        firstStageHintTimer += Time.deltaTime;

        if (firstStageHintTimer >= firstStageHintTime)
        {
            firstStageHint.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() =>
            {
                firstStageHint.SetActive(false);
            });
            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () =>
            {
                fromFirstStageHintToAskQuestion.Set(true);
            });
        }
    }

    [PunRPC]
    public void RPC_SelectAnswerInQuestionMenu(int answerId, float timeToAnswer, PhotonMessageInfo info)
    {
        questionSession.SetAnswerData(info.Sender.ActorNumber - 1, answerId, timeToAnswer);
    }

    public void SelectSelfAnswerInQuestionMenu(int buttonID)
    {
        pv.RPC("RPC_SelectAnswerInQuestionMenu", RpcTarget.All, buttonID, (float)questionMenu1.GetComponent<AskQuestionInQuestionMenu>().timer);
    }

    public void AskQuestionStart()
    {
        //questionMenu.SetActive(true);

        //questionManager.content.SetActive(true);
        //questionManager.content.GetComponent<CanvasGroup>().alpha = 1;

        //questionManager.tableMenu.SetActive(false);
        //questionManager.tableMenu.GetComponent<CanvasGroup>().alpha = 0;

        //questionMenu.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();

        //questionManager.setQuestion(currentQuestion);

        questionSession.NextQuestion();
        questionSession.UpdateCountPlayerAnswerData();
        questionMenu1.SetActive(true);
        questionMenu1.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime);
        questionMenu1.GetComponent<AskQuestionInQuestionMenu>().Init(questionSession.GetCurrentQuestion());
        questionMenu1.GetComponent<AskQuestionInQuestionMenu>().timer = 0;

        // какая-то неведомая херня, работает или нет – неизвестно (тип вырубить кнопки ответа спектатору и поменять цвет на серый (кнопка не активна))
        for (int i = 0; i < playersManager.players.count; i++) {
            if (playersManager.players.get(i).isLose && playersManager.players.get(i).isLocalClient) {
                Button[] buttons = questionMenu1.GetComponent<AskQuestionInQuestionMenu>().answerButtons;
                for (int j = 0; j < buttons.Length; j++) {
                    buttons[i].enabled = false;
                }
            }
        }

        //askQuestionStateIsEnded.state = false;
    }

    public void AskQuestionUpdate()
    {
        //if (questionManager.timerToQuestion <= 0)
        //    askQuestionStateIsEnded.state = true;

        AskQuestionInQuestionMenu askQuestionInQuestionMenu = questionMenu1.GetComponent<AskQuestionInQuestionMenu>();
        askQuestionInQuestionMenu.timer += Time.deltaTime;
        float timeLeft = (float)(questionSession.GetCurrentQuestion().timeToQuestion - askQuestionInQuestionMenu.timer);
        if (timeLeft >= 0)
            askQuestionInQuestionMenu.timerText.text = GlobalVariables.GetTimeStr(timeLeft);             
                //(timeLeft / 60).ToString("00") + ":" + (timeLeft % 60).ToString("00");

        if(askQuestionInQuestionMenu.timer >= questionSession.GetCurrentQuestion().timeToQuestion)
        {
            fromAskQuestionToRightAnswer.Set(true);
            askQuestionInQuestionMenu.timer = float.NaN;
        }
    }

    public void RightAnswerStart()
    {
        questionMenu1.GetComponent<AskQuestionInQuestionMenu>().ShowCorrectAnswer();
        timerToShowCorrectAnswer = 0;
    }

    public void RightAnswerUpdate()
    {
        timerToShowCorrectAnswer += Time.deltaTime;
        if(timerToShowCorrectAnswer > timeToShowCorrectAnswer)
        {
            timerToShowCorrectAnswer = float.NaN;
            questionMenu1.GetComponent<AskQuestionInQuestionMenu>().HideCorrectAnswer();
            questionMenu1.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setDelay(menusTransitionTime).setOnComplete(() => { questionMenu1.SetActive(false); });
            fromRightAnswerToShowResultsInAskQuestion.Set(true);
        }
    }

    public void ShowResultsInAskQuestionStart()
    {
        questionMenuTable.SetActive(true);
        questionMenuTable.GetComponent<TableMenu>().compileTheTable(questionSession.playerAnswerData);
        questionMenuTable.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime);
        timerToShowTableMenu = 0;
    }

    public void ShowResultsInAskQuestionUpdate()
    {
        timerToShowTableMenu += Time.deltaTime;
        if(timerToShowTableMenu >= timeToShowTableMenu)
        {
            questionMenuTable.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setOnComplete(() => { 
                questionMenuTable.SetActive(false);
                fromShowResultsInAskQuestionToRegionSelection.Set(true);
            });
            timerToShowTableMenu = float.NaN;
        }
    }

    //public void ViewResultsStart()
    //{
    //    viewResultsTimer = 0;
    //    viewResultsStateIsEnded.state = false;
    //}

    //public void ViewResultsUpdate()
    //{
    //    viewResultsTimer += Time.deltaTime;
    //    if (viewResultsTimer >= viewResultsTime)
    //    {
    //        viewResultsStateIsEnded.state = true;
    //        questionManager.CloseQuestionMenu();
    //    }
    //}

    public bool isPlayerValid(Player player)
    {
        for (int i = 0; i < playersManager.players.count; i++)
            if (player == playersManager.players.get(i))
                return true;
        return false;
    }

    public void RegionSelectionStart()
    {
        if (questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer)
        {
            winner = questionMenuTable.GetComponent<TableMenu>().table[0];
            if (!isPlayerValid(winner))
                winner = null;

            if (winner != null)
            {
                winnerRegionsCountAtStartOfSelection = winner.claimedRegions.Count;

                regionSelectionToast = new BoolToastMessage($"<color=#{winner.color.ToHexString()}>{winner.nickname}</color> выбирает территорию");
                toast.showText(regionSelectionToast);
            }
            regionSelectionTimer = 0;
            wasRpcSent = false;
        }
    }

    public void RegionSelectionUpdate()
    {
        bool stateEnded = false;
        regionSelectionTimer += Time.deltaTime;

        if (winner != null)
        {
            if (questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer)
            {
                regionSelectionToast.message = $"<color=#{winner.color.ToHexString()}>{winner.nickname}</color> выбирает территорию: {(int)(regionSelectionMaxTime - regionSelectionTimer)}";
            }

            if (questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer)
            {
                if (winner.isLocalClient)
                {
                    GrantRegionToWinnerByMouseClick();
                }
            }


            if (questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer)
            {
                if (winner.claimedRegions.Count > winnerRegionsCountAtStartOfSelection)
                {
                    stateEnded = true;
                }
            }

            if (regionSelectionTimer >= regionSelectionMaxTime || !questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer) {
                if (questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer) {
                    if (winner.isLocalClient) {
                        GrantRandomFreeRegionToPlayer(winner);
                    }
                }
                stateEnded = true;
            }
        }
        else
        {
            stateEnded = true;
        }  

        /*if (stateEnded && !onceAddSteps) {
            onceAddSteps = true;
            steps++;
            SetStepsText(steps, maxSteps);

            if (questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer)
            {
                if (regionSelectionToast != null)
                    regionSelectionToast.isDone = true;
            }

            if (GetFreeRegionsCount() > 0)
            {
                regionSelectionStateIsEnded.state = true;
            }
            else
            {
                firstStageIsEnded.state = true;
            }
        }*/
    }

    public void GiveRegion(Player player, Region region)
    {
        player.ClaimRegion(region);
        region.hostPlayer = player;
    }
    
    [PunRPC]
    public void RPC_RegionWasChosen(int regionId, int playerColorId, string source) {
        //Debug.LogError($"Player {playerId} has claimed region {regionId}");

        Region region = regionSystem.regionSerds[regionId].region;
        Player player = null;

        for (int i = 0; i < playersManager.players.count; i++) {
            if (playersManager.players.get(i).colorId == playerColorId) {
                player = playersManager.players.get(i);
                break;
            }
        }

        //player.ClaimRegion(region);
        GiveRegion(player, region);

        regionIndexes.Remove(regionId);
        //Debug.LogError($"Free regions: {string.Join(" ", regionIndexes)} ({GetFreeRegionsCount()})");

        player.scores += countScoresForRegion;
        scoreTableManager.UpdateTable();

        if (region && questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer) {
            Vector2 regionCenter = Vector2.zero;
            for (int x = 0; x < region.GetComponent<MeshFilter>().mesh.vertices.Length; x++) {
                regionCenter += region.GetComponent<MeshFilter>().mesh.vertices[x].ToXY();
            }

            regionCenter /= region.GetComponent<MeshFilter>().mesh.vertices.Length;

            //pv.RPC("RPC_MoveCameraToChoosenRegion", RpcTarget.All, regionCenter.x, regionCenter.y);
            Camera.main.GetComponent<MoveCameraToActiveRegion>().SetTarget(new Vector2(regionCenter.x, regionCenter.y));
        }

        steps++;
        SetStepsText(steps, maxSteps);

        if (questionMenuTable.GetComponent<TableMenu>().isHaveRightAnswer) {
            regionSelectionToast.isDone = true;
        }

        if (GetFreeRegionsCount() > 0) {
            regionSelectionStateIsEnded.state = true;
        } else {
            firstStageIsEnded.state = true;
        }
    }

    /*[PunRPC]
    public void RPC_MoveCameraToChoosenRegion(float posX, float posY) {
        Camera.main.GetComponent<MoveCameraToActiveRegion>().SetTarget(new Vector2(posX, posY));
    }*/

    [PunRPC]
    public void RPC_RegionDistribution(Dictionary<int, int> info) {
        foreach (var rec in info) {
            //Debug.LogError($"Player {rec.Key} has claimed region {rec.Value}");

            Region region = regionSystem.regionSerds[rec.Value].region;
            Player player = playersManager.players.get(rec.Key);

            //player.ClaimRegion(region);
            GiveRegion(player, region);
            regionIndexes.Remove(rec.Value);
            //Debug.LogError($"Free regions: {string.Join(" ", regionIndexes)} ({GetFreeRegionsCount()})");
        }

        scoreTableManager.UpdateTable();
    }

    public void GrantRandomFreeRegionToPlayer(Player player) {
        if (!wasRpcSent) {
            List<int> freeRegionsIndices = GetFreeRegionsIndices();
            int randId = random.Next(0, freeRegionsIndices.Count);
            pv.RPC("RPC_RegionWasChosen", RpcTarget.All, freeRegionsIndices[randId], player.colorId, "GrantRandomFreeRegionToPlayer()");
            wasRpcSent = true;

            //int randRegIdx = random.Next(0, regionIndexes.Count);
            //pv.RPC("RPC_RegionWasChosen", RpcTarget.All, regionIndexes[randRegIdx], player.id, "GrantRandomFreeRegionToPlayer()");
            //wasRpcSent = true;
        }
    }

    public List<int> GetFreeRegionsIndices()
    {
        List<int> freeRegionsIndices = new List<int>();
        for (int i = 0; i < regionSystem.regionSerds.Count; i++)
        {
            if (regionSystem.regionSerds[i].region.hostPlayer == null)
                freeRegionsIndices.Add(i);
        }
        return freeRegionsIndices;
    }

    public void GrantRegionToWinnerByMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(new Ray(mouseWorldPos, Vector3.forward), out hit))
            {
                Region region = hit.collider.gameObject.GetComponent<Region>();

                if (region)
                {
                    playSound.SoundPlay("region_claim");

                    int regionId = GetRegionId(region);
                    pv.RPC("RPC_RegionWasChosen", RpcTarget.All, regionId, winner.colorId, "GrantRegionToWinnerByMouseClick()");
                    wasRpcSent = true;

                    //for (int k = 0; k < regionIndexes.Count; k++) {
                    //    if (regionSystem.regionSerds[regionIndexes[k]].region == region) {
                    //        pv.RPC("RPC_RegionWasChosen", RpcTarget.All, regionIndexes[k], winner.id, "GrantRegionToWinnerByMouseClick()");
                    //        wasRpcSent = true;
                    //    }
                    //}
                }
            }
        }
    }

    private int GetRegionId(Region region)
    {
        for (int i = 0; i < regionSystem.regionSerds.Count; i++)
        {
            if (regionSystem.regionSerds[i].region == region)
                return i;
        }
        return -1;
    }

    private void GrantPlayersStartingRegions() {
        if (PhotonNetwork.IsMasterClient) {
            Dictionary<int, int> playerIdxToRegIdx = new Dictionary<int, int>();
            List<int> regs = new List<int>(Enumerable.Range(0, regionSystem.regionSerds.Count));

            for (int i = 0; i < playersManager.players.count; i++) {
                int playerIdx = playersManager.players.get(i).id;
                int randRegIdx = random.Next(0, regs.Count);

                playerIdxToRegIdx.Add(playerIdx, regs[randRegIdx]);
                regs.RemoveAt(randRegIdx);
            }

            pv.RPC("RPC_RegionDistribution", RpcTarget.All, playerIdxToRegIdx);
        }
    }

    /*[PunRPC]
    public void RPC_GetScoresForRegion(int playerId)
    {
        Player player = null;
        for(int x = 0; x < playersManager.players.count; x++)
            if(playerId == playersManager.players.get(x).id)
            {
                player = playersManager.players.get(x);
                break;
            }
        player.scores += countScoresForRegion;
        scoreTableManager.UpdateTable();
    }*/

    public void PreparationStart()
    {
        onceAddSteps = false;
        preparationToast = new BoolToastMessage("Подготовка к следующему вопросу");
        toast.showText(preparationToast);
        preparationTimer = 0;
        preparationStateIsEnded.state = false;
    }

    public void PreparationUpdate()
    {
        preparationTimer += Time.deltaTime;

        preparationToast.message = $"Подготовка к следующему вопросу: {(int)(preparationTime - preparationTimer)}";

        if (preparationTimer >= preparationTime * 0.9)
            preparationToast.isDone = true;

        if (preparationTimer >= preparationTime)
        {
            preparationStateIsEnded.state = true;
            currentQuestion = (currentQuestion + 1) % questionSession.questionLoader.questions.Count();
        }
    }

    //public void stageTwoAnnouncementStart()
    //{
    //    stageTwoAnnoucment.SetActive(true);
    //    stageTwoAnnoucment.GetComponent<CanvasGroup>().LeanAlpha(1, 0.3f).setEaseLinear();
    //    stageTwoAnnouncmentTimer = 0;
    //    stageTwoAnnouncementIsEnded.state = false;
    //}

    //public void stageTwoAnnouncementUpdate()
    //{
    //    stageTwoAnnouncmentTimer += Time.deltaTime;

    //    if (stageTwoAnnouncmentTimer >= stageTwoAnnouncmentTime)
    //    {
    //        stageTwoAnnoucment.GetComponent<CanvasGroup>().LeanAlpha(0, 0.3f).setEaseLinear();
    //        stageTwoAnnouncementIsEnded.state = true;
    //        Wait(0.5f);
    //    }
    //}

    public void SecondStageHintStart()
    {
        secondStageHint.SetActive(true);
        secondStageHint.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();
        playSound.SoundPlay("new_stage");
    }

    public void SecondStageHintUpdate()
    {
        secondStageHintTimer += Time.deltaTime;
        if (secondStageHintTimer >= secondStageHintTime)
        {
            secondStageHint.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() =>
            {
                secondStageHint.SetActive(false);
            });
            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () =>
            {
                fromSecondStageHintToOffensivePlayerSelection.Set(true);
            });
        }
    }

    public void UpdateRegionIndices() // Very unoptimized code
    {
        List<int> freeRegionsIndices = new List<int>();
        for (int i = 0; i < regionSystem.regionSerds.Count; i++)
            freeRegionsIndices.Add(i);

            List<int> claimedRegions = new List<int>();
        for (int i = 0; i < regionSystem.regionSerds.Count; i++)
        {
            for (int p = 0; p < playersManager.players.count; p++)
            {
                for (int r = 0; r < playersManager.players.get(p).claimedRegions.Count; r++)
                {
                    if (regionSystem.regionSerds[i].region == playersManager.players.get(p).claimedRegions[r])
                    {
                        claimedRegions.Add(i);
                    }
                }
            }
        }
        freeRegionsIndices.Except(claimedRegions);
        regionIndexes = freeRegionsIndices;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) 
    {
        //Player player = playersManager.players.get(otherPlayer.ActorNumber - 1);

        Player leavingPlayer = playersManager.players.get(otherPlayer.ActorNumber - 1);

        for (int i = 0; i < regionSystem.regionSerds.Count; i++)
        {
            Region region = regionSystem.regionSerds[i].region;
            if (region.hostPlayer == leavingPlayer)
                region.hostPlayer = null;
        }

        if (winner == leavingPlayer)
            winner = null;
        playersManager.disconnect(otherPlayer.ActorNumber-1);
        UpdateRegionIndices();
    }

    public void OffensivePlayerSelectionStart()
    {
        Debug.Log("OffensivePlayerSelectionStart");

        offensivePlayerSelectionTimer = 0;
        offenseAnnouncement.SetActive(true);
        offenseAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();

        offensePlayer = playersManager.players.get(currentOffensivePlayer);
        offenseAnnouncementAvatar.GetComponent<UnityEngine.UI.Image>().sprite = iconsContent.lobbyIcons[offensePlayer.iconId];
        offenseAnnouncementAvatar.GetComponent<UnityEngine.UI.Image>().color = offensePlayer.color;

        if (offensePlayer.isLocalClient)
            offenseAnnouncementText.text = "Ваша очередь нападать. Выберите, на кого хотите напасть.";
        else
            offenseAnnouncementText.text = $"<color=#{offensePlayer.color.ToHexString()}>{offensePlayer.nickname}</color> выбирает на кого напасть.";

        offenseAnnouncementTimerText.text = GlobalVariables.GetTimeStr(offensivePlayerSelectionTime);
        battle = null;
        offensivePlayerSelectionIsEnded.state = false;
    }

    public void OffensivePlayerSelectionUpdate()
    {
        int roundsCount = 3;
        double maxPlayersHealth = 100;
        bool stateEnded = false;
        offensivePlayerSelectionTimer += Time.deltaTime;
        offenseAnnouncementTimerText.text = GlobalVariables.GetTimeStr(offensivePlayerSelectionTime - offensivePlayerSelectionTimer);

        if (offensePlayer.isLocalClient)
            StartBattleByMouseClick(roundsCount, maxPlayersHealth);

        if (battle != null) {
            stateEnded = true;
        }

        if (offensivePlayerSelectionTimer >= offensivePlayerSelectionTime)
        {
            StartBattleWithRandomPlayer(roundsCount, maxPlayersHealth);
            stateEnded = true;
        }
        
        if (stateEnded)
        {
            if (offensePlayer.isLocalClient) {
                int regionId = 0;

                for (int i = 0; i < regionSystem.regionSerds.Count; i++) {
                    if (regionSystem.regionSerds[i].region == battle.region) {
                        regionId = i;
                        break;
                    };
                }

                pv.RPC("RPC_AttackAnnouncementStart", RpcTarget.All, battle.opponents[0].player.id, battle.opponents[1].player.id, regionId, battle.questions.Count, battle.opponents[0].maxHealh);
            }

            offenseAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() => 
            { 
                offenseAnnouncement.SetActive(false); 
            });
            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () => 
            {
                offensivePlayerSelectionIsEnded.state = true;
            });
            //Wait(0.5f);
        }
    }

    public void AttackAnnouncementStart()
    {
        attackAnnouncement.SetActive(true);
        attackAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();
        attackAnnouncementText.text = $"<color=#{battle.opponents[0].player.color.ToHexString()}>{battle.opponents[0].player.nickname}</color> напал на " +
                                      $"<color=#{battle.opponents[1].player.color.ToHexString()}>{battle.opponents[1].player.nickname}</color>";
        attackAnnouncementTimer = 0;
        attackAnnouncementIsEnded.state = false;
    }

    [PunRPC]
    public void RPC_AttackAnnouncementStart(int firstPlayerId, int secondPlayerId, int regionId, int roundsCount, double playersMaxHealth) {
        Player firstPlayer = null, secondPlayer = null;
        Region region = regionSystem.regionSerds[regionId].region;
        
        for (int i = 0; i < playersManager.players.count; i++) {
            if (playersManager.players.get(i).id == firstPlayerId) {
                firstPlayer = playersManager.players.get(i);
            }

            if (playersManager.players.get(i).id == secondPlayerId) {
                secondPlayer = playersManager.players.get(i);
            }
        }

        battle = StartBattle(firstPlayer, secondPlayer, region, roundsCount, playersMaxHealth);

        //AttackAnnouncementStart();
    }

    public void AttackAnnouncementUpdate()
    {
        attackAnnouncementTimer += Time.deltaTime;

        if (attackAnnouncementTimer >= attackAnnouncementTime)
        {
            attackAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() => 
            { 
                attackAnnouncement.SetActive(false); 
            });
            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () =>
            {
                attackAnnouncementIsEnded.state = true;
            });          
            //Wait(0.5f);
        }
    }

    public void OpponentsAnnouncementStart()
    {
        OpponentsAnnouncementTimer = 0;
        opponentsAnnouncement.SetActive(true);
        opponentsAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();

        Player player1 = battle.opponents[0].player;
        Player player2 = battle.opponents[1].player;


        opponentsAnnouncementOpponent1Icon.sprite = iconsContent.lobbyIcons[player1.iconId];
        opponentsAnnouncementOpponent1Icon.color = player1.color;

        opponentsAnnouncementOpponent1Nickname.text = player1.nickname;
        opponentsAnnouncementOpponent1Nickname.color = player1.color;


        opponentsAnnouncementOpponent2Icon.sprite = iconsContent.lobbyIcons[player2.iconId];
        opponentsAnnouncementOpponent2Icon.color = player2.color;

        opponentsAnnouncementOpponent2Nickname.text = player2.nickname;
        opponentsAnnouncementOpponent2Nickname.color = player2.color;
        
        opponentsAnnouncementIsEnded.state = false;
    }

    public void OpponentsAnnouncementUpdate()
    {
        OpponentsAnnouncementTimer += Time.deltaTime;

        if (OpponentsAnnouncementTimer >= OpponentsAnnouncementTime)
        {
            opponentsAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() => 
            { 
                opponentsAnnouncement.SetActive(false); 
            });
            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () =>
            {
                opponentsAnnouncementIsEnded.state = true;
            });
        }
    }

    public void QuestionNumberAnnouncementStart()
    {
        questionNumberAnnouncementTimer = 0;
        questionNumberAnnouncement.SetActive(true);
        questionNumberAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();
        questionNumberAnnouncementText.text = $"Вопрос {battle.currentQuestion + 1}";
        questionNumberAnnouncementIsEnded.state = false;
    }

    public void QuestionNumberAnnouncementUpdate()
    {
        questionNumberAnnouncementTimer += Time.deltaTime;

        if (questionNumberAnnouncementTimer >= questionNumberAnnouncementTime)
        {     
            questionNumberAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() => 
            {
                questionNumberAnnouncement.SetActive(false);
            });
            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () =>
            {
                questionNumberAnnouncementIsEnded.state = true;
            });
        }
    }

    public void AskQuestionInBattleStart()
    {
        askQuestionInBattle.SetActive(true);

        askQuestionInBattle.GetComponent<AskQuestionInBattle>().Init(battle.opponents[0], battle.opponents[1], battle.questions[battle.currentQuestion]);
        askQuestionInBattle.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();

        askQuestionInBattleIsEnded.state = false;
    }

    public void AskQuestionInBattleUpdate()
    {
        AskQuestionInBattle askQuestionInBattleComponent = askQuestionInBattle.GetComponent<AskQuestionInBattle>();
        askQuestionInBattleComponent.timer += Time.deltaTime;
        askQuestionInBattleComponent.timerText.text = GlobalVariables.GetTimeStr(askQuestionInBattleTime - askQuestionInBattleComponent.timer);

        if (askQuestionInBattleComponent.timer >= askQuestionInBattleTime)
        {    
            askQuestionInBattleIsEnded.state = true;
        }
    }

    public void CorrectAnsewerRevealingInBattleStart()
    {
        AskQuestionInBattle askQuestionInBattleComponent = askQuestionInBattle.GetComponent<AskQuestionInBattle>();
        askQuestionInBattleComponent.ShowCorrectAnswer();

        correctAnsewerRevealingInBattleIsEnded.state = false;
        correctAnsewerRevealingTimer = 0;
    }

    public void CorrectAnsewerRevealingInBattleUpdate()
    {
        correctAnsewerRevealingTimer += Time.deltaTime;

        if (correctAnsewerRevealingTimer >= correctAnsewerRevealingTime)
        {
            askQuestionInBattle.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() =>
            {
                askQuestionInBattle.SetActive(false);
            });         
            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () =>
            {
                correctAnsewerRevealingInBattleIsEnded.state = true;
            });
        }
    }

    public void BattleRoundResultsStart()
    {
        for(int x = 0; x < battle.opponents.Length; x++) {
            for(int y = 0; y < battle.opponents[x].playerAnswerData.Count; y++) {
                if(battle.opponents[x].playerAnswerData[y].timeToAnswer < fastedWinner.timeToAnswer && battle.opponents[x].playerAnswerData[y].answerId >= 0) {
                    fastedWinner.player = battle.opponents[x].player;
                    fastedWinner.answer = battle.questions[battle.currentQuestion].answer[battle.opponents[x].playerAnswerData[y].answerId];
                    fastedWinner.timeToAnswer = battle.opponents[x].playerAnswerData[y].timeToAnswer;
                }
            }
        }

        damageDealingDelayTimer = 0;
        BattleRoundResults roundResults = battleRoundResults.GetComponent<BattleRoundResults>();

        roundResults.Init(battle);
        battleRoundResults.SetActive(true);
        battleRoundResults.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();
        
        roundResults.notification.GetComponent<CanvasGroup>().alpha = 0;
        roundResults.notification.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine().setDelay((float)battleRoundResultsNotificationDelay);
        roundIsEnded.state = false;

        battle.currentQuestion++;
    }

    public void BattleRoundResultsUpdate()
    {
        BattleRoundResultsTimer += Time.deltaTime;
        damageDealingDelayTimer += Time.deltaTime;

        if (damageDealingDelayTimer >= damageDealingDelay + battleRoundResultsNotificationDelay)
        {

            if (battleRoundResults.GetComponent<BattleRoundResults>().SomeoneAnsweredCorrectly())
            {
                playSound.SoundPlay("damage");
                battleRoundResults.GetComponent<BattleRoundResults>().InflictDamageOnLoser();
            }
            battleRoundResults.GetComponent<BattleRoundResults>().UpdateOpponentsHealthGradudally(opponentsHealthUpdatingTime);
            battleRoundResults.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setDelay(battleRoundResultsDisappearingTime).
                setOnComplete(() => 
            {
                battleRoundResults.SetActive(false);
            });

            GlobalVariables.Delay(battleRoundResultsDisappearingTime + menusTransitionDelayTime, () =>
            {
                bool runOutOfQuestions = battle.currentQuestion >= battle.questions.Count;
                bool someoneDied = battle.GetDeadCount() > 0;
                if (runOutOfQuestions || someoneDied)
                    battleCond.Set(true);
                else
                    roundIsEnded.Set(true);
            });

            
            damageDealingDelayTimer = double.NaN;
        }
    }

    public void BattleResultsStart()
    {
        if (battle.GetDeadCount() > 0)
        {
            battleResultsVictory.SetActive(true);
            battleResultsVictory.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();
            Player winner = battle.GetWinner().player;
            Player defender = battle.GetDefender().player;
            if (winner == defender)
                battleResultsVictory.GetComponent<BattleResultsVictory>().Init(winner, battle.GetLoser().player, "defense");
            else
                battleResultsVictory.GetComponent<BattleResultsVictory>().Init(winner, battle.GetLoser().player, "offence");
        }
        else
        {
            battleResultsDraw.SetActive(true);
            battleResultsDraw.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();
        }
        closeBattleResultsTimer = 0;
        battleResultsTimer = 0;
    }

    public void BattleResultsUpdate()
    {
        battleResultsTimer += Time.deltaTime;
        closeBattleResultsTimer += Time.deltaTime;

        if (closeBattleResultsTimer >= closeBattleResultsTime)
        {

            if (battleResultsVictory.activeSelf)
            {
                battleResultsVictory.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() =>
                {
                    battleResultsVictory.SetActive(false);
                    if (battle.GetLoser().player.claimedRegions[0] == battle.region)
                        Debug.Log("They are identical");
                    battle.GetLoser().player.LoseRegion(battle.region);
                    //battle.GetWinner().player.ClaimRegion(battle.region);
                    GiveRegion(battle.GetWinner().player, battle.region);
                });
            } 
            else
            {
                battleResultsDraw.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() =>
                {
                    battleResultsDraw.SetActive(false);
                });
            }
            closeBattleResultsTimer = double.NaN;
        }


        if (battleResultsTimer >= battleResultsTime)
        {
            GlobalVariables.Delay(menusTransitionDelayTime, () =>
            {
                steps++;
                do
                {
                    currentOffensivePlayer = (currentOffensivePlayer + 1) % playersManager.players.count;
                } while (playersManager.players.get(currentOffensivePlayer).isLose);
                //Debug.Log("Количесто оставшихся территорий у проигравшено = " + battle.GetLoser().player.claimedRegions.Count);
                
                if (battle != null && !battle.IsDraw()) {
                    if (battle.GetLoser().player.claimedRegions.Count == 0)
                        fromBattleResultsToLosePlayer.Set(true);
                    else if (steps >= maxSteps)
                        fromBattleResultsToEndGame.Set(true);
                    else
                        fromBattleResultsToOffensive.Set(true);

                    scoreTableManager.UpdateTable();
                } else {
                    // а так правильно?
                    if (steps >= maxSteps)
                        fromBattleResultsToEndGame.Set(true);
                    else
                        fromBattleResultsToOffensive.Set(true);
                }
            });         
        }
    }

    public WinnerPerson GetFastedWinner()
    {
        TilesWordInfo tilesWordInfo = new TilesWordInfo();
        for(int x = 0; x < fastedWinner.answer.Length; x++)
            tilesWordInfo.tiles.Add(new LetterWithPoints(fastedWinner.answer[x], 0));

        WinnerPerson winner = new WinnerPerson();
        winner.person = fastedWinner.player;
        winner.word = tilesWordInfo;

        return winner;
    }

    public WinnerPerson GetWinnerWithTheMostTerritories()
    {
        Player winnerPlayer = null;
        int maxClaimedRegions = -1;
        for(int x = 0; x < playersManager.players.count; x++)
            if(playersManager.players.get(x).claimedRegions.Count > maxClaimedRegions)
            {
                maxClaimedRegions = playersManager.players.get(x).claimedRegions.Count;
                winnerPlayer = playersManager.players.get(x);
            }

        WinnerPerson resultWinner = new WinnerPerson();
        resultWinner.person = winnerPlayer;
        resultWinner.word = new TilesWordInfo();

        return resultWinner;
    }

    public void InitEndGameMenu()
    {
        List<Player> players = new List<Player>();
        for (int x = 0; x < playersManager.players.count; x++)
            players.Add(playersManager.players.get(x));

        EndMenuManager endMenuManager = endGameMenu.GetComponent<EndMenuManager>();
        endMenuManager.SetEndMenuData(players, GetFastedWinner(), GetWinnerWithTheMostTerritories());
    }

    public void EndGameStart()
    {
        endGameAnnouncment.SetActive(true);
        endGameAnnouncment.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime);
        endGameAnnouncment.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setDelay((float)(menusTransitionTime + endGameAnnouncmentTime)).setOnComplete(() => {
            endGameAnnouncment.SetActive(false);
            loadingScreen.SetActive(true);
        });

        loadingScreen.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setDelay((float)(menusTransitionTime * 2 + endGameAnnouncmentTime)).setOnComplete(() =>
        {
            endGameMenu.SetActive(true);
            InitEndGameMenu();
            uiInterface.SetActive(false);
        });
        loadingScreen.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setDelay((float)(menusTransitionTime * 2 + endGameAnnouncmentTime + endGameLoadingScreenTime))
            .setOnComplete(() => { 
                loadingScreen.SetActive(false);

            });
    }

    public void LosePlayerStart()
    {
        battle.GetLoser().player.isLose = true;
        scoreTableManager.UpdateRowsOrder();
        losePlayerAnnouncement.SetActive(true);
        losePlayerAnnouncementText.text = "Игрок " + "<color=#" + battle.GetLoser().player.color.ToHexString().Substring(0, 6) + ">" + battle.GetLoser().player.nickname + "</color> потерял все территории";
        losePlayerAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime);
        GlobalVariables.Delay(menusTransitionTime + losePlayerAnnouncmentTime / 2.0, () => { scoreTableManager.FindRowByPlayer(battle.GetLoser().player).GetComponent<ScoreTableRow>().isLose = true; });
        losePlayerAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime)
            .setDelay((float)(menusTransitionTime + losePlayerAnnouncmentTime))
            .setOnComplete(() => { 
                losePlayerAnnouncement.SetActive(false);
                if (steps >= maxSteps || battle.GetWinner().player.claimedRegions.Count >= regionSystem.regionSerds.Count)
                    fromLosePlayerToEndGame.Set(true);
                else
                    fromLosePlayerToOffensive.Set(true);
            });
    }

    public void LosePlayerUpdate()
    {
        
    }

    public void EndGameUpdate()
    {

    }

    public void Awake()
    {
        playersManager = GetComponent<PlayersManager>();
        scoreTableManager = GameObject.FindGameObjectWithTag("SCORE_TABLE_TAG").GetComponent<ScoreTableManager>();
        colorsHolderInstance = GameObject.FindGameObjectWithTag("COLOR_CONTENT_TAG").GetComponent<ColorsHolder>();
        iconsContent = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
        pv = GetComponent<PhotonView>();

        random = new System.Random();
        regionIndexes.AddRange(Enumerable.Range(0, regionSystem.regionSerds.Count));
          
        int idx = 0;
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            playersManager.connected(new Player(player.ActorNumber - 1,
                                                (int)player.CustomProperties["playerIconId"],
                                                (int)player.CustomProperties["playerColorIndex"],
                                                colorsHolderInstance.colors[(int)player.CustomProperties["playerColorIndex"]],
                                                player.NickName,
                                                player.IsLocal));
            idx++;

        }
    }

    [PunRPC]
    public void RPC_InitQuestionSession(int questionsSeed)
    {
        questionSession.InitQuestionRandom(questionsSeed);
    }


    public void Start()
    {
        //if (PhotonNetwork.IsMasterClient)
        //    pv.RPC("RPC_InitQuestionSession", RpcTarget.All, (int)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());

        questionSession = new QuestionSession(playersManager);
        questionSession.UpdateCountPlayerAnswerData();

        SetStepsText(steps, maxSteps);

        fastedWinner = new FastedWinner();
        fastedWinner.player = playersManager.players.get(0);
        fastedWinner.answer = "";
        fastedWinner.timeToAnswer = float.MaxValue;

        State gameBegginingState = new State();
        gameBegginingState.startEvents += GameBegginingStart;
        gameBegginingState.updateEvents += GameBegginingUpdate;
        gameStateMachine.states.Add(gameBegginingState);

        State firstStageHintState = new State();
        firstStageHintState.startEvents += FirstStageHintStart;
        firstStageHintState.updateEvents += FirstStageHintUpdate;
        gameStateMachine.states.Add(firstStageHintState);

        fromGameBegginingToFirstStageHint = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromGameBegginingToFirstStageHint, gameBegginingState, firstStageHintState, gameStateMachine));

        State askQuestionState = new State(); // 0
        askQuestionState.startEvents += AskQuestionStart;
        askQuestionState.updateEvents += AskQuestionUpdate;
        gameStateMachine.states.Add(askQuestionState);


        State rightAnswerState = new State();
        rightAnswerState.startEvents += RightAnswerStart;
        rightAnswerState.updateEvents += RightAnswerUpdate;
        gameStateMachine.states.Add(rightAnswerState);


        State showResultsInAskQuestionState = new State();
        showResultsInAskQuestionState.startEvents += ShowResultsInAskQuestionStart;
        showResultsInAskQuestionState.updateEvents += ShowResultsInAskQuestionUpdate;
        gameStateMachine.states.Add(showResultsInAskQuestionState);

        fromAskQuestionToRightAnswer = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromAskQuestionToRightAnswer, askQuestionState, rightAnswerState, gameStateMachine));

        fromRightAnswerToShowResultsInAskQuestion = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromRightAnswerToShowResultsInAskQuestion, rightAnswerState, showResultsInAskQuestionState, gameStateMachine));

        //fromFirstStageHintToAskQuestion

        fromFirstStageHintToAskQuestion = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromFirstStageHintToAskQuestion, firstStageHintState, askQuestionState, gameStateMachine));

        //fromGameBegginingToAskQuestion = new BoolCondition();
        //gameStateMachine.AddTransition(new Transition(fromGameBegginingToAskQuestion, gameBegginingState, askQuestionState, gameStateMachine));

        //State viewResultsState = new State(); // 1
        //viewResultsState.startEvents += ViewResultsStart;
        //viewResultsState.updateEvents += ViewResultsUpdate;
        //gameStateMachine.states.Add(viewResultsState);

        //askQuestionStateIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        //gameStateMachine.AddTransition(new Transition(askQuestionStateIsEnded, askQuestionState, viewResultsState, gameStateMachine));

        State regionSelectionState = new State(); // 2
        regionSelectionState.startEvents += RegionSelectionStart;
        regionSelectionState.updateEvents += RegionSelectionUpdate;
        gameStateMachine.states.Add(regionSelectionState);

        fromShowResultsInAskQuestionToRegionSelection = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromShowResultsInAskQuestionToRegionSelection, showResultsInAskQuestionState, regionSelectionState, gameStateMachine));


        //viewResultsStateIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        //gameStateMachine.AddTransition(new Transition(viewResultsStateIsEnded, viewResultsState, regionSelectionState, gameStateMachine));

        State preparationState = new State(); // 3
        preparationState.startEvents += PreparationStart;
        preparationState.updateEvents += PreparationUpdate;
        gameStateMachine.states.Add(preparationState);

        regionSelectionStateIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(regionSelectionStateIsEnded, regionSelectionState, preparationState, gameStateMachine));

        preparationStateIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(preparationStateIsEnded, preparationState, askQuestionState, gameStateMachine));

        //State stageTwoAnnouncementState = new State(); // 4
        //stageTwoAnnouncementState.startEvents += stageTwoAnnouncementStart;
        //stageTwoAnnouncementState.updateEvents += stageTwoAnnouncementUpdate;
        //gameStateMachine.states.Add(stageTwoAnnouncementState);

        StageTwoAnnouncementState stageTwoAnnouncementState = new StageTwoAnnouncementState(this, stageTwoAnnoucment,
                                                                        stageTwoAnnouncmentTimer,
                                                                        stageTwoAnnouncmentTime,
                                                                        menusTransitionTime,
                                                                        menusTransitionDelayTime);
        gameStateMachine.states.Add(stageTwoAnnouncementState);

        firstStageIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(firstStageIsEnded, regionSelectionState, stageTwoAnnouncementState, gameStateMachine));

        State secondStageHintState = new State(); // 5
        secondStageHintState.startEvents += SecondStageHintStart;
        secondStageHintState.updateEvents += SecondStageHintUpdate;
        gameStateMachine.states.Add(secondStageHintState);

        //fromStageTwoAnnouncementToSecondStageHint = new BoolCondition();
        gameStateMachine.AddTransition(new Transition(stageTwoAnnouncementState.fromStageTwoAnnouncementToSecondStageHint, stageTwoAnnouncementState, secondStageHintState, gameStateMachine));


        
        State offensivePlayerSelectionState = new State(); // 5
        offensivePlayerSelectionState.startEvents += OffensivePlayerSelectionStart;
        offensivePlayerSelectionState.updateEvents += OffensivePlayerSelectionUpdate;
        gameStateMachine.states.Add(offensivePlayerSelectionState);

        fromSecondStageHintToOffensivePlayerSelection = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromSecondStageHintToOffensivePlayerSelection, secondStageHintState, offensivePlayerSelectionState, gameStateMachine));

        //stageTwoAnnouncementIsEnded = new BoolCondition();
        gameStateMachine.AddTransition(new Transition(stageTwoAnnouncementState.offensivePlayerSelectionCond, 
                                                        stageTwoAnnouncementState, 
                                                        offensivePlayerSelectionState, 
                                                        gameStateMachine));

        State attackAnnouncementState = new State(); // 6
        attackAnnouncementState.startEvents += AttackAnnouncementStart;
        attackAnnouncementState.updateEvents += AttackAnnouncementUpdate;
        gameStateMachine.states.Add(attackAnnouncementState);

        offensivePlayerSelectionIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(offensivePlayerSelectionIsEnded, offensivePlayerSelectionState, attackAnnouncementState, gameStateMachine));

        State opponentsAnnouncementState = new State(); // 7
        opponentsAnnouncementState.startEvents += OpponentsAnnouncementStart;
        opponentsAnnouncementState.updateEvents += OpponentsAnnouncementUpdate;
        gameStateMachine.states.Add(opponentsAnnouncementState);

        attackAnnouncementIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(attackAnnouncementIsEnded, attackAnnouncementState, opponentsAnnouncementState, gameStateMachine));

        State questionNumberAnnouncementState = new State(); // 8
        questionNumberAnnouncementState.startEvents += QuestionNumberAnnouncementStart;
        questionNumberAnnouncementState.updateEvents += QuestionNumberAnnouncementUpdate;
        gameStateMachine.states.Add(questionNumberAnnouncementState);

        opponentsAnnouncementIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(opponentsAnnouncementIsEnded, opponentsAnnouncementState, questionNumberAnnouncementState, gameStateMachine));

        State askQuestionInBattleState = new State(); // 9
        askQuestionInBattleState.startEvents += AskQuestionInBattleStart;
        askQuestionInBattleState.updateEvents += AskQuestionInBattleUpdate;
        gameStateMachine.states.Add(askQuestionInBattleState);

        questionNumberAnnouncementIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(questionNumberAnnouncementIsEnded, questionNumberAnnouncementState, askQuestionInBattleState, gameStateMachine));

        State correctAnsewerRevealingInBattleState = new State(); // 10
        correctAnsewerRevealingInBattleState.startEvents += CorrectAnsewerRevealingInBattleStart;
        correctAnsewerRevealingInBattleState.updateEvents += CorrectAnsewerRevealingInBattleUpdate;
        gameStateMachine.states.Add(correctAnsewerRevealingInBattleState);

        askQuestionInBattleIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(askQuestionInBattleIsEnded, askQuestionInBattleState, correctAnsewerRevealingInBattleState, gameStateMachine));

        State battleRoundResultsState = new State(); // 11
        battleRoundResultsState.startEvents += BattleRoundResultsStart;
        battleRoundResultsState.updateEvents += BattleRoundResultsUpdate;
        gameStateMachine.states.Add(battleRoundResultsState);

        correctAnsewerRevealingInBattleIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(correctAnsewerRevealingInBattleIsEnded, 
                                                        correctAnsewerRevealingInBattleState, 
                                                        battleRoundResultsState, 
                                                        gameStateMachine));

        roundIsEnded = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(roundIsEnded, battleRoundResultsState, questionNumberAnnouncementState, gameStateMachine));

        State battleResultsState = new State();
        battleResultsState.startEvents += BattleResultsStart;
        battleResultsState.updateEvents += BattleResultsUpdate;
        gameStateMachine.states.Add(battleResultsState);

        State endGameState = new State();
        endGameState.startEvents += EndGameStart;
        endGameState.updateEvents += EndGameUpdate;
        gameStateMachine.states.Add(endGameState);

        State losePlayerState = new State();
        losePlayerState.startEvents += LosePlayerStart;
        losePlayerState.updateEvents += LosePlayerUpdate;
        gameStateMachine.states.Add(losePlayerState);

        battleCond = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(battleCond, battleRoundResultsState, battleResultsState, gameStateMachine));

        fromBattleResultsToOffensive = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromBattleResultsToOffensive, battleResultsState, offensivePlayerSelectionState, gameStateMachine));

        fromBattleResultsToEndGame = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromBattleResultsToEndGame, battleResultsState, endGameState, gameStateMachine));

        fromBattleResultsToLosePlayer = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromBattleResultsToLosePlayer, battleResultsState, losePlayerState, gameStateMachine));

        fromLosePlayerToEndGame = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromLosePlayerToEndGame, losePlayerState, endGameState, gameStateMachine));

        fromLosePlayerToOffensive = new SynchronizedBoolCondition(playersManager, pv, true);
        gameStateMachine.AddTransition(new Transition(fromLosePlayerToOffensive, losePlayerState, offensivePlayerSelectionState, gameStateMachine));

        GrantPlayersStartingRegions();

        gameStateMachine.Start(0); //Стадия 1
        //gameStateMachine.Start(4); //Стадия 2
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
        //SetStepsText(steps, maxSteps);
        UpdateRegions();
        //UpdateRegionColors();
    }

    private void UpdateRegions()
    {
        for (int i = 0; i < playersManager.players.count; i++)
        {
            for (int j = 0; j < playersManager.players.get(i).claimedRegions.Count; j++)
            {
                Player player = playersManager.players.get(i);
                player.claimedRegions[j].hostPlayer = player;
                player.claimedRegions[j].SetColor(player.color);
            }
        }

        for (int i = 0; i < regionSystem.regionSerds.Count; i++)
        {
            Region region = regionSystem.regionSerds[i].region;
            if (region.hostPlayer == null)
            {
                region.SetColor(unclaimedRegionColor);
            }             
        }
    }

    private void UpdateRegionColors()
    {
        for (int i = 0; i < regionSystem.regionSerds.Count; i++)
        {
            regionSystem.regionSerds[i].region.SetColor(unclaimedRegionColor);
        }


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

        nextQuestionTimeText.text = $"{minutesNonsignificantZero + minutes}:{secondsNonsignificantZero + restOfSeconds}";
    }



    public void Wait(double seconds)
    {
        waiteTime = seconds;
        waitTimer = 0;
    }

    public void SetStepsText(int steps, int maxSteps)
    {
        stepsText.text = $"{steps}/{maxSteps}";
    }

    public int GetFreeRegionsCount()
    {
        /* int claimedRegions = 0;
         for (int p = 0; p < playersManager.players.count; p++) {
             claimedRegions += playersManager.players.get(p).claimedRegions.Count;
         }
         int freeRegions = regionSystem.regionSerds.Count - claimedRegions;*/

        int freeRegionsCount = 0;
        for (int i = 0; i < regionSystem.regionSerds.Count; i++)
            if (regionSystem.regionSerds[i].region.hostPlayer == null)
                freeRegionsCount++;
        return freeRegionsCount;
        //return regionIndexes.Count;
    }

    public Battle StartBattle(Player player1, Player player2, Region region, int roundsCount, double playersMaxHealth)
    {
        Opponent opponent1 = new Opponent(player1, playersMaxHealth, playersMaxHealth, 0);
        Opponent opponent2 = new Opponent(player2, playersMaxHealth, playersMaxHealth, 0);
        Battle newBattle = new Battle(opponent1, opponent2, region);

        if (roundsCount > questionSession.questionLoader.questions.Count())
            roundsCount = questionSession.questionLoader.questions.Count();

        int idsCount = questionSession.questionLoader.questions.Count();
        int[] ids = new int[idsCount];
        for (int i = 0; i < idsCount; i++)
            ids[i] = i;

        System.Random rnd = new System.Random();
        for (int i = 0; i < roundsCount; i++)
        {
            int randInt = rnd.Next(0, idsCount - 1);
            QuestionManager.Question randQuestion = questionSession.questionLoader.questions[ids[randInt]];

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
                        battle = StartBattle(offensePlayer, regionHost, region, roundsCount, playersMaxHealth);
                    }

                }
            }
        }
    }

    //public void StartBattleWithRandomPlayer(int roundsCount, double playersMaxHealth)
    //{
    //    int player1Id = -1;
    //    for (int i = 0; i < playersManager.players.count; i++)
    //    {
    //        if (offensePlayer == playersManager.players.get(i))
    //        {
    //            player1Id = i;
    //            break;
    //        }
    //    }

    //    int idsCount = playersManager.players.count;
    //    int[] ids = new int[idsCount];
    //    for (int i = 0; i < idsCount; i++)
    //        ids[i] = i;

    //    (ids[player1Id], ids[idsCount - 1]) = (ids[idsCount - 1], ids[player1Id]);
    //    idsCount--;

    //    System.Random rnd = new System.Random();

    //    int randomPlayerId = rnd.Next(0, idsCount);
    //    Player randomPlayer = playersManager.players.get(ids[randomPlayerId]);

    //    int randomRegionId = rnd.Next(0, randomPlayer.claimedRegions.Count - 1);
    //    Region randomPlayerRegion = randomPlayer.claimedRegions[randomRegionId];

    //    battle = StartBattle(offensePlayer, randomPlayer, randomPlayerRegion, roundsCount, playersMaxHealth);
    //}


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

        
        Player randomPlayer = null;
        while (idsCount > 0)
        {
            int randomPlayerId = rnd.Next(0, idsCount);
            randomPlayer = playersManager.players.get(ids[randomPlayerId]);
            if (randomPlayer.claimedRegions.Count > 0)
                break;
            else
            {
                (ids[randomPlayerId], ids[idsCount - 1]) = (ids[idsCount - 1], ids[randomPlayerId]);
                idsCount--;
            }
        }

        int randomRegionId = rnd.Next(0, randomPlayer.claimedRegions.Count - 1);
        Region randomPlayerRegion = randomPlayer.claimedRegions[randomRegionId];

        battle = StartBattle(offensePlayer, randomPlayer, randomPlayerRegion, roundsCount, playersMaxHealth);
    }
}
