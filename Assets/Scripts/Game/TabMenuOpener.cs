using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabMenuOpener : MonoBehaviour
{

    public Animator animator;
    private float timer;
    public float time = 0.1f;
    public void OpenMenu()
    {
        if(timer <= 0)
            animator.SetBool("Open", true);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (animator.GetBool("Open"))
            if (Input.GetMouseButton(0))
            {
                animator.SetBool("Open", false);
                timer = time;
            }
        if(timer > 0)
            timer -= Time.deltaTime;
    }
}
