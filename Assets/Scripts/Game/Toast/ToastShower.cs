using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToastMessage
{
    public string message;
    abstract public bool ClosingIsNeeded();
    abstract public void UpdateToastMessage();

}

public class TemporaryToastMessage : ToastMessage
{
    public double timerToClose = 0;
    public double timeToClose = 0;
    public bool isDone = false;

    public TemporaryToastMessage(string message, double timeToClose)
    {
        this.message = message;
        this.timeToClose = timeToClose;
    }

    public override bool ClosingIsNeeded()
    {
        return isDone;
    }

    public override void UpdateToastMessage()
    {
        timerToClose += Time.deltaTime;
        if (timerToClose >= timeToClose)
            isDone = true;
    }
}

public class BoolToastMessage : ToastMessage
{
    public bool isDone = false;

    public BoolToastMessage(string message)
    {
        this.message = message;
    }

    public override bool ClosingIsNeeded()
    {
        return isDone;
    }

    public override void UpdateToastMessage()
    {
    }
}

public class ToastShower : MonoBehaviour
{
    public Animator animator;
    public TMPro.TMP_Text text;
    public float timeShow = 2;
    public float offsetTime = 0.5f;
    public float timer;

    public Queue<ToastMessage> messages;
    public ToastMessage currentMessage = null;
    
    public void showText(string text, double timeToClose = 5)
    {
        messages.Enqueue(new TemporaryToastMessage(text, timeToClose));
    }

    public void showText(BoolToastMessage boolToastMessage)
    {
        messages.Enqueue(boolToastMessage);
    }

    public void showText(TemporaryToastMessage temporaryToastMessage)
    {
        messages.Enqueue(temporaryToastMessage);
    }

    void Start()
    {
        messages = new Queue<ToastMessage>();
    }

    void Update()
    {
        if (messages.Count > 0)
        {
            if (currentMessage == null)
            {
                currentMessage = messages.Dequeue();
                animator.SetBool("Open", true);
                text.text = currentMessage.message;
            }
        }

        if (currentMessage != null)
        {
            currentMessage.UpdateToastMessage();
            text.text = currentMessage.message;
            if (currentMessage.ClosingIsNeeded())
            {
                currentMessage = null;
                animator.SetBool("Open", false);
            }
        }




        //timer -= Time.deltaTime;
        //if (timer < 0) timer = 0;

        //if(timer < offsetTime)
        //    animator.SetBool("Open", false);

        //if(timer == 0)
        //    if(message.Count != 0)
        //    {
        //        timer = timeShow;
        //        animator.SetBool("Open", true);
        //        text.text = message.Dequeue();
        //    }
    }
}
