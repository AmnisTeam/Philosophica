using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    //public GameplayManager gamePlayManager;
    //public EndMenuManager endMenuManager;
    //public CanvasGroup transitionScreen;
    //public GameObject gameInterface;

    //public Transform cameraPosition;
    //public float cameraSize;

    //public float timeToAppereanceTransitionScreen;

    //public WinnerPerson GetTopWinner()
    //{
    //    return null;
    //}

    //public WinnerPerson GetLongestWinner()
    //{
    //    return null;
    //}

    //public void OpenEndGameMenu()
    //{
        //transitionScreen.gameObject.SetActive(true);
        //transitionScreen.LeanAlpha(1, timeToAppereanceTransitionScreen).setOnComplete(() => { 
        //    endMenuManager.gameObject.SetActive(true);
        //    gameInterface.SetActive(false);
        //    Camera.main.transform.position = cameraPosition.position;
        //    Camera.main.orthographicSize = cameraSize;


        //    List<Player> persons = new List<Player>();
        //    for (int x = 0; x < gamePlayManager.persons.persons.Count; x++)
        //        persons.Add(gamePlayManager.personManager.persons[x]);

        //    endMenuManager.SetEndMenuData(persons, GetTopWinner(), GetLongestWinner());

        //});
        //transitionScreen.LeanAlpha(0, timeToAppereanceTransitionScreen)
        //                .setDelay(timeToAppereanceTransitionScreen)
        //                .setOnComplete(() => { transitionScreen.gameObject.SetActive(false); });
    //}

    void Start()
    {

    }

    void Update()
    {
        
    }
}
