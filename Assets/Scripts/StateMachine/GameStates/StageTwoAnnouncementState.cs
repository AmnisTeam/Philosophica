using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTwoAnnouncementState : GameState
{
    private GameObject stageTwoAnnoucment;
    private double stageTwoAnnouncmentTimer;
    private double stageTwoAnnouncmentTime;
    public BoolCondition stageTwoAnnouncementIsEnded;
    public StageTwoAnnouncementState(GameObject stageTwoAnnoucment, double stageTwoAnnouncmentTimer,
                                                                    double stageTwoAnnouncmentTime)
    {
        this.stageTwoAnnoucment = stageTwoAnnoucment;
        this.stageTwoAnnouncmentTime = stageTwoAnnouncmentTime;
        this.stageTwoAnnouncmentTimer = stageTwoAnnouncmentTimer;
        stageTwoAnnouncementIsEnded = new BoolCondition();
    }

    public override void Start()
    {
        stageTwoAnnoucment.SetActive(true);
        stageTwoAnnoucment.GetComponent<CanvasGroup>().LeanAlpha(1, 0.3f).setEaseLinear();
        stageTwoAnnouncmentTimer = 0;
        stageTwoAnnouncementIsEnded.state = false;
    }

    public override void Update()
    {
        stageTwoAnnouncmentTimer += Time.deltaTime;

        if (stageTwoAnnouncmentTimer >= stageTwoAnnouncmentTime)
        {
            stageTwoAnnoucment.GetComponent<CanvasGroup>().LeanAlpha(0, 0.3f).setEaseLinear();

            GlobalVariables.Delay(0.5, () =>
            {
                stageTwoAnnouncementIsEnded.state = true;
            });
        }
    }

    public void kek()
    {

    }



}
