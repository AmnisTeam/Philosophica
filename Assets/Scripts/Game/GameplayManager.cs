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
using UnityEngine.UIElements;
using Photon.Pun;

public class BoolCondition : Condition
{
    public bool state = false;

    public void Set(bool state)
    {
        this.state = state;
    }

    public override bool CheckCondition()
    {
        return state;
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

    private int winnerRegionsCountAtStartOfSelection;
    private Player winner;
    private Player offensePlayer;

    public Battle battle;

    public BoolToastMessage regionSelectionToast;
    public BoolToastMessage preparationToast;


    //public BoolCondition fromGameBegginingToAskQuestion;
    public BoolCondition fromFirstStageHintToAskQuestion;
    public BoolCondition fromGameBegginingToFirstStageHint;
    public BoolCondition askQuestionStateIsEnded;
    public BoolCondition viewResultsStateIsEnded;
    public BoolCondition regionSelectionStateIsEnded;
    public BoolCondition preparationStateIsEnded;
    public BoolCondition firstStageIsEnded;
    public BoolCondition stageTwoAnnouncementIsEnded;
    public BoolCondition fromSecondStageHintToOffensivePlayerSelection;
    public BoolCondition fromStageTwoAnnouncementToSecondStageHint;
    public BoolCondition offensivePlayerSelectionIsEnded;
    public BoolCondition attackAnnouncementIsEnded;
    public BoolCondition opponentsAnnouncementIsEnded;
    public BoolCondition questionNumberAnnouncementIsEnded;
    public BoolCondition askQuestionInBattleIsEnded;
    public BoolCondition correctAnsewerRevealingInBattleIsEnded;
    public BoolCondition roundIsEnded;
    public BoolCondition battleCond;
    public BoolCondition fromBattleResultsToOffensive;
    public BoolCondition fromBattleResultsToEndGame;
    public BoolCondition fromBattleResultsToLosePlayer;

    public void GameBegginingStart()
    {
        gameBeggining.SetActive(true);
        gameBeggining.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();

        fromGameBegginingToFirstStageHint.Set(false);
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
        firstStageHint.SetActive(true);
        firstStageHint.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();

        firstStageHintTimer = 0;
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

    public void AskQuestionStart()
    {
        questionMenu.SetActive(true);
        questionMenu.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();

        askQuestionStateIsEnded.state = false;
        
        questionManager.setQuestion(currentQuestion);

        wasRpcSent = false;
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
        if (questionManager.tableCompiler.isHaveRightAnswer)
        {
            winner = questionManager.tableCompiler.table[0];
            winnerRegionsCountAtStartOfSelection = winner.claimedRegions.Count;

            regionSelectionToast = new BoolToastMessage($"<color=#{winner.color.ToHexString()}>{winner.nickname}</color> выбирает территорию");
            toast.showText(regionSelectionToast);

            regionSelectionTimer = 0;
            regionSelectionStateIsEnded.state = false;
        }
    }

    public void RegionSelectionUpdate()
    {
        bool stateEnded = false;
        regionSelectionTimer += Time.deltaTime;

        if (questionManager.tableCompiler.isHaveRightAnswer) {
            regionSelectionToast.message = $"<color=#{winner.color.ToHexString()}>{winner.nickname}</color> выбирает территорию: {(int)(regionSelectionMaxTime - regionSelectionTimer)}";
        }

        if (questionManager.tableCompiler.isHaveRightAnswer) {
            if (winner.isLocalClient) {
                GrantRegionToWinnerByMouseClick();
            }
        }


        if (questionManager.tableCompiler.isHaveRightAnswer) {
            if (winner.claimedRegions.Count > winnerRegionsCountAtStartOfSelection) {
                stateEnded = true;
            }
        }

        if (regionSelectionTimer >= regionSelectionMaxTime || !questionManager.tableCompiler.isHaveRightAnswer) {
            if (questionManager.tableCompiler.isHaveRightAnswer) {
                GrantRandomFreeRegionToPlayer(winner);
            }
            stateEnded = true;
        }

        if (stateEnded) {
            pv.RPC("RPC_StepsUpdate", RpcTarget.All);
        }
    }
    
    [PunRPC]
    public void RPC_RegionWasChosen(int regionId, int playerId, string source) {
        //Debug.LogError($"Player {playerId} has claimed region {regionId}");

        Region region = regionSystem.regionSerds[regionId].region;
        Player player = playersManager.players.get(playerId);

        player.ClaimRegion(region);
        regionIndexes.Remove(regionId);
        //Debug.LogError($"Free regions: {string.Join(" ", regionIndexes)} ({GetFreeRegionsCount()})");

        player.scores += countScoresForRegion;
        scoreTableManager.UpdateTable();

        if (region && questionManager.tableCompiler.isHaveRightAnswer) {
            Vector2 regionCenter = Vector2.zero;
            for (int x = 0; x < region.GetComponent<MeshFilter>().mesh.vertices.Length; x++) {
                regionCenter += region.GetComponent<MeshFilter>().mesh.vertices[x].ToXY();
            }

            regionCenter /= region.GetComponent<MeshFilter>().mesh.vertices.Length;

            //pv.RPC("RPC_MoveCameraToChoosenRegion", RpcTarget.All, regionCenter.x, regionCenter.y);
            Camera.main.GetComponent<MoveCameraToActiveRegion>().SetTarget(new Vector2(regionCenter.x, regionCenter.y));
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

            player.ClaimRegion(region);
            regionIndexes.Remove(rec.Value);
            //Debug.LogError($"Free regions: {string.Join(" ", regionIndexes)} ({GetFreeRegionsCount()})");
        }
    }

    public void GrantRandomFreeRegionToPlayer(Player player) {
        if (!wasRpcSent) {
            int randRegIdx = random.Next(0, regionIndexes.Count);
            pv.RPC("RPC_RegionWasChosen", RpcTarget.All, regionIndexes[randRegIdx], player.id, "GrantRandomFreeRegionToPlayer()");
            wasRpcSent = true;
        }
    }

    public void GrantRegionToWinnerByMouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            if (!wasRpcSent) {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(new Ray(mouseWorldPos, Vector3.forward), out hit)) {
                    Region region = hit.collider.gameObject.GetComponent<Region>();

                    if (region) {
                        for (int k = 0; k < regionIndexes.Count; k++) {
                            if (regionSystem.regionSerds[regionIndexes[k]].region == region) {
                                pv.RPC("RPC_RegionWasChosen", RpcTarget.All, regionIndexes[k], winner.id, "GrantRegionToWinnerByMouseClick()");
                                wasRpcSent = true;
                            }
                        }
                    }
                }
            }
        }
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

    [PunRPC]
    public void RPC_StepsUpdate() {
        steps++;
        SetStepsText(steps, maxSteps);

        if (questionManager.tableCompiler.isHaveRightAnswer) {
            regionSelectionToast.isDone = true;
        }

        if (GetFreeRegionsCount() > 0) {
            regionSelectionStateIsEnded.state = true;
        } else {
            firstStageIsEnded.state = true;
        }
            
        Wait(0.8);
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

        preparationToast.message = $"Подготовка к следующему вопросу: {(int)(preparationTime - preparationTimer)}";

        if (preparationTimer >= preparationTime * 0.9)
            preparationToast.isDone = true;

        if (preparationTimer >= preparationTime)
        {
            preparationStateIsEnded.state = true;
            currentQuestion = (currentQuestion + 1) % questionManager.questionLoader.questions.Count();
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

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) 
    {
        playersManager.disconnect(otherPlayer.ActorNumber-1);
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
        questionNumberAnnouncementText.text = $"������ {battle.currentQuestion + 1}";
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

        battleCond.Set(false);
    }

    public void BattleRoundResultsUpdate()
    {
        BattleRoundResultsTimer += Time.deltaTime;
        damageDealingDelayTimer += Time.deltaTime;

        if (damageDealingDelayTimer >= damageDealingDelay + battleRoundResultsNotificationDelay)
        {
            if (battleRoundResults.GetComponent<BattleRoundResults>().SomeoneAnsweredCorrectly())
                battleRoundResults.GetComponent<BattleRoundResults>().InflictDamageOnLoser();
            battleRoundResults.GetComponent<BattleRoundResults>().UpdateOpponentsHealthGradudally(opponentsHealthUpdatingTime);
            battleRoundResults.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setDelay(battleRoundResultsDisappearingTime).
                setOnComplete(() => 
            {
                battleRoundResults.SetActive(false);
            });

            battle.currentQuestion++;

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
        fromBattleResultsToOffensive.Set(false);
        battleCond.Set(false);
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
                    battle.GetWinner().player.ClaimRegion(battle.region);
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
                int count = 0;
                do
                {
                    currentOffensivePlayer = (currentOffensivePlayer + 1) % playersManager.players.count;
                    count++;
                } while (!playersManager.players.get(currentOffensivePlayer).isLose && count < playersManager.players.count);
                //Debug.Log("Количесто оставшихся территорий у проигравшено = " + battle.GetLoser().player.claimedRegions.Count);
                
                if (!battle.IsDraw()) {
                    if (battle.GetLoser().player.claimedRegions.Count == 0)
                        fromBattleResultsToLosePlayer.Set(true);
                    else if (steps >= maxSteps)
                        fromBattleResultsToEndGame.Set(true);
                    else
                        fromBattleResultsToOffensive.Set(true);
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
        fromBattleResultsToEndGame.Set(false);
    }

    public void LosePlayerStart()
    {
        battle.GetLoser().player.isLose = true;
        scoreTableManager.UpdateRowsOrder();
        losePlayerAnnouncement.SetActive(true);
        losePlayerAnnouncementText.text = "Игрок" + "<color=#" + battle.GetLoser().player.color.ToHexString().Substring(0, 6) + ">" + battle.GetLoser().player.nickname + "</color> потерял все территории";
        losePlayerAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime);
        GlobalVariables.Delay(menusTransitionTime + losePlayerAnnouncmentTime / 2.0, () => { scoreTableManager.FindRowByPlayer(battle.GetLoser().player).GetComponent<ScoreTableRow>().isLose = true; });
        losePlayerAnnouncement.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime)
            .setDelay((float)(menusTransitionTime + losePlayerAnnouncmentTime))
            .setOnComplete(() => { 
                losePlayerAnnouncement.SetActive(false);
                if (steps >= maxSteps || battle.GetWinner().player.claimedRegions.Count >= regionSystem.regionSerds.Count)
                    fromBattleResultsToEndGame.Set(true);
                else
                    fromBattleResultsToOffensive.Set(true);
            });
        fromBattleResultsToLosePlayer.Set(false);
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
            playersManager.connected(new Player(idx,
                                                (int)player.CustomProperties["playerIconId"],
                                                (int)player.CustomProperties["playerColorIndex"],
                                                colorsHolderInstance.colors[(int)player.CustomProperties["playerColorIndex"]],
                                                player.NickName,
                                                player.IsLocal));
            idx++;

        }
    }

    public void Start()
    {
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

        fromGameBegginingToFirstStageHint = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(fromGameBegginingToFirstStageHint, gameBegginingState, firstStageHintState, gameStateMachine));

        State askQuestionState = new State(); // 0
        askQuestionState.startEvents += AskQuestionStart;
        askQuestionState.updateEvents += AskQuestionUpdate;
        gameStateMachine.states.Add(askQuestionState);

        //fromFirstStageHintToAskQuestion

        fromFirstStageHintToAskQuestion = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(fromFirstStageHintToAskQuestion, firstStageHintState, askQuestionState, gameStateMachine));

        //fromGameBegginingToAskQuestion = new BoolCondition();
        //gameStateMachine.transitions.Add(new Transition(fromGameBegginingToAskQuestion, gameBegginingState, askQuestionState, gameStateMachine));

        State viewResultsState = new State(); // 1
        viewResultsState.startEvents += ViewResultsStart;
        viewResultsState.updateEvents += ViewResultsUpdate;
        gameStateMachine.states.Add(viewResultsState);

        askQuestionStateIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(askQuestionStateIsEnded, askQuestionState, viewResultsState, gameStateMachine));

        State regionSelectionState = new State(); // 2
        regionSelectionState.startEvents += RegionSelectionStart;
        regionSelectionState.updateEvents += RegionSelectionUpdate;
        gameStateMachine.states.Add(regionSelectionState);

        viewResultsStateIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(viewResultsStateIsEnded, viewResultsState, regionSelectionState, gameStateMachine));

        State preparationState = new State(); // 3
        preparationState.startEvents += PreparationStart;
        preparationState.updateEvents += PreparationUpdate;
        gameStateMachine.states.Add(preparationState);

        regionSelectionStateIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(regionSelectionStateIsEnded, regionSelectionState, preparationState, gameStateMachine));

        preparationStateIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(preparationStateIsEnded, preparationState, askQuestionState, gameStateMachine));

        //State stageTwoAnnouncementState = new State(); // 4
        //stageTwoAnnouncementState.startEvents += stageTwoAnnouncementStart;
        //stageTwoAnnouncementState.updateEvents += stageTwoAnnouncementUpdate;
        //gameStateMachine.states.Add(stageTwoAnnouncementState);

        StageTwoAnnouncementState stageTwoAnnouncementState = new StageTwoAnnouncementState(stageTwoAnnoucment,
                                                                        stageTwoAnnouncmentTimer,
                                                                        stageTwoAnnouncmentTime,
                                                                        menusTransitionTime,
                                                                        menusTransitionDelayTime);
        gameStateMachine.states.Add(stageTwoAnnouncementState);

        firstStageIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(firstStageIsEnded, regionSelectionState, stageTwoAnnouncementState, gameStateMachine));

        State secondStageHintState = new State(); // 5
        secondStageHintState.startEvents += SecondStageHintStart;
        secondStageHintState.updateEvents += SecondStageHintUpdate;
        gameStateMachine.states.Add(secondStageHintState);

        //fromStageTwoAnnouncementToSecondStageHint = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(stageTwoAnnouncementState.fromStageTwoAnnouncementToSecondStageHint, stageTwoAnnouncementState, secondStageHintState, gameStateMachine));



        State offensivePlayerSelectionState = new State(); // 5
        offensivePlayerSelectionState.startEvents += OffensivePlayerSelectionStart;
        offensivePlayerSelectionState.updateEvents += OffensivePlayerSelectionUpdate;
        gameStateMachine.states.Add(offensivePlayerSelectionState);

        fromSecondStageHintToOffensivePlayerSelection = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(fromSecondStageHintToOffensivePlayerSelection, secondStageHintState, offensivePlayerSelectionState, gameStateMachine));

        //stageTwoAnnouncementIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(stageTwoAnnouncementState.offensivePlayerSelectionCond, 
                                                        stageTwoAnnouncementState, 
                                                        offensivePlayerSelectionState, 
                                                        gameStateMachine));

        State attackAnnouncementState = new State(); // 6
        attackAnnouncementState.startEvents += AttackAnnouncementStart;
        attackAnnouncementState.updateEvents += AttackAnnouncementUpdate;
        gameStateMachine.states.Add(attackAnnouncementState);

        offensivePlayerSelectionIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(offensivePlayerSelectionIsEnded, offensivePlayerSelectionState, attackAnnouncementState, gameStateMachine));

        State opponentsAnnouncementState = new State(); // 7
        opponentsAnnouncementState.startEvents += OpponentsAnnouncementStart;
        opponentsAnnouncementState.updateEvents += OpponentsAnnouncementUpdate;
        gameStateMachine.states.Add(opponentsAnnouncementState);

        attackAnnouncementIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(attackAnnouncementIsEnded, attackAnnouncementState, opponentsAnnouncementState, gameStateMachine));

        State questionNumberAnnouncementState = new State(); // 8
        questionNumberAnnouncementState.startEvents += QuestionNumberAnnouncementStart;
        questionNumberAnnouncementState.updateEvents += QuestionNumberAnnouncementUpdate;
        gameStateMachine.states.Add(questionNumberAnnouncementState);

        opponentsAnnouncementIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(opponentsAnnouncementIsEnded, opponentsAnnouncementState, questionNumberAnnouncementState, gameStateMachine));

        State askQuestionInBattleState = new State(); // 9
        askQuestionInBattleState.startEvents += AskQuestionInBattleStart;
        askQuestionInBattleState.updateEvents += AskQuestionInBattleUpdate;
        gameStateMachine.states.Add(askQuestionInBattleState);

        questionNumberAnnouncementIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(questionNumberAnnouncementIsEnded, questionNumberAnnouncementState, askQuestionInBattleState, gameStateMachine));

        State correctAnsewerRevealingInBattleState = new State(); // 10
        correctAnsewerRevealingInBattleState.startEvents += CorrectAnsewerRevealingInBattleStart;
        correctAnsewerRevealingInBattleState.updateEvents += CorrectAnsewerRevealingInBattleUpdate;
        gameStateMachine.states.Add(correctAnsewerRevealingInBattleState);

        askQuestionInBattleIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(askQuestionInBattleIsEnded, askQuestionInBattleState, correctAnsewerRevealingInBattleState, gameStateMachine));

        State battleRoundResultsState = new State(); // 11
        battleRoundResultsState.startEvents += BattleRoundResultsStart;
        battleRoundResultsState.updateEvents += BattleRoundResultsUpdate;
        gameStateMachine.states.Add(battleRoundResultsState);

        correctAnsewerRevealingInBattleIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(correctAnsewerRevealingInBattleIsEnded, 
                                                        correctAnsewerRevealingInBattleState, 
                                                        battleRoundResultsState, 
                                                        gameStateMachine));

        roundIsEnded = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(roundIsEnded, battleRoundResultsState, questionNumberAnnouncementState, gameStateMachine));

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

        battleCond = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(battleCond, battleRoundResultsState, battleResultsState, gameStateMachine));

        fromBattleResultsToOffensive = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(fromBattleResultsToOffensive, battleResultsState, offensivePlayerSelectionState, gameStateMachine));

        fromBattleResultsToEndGame = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(fromBattleResultsToEndGame, battleResultsState, endGameState, gameStateMachine));

        fromBattleResultsToLosePlayer = new BoolCondition();
        gameStateMachine.transitions.Add(new Transition(fromBattleResultsToLosePlayer, battleResultsState, losePlayerState, gameStateMachine));

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

        UpdateRegionColors();
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

        return regionIndexes.Count;
    }

    public Battle StartBattle(Player player1, Player player2, Region region, int roundsCount, double playersMaxHealth)
    {
        Opponent opponent1 = new Opponent(player1, playersMaxHealth, playersMaxHealth, 0);
        Opponent opponent2 = new Opponent(player2, playersMaxHealth, playersMaxHealth, 0);
        Battle newBattle = new Battle(opponent1, opponent2, region);

        if (roundsCount > questionManager.questionLoader.questions.Count())
            roundsCount = questionManager.questionLoader.questions.Count();

        int idsCount = questionManager.questionLoader.questions.Count();
        int[] ids = new int[idsCount];
        for (int i = 0; i < idsCount; i++)
            ids[i] = i;

        System.Random rnd = new System.Random();
        for (int i = 0; i < roundsCount; i++)
        {
            int randInt = rnd.Next(0, idsCount - 1);
            QuestionManager.Question randQuestion = questionManager.questionLoader.questions[ids[randInt]];

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
