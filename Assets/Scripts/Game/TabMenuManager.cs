using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenuManager : MonoBehaviour
{
    public class Worker
    {
        public bool start = true;
        public int idWork = -1;
    }
    public CanvasGroup[] line;
    public CanvasGroup[] circle;
    public CanvasGroup[] nickname;
    public CanvasGroup[] player;
    public Image[] icons;
    public TMPro.TMP_Text[] nicknameText;
    public IconsContent iconsContent;
    public PlayersManager playersManager;
    public float timer;

    public float heightTabMenuPlayer = 120;
    public float heightOffsetTabMenu = 5;

    private Worker worker;
    private int disconnectedPlayerId;

    //Параметры анимации исчезновения игрока
    public float timeDisappeare = 0.3f;
    public float timeMoveUp = 0.3f;
    public AnimationCurve positionOffset;

    public void updateTabMenu()
    {
        for (int x = 0; x < playersManager.MAX_COUNT_PLAYERS; x++)
        {
            float posY = -x * (heightTabMenuPlayer + heightOffsetTabMenu);
            line[x].gameObject.transform.localPosition = new Vector3(line[x].gameObject.transform.localPosition.x, posY, line[x].gameObject.transform.localPosition.z);
            circle[x].gameObject.transform.localPosition = new Vector3(circle[x].gameObject.transform.localPosition.x, posY, circle[x].gameObject.transform.localPosition.z);
            nickname[x].gameObject.transform.localPosition = new Vector3(nickname[x].gameObject.transform.localPosition.x, posY, nickname[x].gameObject.transform.localPosition.z);
            player[x].gameObject.transform.localPosition = new Vector3(player[x].gameObject.transform.localPosition.x, posY, player[x].gameObject.transform.localPosition.z);

            if (x < playersManager.players.count)
            {
                line[x].alpha = 1;
                circle[x].alpha = 1;
                nickname[x].alpha = 1;
                player[x].alpha = 1;
            }
            else
            {
                line[x].alpha = 0;
                circle[x].alpha = 0;
                nickname[x].alpha = 0;
                player[x].alpha = 0;
            }
        }

        for (int x = 0; x < playersManager.players.count; x++)
        {
            icons[x].sprite = iconsContent.icons[playersManager.players.get(x).iconId].sprite;
            icons[x].color = playersManager.players.get(x).color;
            nicknameText[x].text = playersManager.players.get(x).nickname;
        }
    }

    public void disconnectPlayer(int id)
    {
        worker.idWork = 0;
        disconnectedPlayerId = id;
    }

    public void smoothDisappPlayer(int idWork, Worker worker, int idPlayer)
    {
        if(idWork == worker.idWork)
        {
            if(worker.start)
            {
                timer = timeDisappeare;
                worker.start = false;
            }

            timer -= Time.deltaTime;
            if (timer < 0) timer = 0;

            line[idPlayer].alpha = timer / timeDisappeare;
            circle[idPlayer].alpha = timer / timeDisappeare;
            nickname[idPlayer].alpha = timer / timeDisappeare;
            player[idPlayer].alpha = timer / timeDisappeare;

            if (timer == 0)
            {
                worker.idWork++;
                worker.start = true;
            }
        }
    }

    public void smoothMoveUp(int idWork, Worker worker, int idPlayer)
    {
        if (idWork == worker.idWork)
        {
            if(worker.start)
            {
                timer = timeMoveUp;
                worker.start = false;
            }

            timer -= Time.deltaTime;
            if (timer < 0) timer = 0;

            for(int x = idPlayer + 1; x < playersManager.MAX_COUNT_PLAYERS; x++)
            {
                float posY = (positionOffset.Evaluate(1 - timer / timeMoveUp) - x) * (heightTabMenuPlayer + heightOffsetTabMenu);
                line[x].gameObject.transform.localPosition = new Vector3(line[x].gameObject.transform.localPosition.x, posY, line[x].gameObject.transform.localPosition.z);
                circle[x].gameObject.transform.localPosition = new Vector3(circle[x].gameObject.transform.localPosition.x, posY, circle[x].gameObject.transform.localPosition.z);
                nickname[x].gameObject.transform.localPosition = new Vector3(nickname[x].gameObject.transform.localPosition.x, posY, nickname[x].gameObject.transform.localPosition.z);
                player[x].gameObject.transform.localPosition = new Vector3(player[x].gameObject.transform.localPosition.x, posY, player[x].gameObject.transform.localPosition.z);
            }

            if(timer == 0)
            {
                worker.idWork++;
                worker.start = true;
                updateTabMenu();
            }
        }
    }

    void Start()
    {
        worker = new Worker();
        playersManager = GetComponent<PlayersManager>();
    }

    void Update()
    {
        smoothDisappPlayer(0, worker, disconnectedPlayerId);
        smoothMoveUp(1, worker, disconnectedPlayerId);

    }
}
