using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastShower : MonoBehaviour
{
    public Animator animator;
    public TMPro.TMP_Text text;
    public float timeShow = 2;
    public float offsetTime = 0.5f;
    public float timer;

    public Queue<string> message;

    public void showText(string text)
    {
        message.Enqueue(text);
    }

    void Start()
    {
        message = new Queue<string>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;

        if(timer < offsetTime)
            animator.SetBool("Open", false);

        if(timer == 0)
            if(message.Count != 0)
            {
                timer = timeShow;
                animator.SetBool("Open", true);
                text.text = message.Dequeue();
            }
    }
}
