using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTwoAnnouncementState : GameState
{
    private GameObject stageTwoAnnoucment;
    private double stageTwoAnnouncmentTimer;
    private double stageTwoAnnouncmentTime;
    public BoolCondition offensivePlayerSelectionCond;
    public BoolCondition fromStageTwoAnnouncementToSecondStageHint;
    public float menusTransitionTime;
    public float menusTransitionDelayTime;
    public GameplayManager gameplayManager;
    public StageTwoAnnouncementState(GameplayManager gameplayManager, GameObject stageTwoAnnoucment,
                                    double stageTwoAnnouncmentTimer, 
                                    double stageTwoAnnouncmentTime,
                                    float menusTransitionTime,
                                    float menusTransitionDelayTime)
    {
        this.gameplayManager = gameplayManager;
        this.stageTwoAnnoucment = stageTwoAnnoucment;
        this.stageTwoAnnouncmentTime = stageTwoAnnouncmentTime;
        this.stageTwoAnnouncmentTimer = stageTwoAnnouncmentTimer;
        this.menusTransitionTime = menusTransitionTime;
        this.menusTransitionDelayTime = menusTransitionDelayTime;
        this.gameplayManager = gameplayManager;
        offensivePlayerSelectionCond = new BoolCondition();
        fromStageTwoAnnouncementToSecondStageHint = new BoolCondition();
    }

    public override void Start()
    {
        gameplayManager.onceAddSteps = false;
        stageTwoAnnoucment.SetActive(true);
        stageTwoAnnoucment.GetComponent<CanvasGroup>().LeanAlpha(1, menusTransitionTime).setEaseOutSine();
        stageTwoAnnouncmentTimer = 0;
        offensivePlayerSelectionCond.Set(false);
        fromStageTwoAnnouncementToSecondStageHint.Set(false);
    }

    public override void Update()
    {
        stageTwoAnnouncmentTimer += Time.deltaTime;

        if (stageTwoAnnouncmentTimer >= stageTwoAnnouncmentTime)
        {
            stageTwoAnnoucment.GetComponent<CanvasGroup>().LeanAlpha(0, menusTransitionTime).setEaseOutSine().setOnComplete(() => 
            { 
                stageTwoAnnoucment.SetActive(false); 
            });

            GlobalVariables.Delay(menusTransitionTime + menusTransitionDelayTime, () =>
            {
                //offensivePlayerSelectionCond.Set(true);
                fromStageTwoAnnouncementToSecondStageHint.Set(true);
            });
        }
    }
}
