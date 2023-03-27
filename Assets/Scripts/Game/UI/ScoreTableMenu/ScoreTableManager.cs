using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class ScoreTableManager : MonoBehaviourPunCallbacks
{

    public class RowMoveableContainer
    {
        public float timeToEvent = float.NaN;
        public GameObject row;
        public Player player = null;

        public RowMoveableContainer(GameObject row)
        {
            this.row = row;
        }
    }

    public GameObject scoreTableRowPrifab;
    public List<RowMoveableContainer> rows;
    public PlayersManager playersManager;
    public GameplayManager gameplayManager;
    public IconsContentHolder iconsContent;
    public ColorsHolder colorsHolder;

    public float offset;
    public float distanceToLerp;
    public float delayToOpenOrClose;
    public float vel;
    public AnimationCurve curve;

    public bool delete0 = false;
    public bool delete1 = false;
    public bool delete2 = false;
    public bool delete3 = false;

    public void UpdateMoveRows()
    {
        for (int x = 0; x < rows.Count; x++)
        {
            RowMoveableContainer row = rows[x];

            float upPos = 0;
            if (x > 0)
                upPos = rows[x - 1].row.transform.localPosition.y - (rows[x - 1].row.GetComponent<RectTransform>().rect.height + offset);

            float pos = rows[x].row.transform.localPosition.y;
            float distance = Mathf.Abs(upPos - pos);

            float velocity = Mathf.Sign(upPos - pos) * curve.Evaluate((distanceToLerp - distance) / distanceToLerp) * vel;
            row.row.transform.localPosition = new Vector2(0, row.row.transform.localPosition.y + velocity);
        }
    }

    public void UpdateEventsOfRows()
    {
        for(int x = 0; x < rows.Count; x++)
        {
            RowMoveableContainer row = rows[x];

            row.timeToEvent -= Time.deltaTime;
            if(row.timeToEvent < 0)
            {
                if (row.row.GetComponent<ScoreTableRow>().isOpen)
                    row.row.GetComponent<ScoreTableRow>().CloseRow();
                else
                    row.row.GetComponent<ScoreTableRow>().OpenRow();

                row.row.GetComponent<ScoreTableRow>().isOpen = !row.row.GetComponent<ScoreTableRow>().isOpen;
                if (x > 0)
                    if(rows[x - 1].row.GetComponent<ScoreTableRow>().isOpen != row.row.GetComponent<ScoreTableRow>().isOpen)
                        rows[x - 1].timeToEvent = delayToOpenOrClose;
                if (x < rows.Count - 1)
                    if (rows[x + 1].row.GetComponent<ScoreTableRow>().isOpen != row.row.GetComponent<ScoreTableRow>().isOpen)
                            rows[x + 1].timeToEvent = delayToOpenOrClose;
                row.timeToEvent = float.NaN;
            }
        }
    }

    public int findRowId(GameObject gameObject)
    {
        int id = 0;
        for (int x = 0; x < rows.Count; x++)
            if (rows[x].row == gameObject)
            {
                id = x;
                break;
            }
        return id;
    }

    public ScoreTableRow FindRowByPlayer(Player player)
    {
        GameObject findedRow = null;
        for(int x = 0; x < rows.Count; x++)
            if(rows[x].player == player)
            {
                findedRow = rows[x].row;
                break;
            }
        return findedRow.GetComponent<ScoreTableRow>();
    }

    public void ChangeStateOfScoreTable(GameObject gameObject)
    {
        int id = findRowId(gameObject);
        rows[id].timeToEvent = -1;
    }

    public void RemovePlayer(int id)
    {
        PhotonNetwork.Destroy(rows[id].row);
        rows.RemoveAt(id);
    }

    public void RecreateTable() {
        for (int i = 0; i < rows.Count; i++) {
            PhotonNetwork.Destroy(rows[i].row);
            rows.RemoveAt(i);
            i--;
        }

        for(int x = 0; x < PhotonNetwork.CurrentRoom.PlayerCount; x++)
        {
            GameObject rowObject = PhotonNetwork.Instantiate(scoreTableRowPrifab.name, new Vector3(), Quaternion.identity);
            rowObject.transform.SetParent(transform, false);
            rowObject.GetComponent<ScoreTableRow>().scoreTableManager = this;
            rowObject.transform.localPosition = new Vector2(0, -(rowObject.GetComponent<RectTransform>().rect.height + offset) * x);

            RowMoveableContainer rowMovableContainer = new RowMoveableContainer(rowObject);
            rows.Add(rowMovableContainer);
        }

        SetTable();
    }

    public List<Player> GetSortedPlayers()
    {
        List<Player> players = new List<Player>();
        for (int x = 0; x < playersManager.players.count; x++)
            players.Add(playersManager.players.get(x));

        players.Sort((Player a, Player b) =>
        {
            if (a.isLose) return 1;
            else if (b.isLose) return -1;
            else if (a.scores > b.scores) return -1;
            else if (a.scores < b.scores) return 1;
            else if (a.claimedRegions.Count > b.claimedRegions.Count) return -1;
            else if (a.claimedRegions.Count < b.claimedRegions.Count) return 1;
            else return 0;
        });

        return players;
    }

    /*
     * Ќазначает подр€д идущим (сверху вниз) запис€м в таблицы данные от игроков, которые
     * сортированы по убыванию очков, а затем по убыванию регионов. 
     * 
     * ƒанные заново назначаютс€, без плавной сортировки!!! 
     * 
     * Ётот метод вызываетс€ в методе RecreateTable() и лучше нигде в другом месте его не юзать 
     * (ну если сильно захочетс€, то можно :D )
     */
    public void SetTable()
    {
        List<Player> players = GetSortedPlayers();

        for (int x = 0; x < rows.Count; x++) {
            ScoreTableRow scoreTableRow = rows[x].row.GetComponent<ScoreTableRow>();
            rows[x].player = players[x];

            scoreTableRow.FillRow(
                iconsContent.lobbyIcons[players[x].iconId], 
                players[x].nickname, 
                players[x].claimedRegions.Count, 
                gameplayManager.regionSystem.regionSerds.Count, 
                players[x].scores, 
                colorsHolder.colors[players[x].colorId]
            );
        }
    }

    /*
     * ѕеремещает уже созданные записи в таблице (в них также уже должны быть данные)
     * в пор€дке сортировки.
     */
    public void UpdateRowsOrder()
    {
        List<Player> players = GetSortedPlayers();

        for(int x = 0; x < players.Count; x++)
        {
            int id = -1;
            for(int y = 0; y < rows.Count; y++)
                if(rows[y].player == players[x])
                {
                    id = y;
                    break;
                }


            RowMoveableContainer temp = rows[x];
            rows[x] = rows[id];
            rows[id] = temp;
        }
    }

    /*
     * ќбнавл€ет уже созданные данные в таблице, а также плавно сортирует их 
     */
    public void UpdateTable() {
        for(int x = 0; x < rows.Count; x++) {
            ScoreTableRow row = rows[x].row.GetComponent<ScoreTableRow>();

            row.FillRow(
                iconsContent.lobbyIcons[rows[x].player.iconId],
                rows[x].player.nickname,
                rows[x].player.claimedRegions.Count,
                gameplayManager.regionSystem.regionSerds.Count,
                rows[x].player.scores,
                colorsHolder.colors[rows[x].player.colorId]
            );
        }

        UpdateRowsOrder();
    }

    void Awake()
    {
        rows = new List<RowMoveableContainer>();
    }

    void Start()
    {
        iconsContent = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
        colorsHolder = GameObject.FindGameObjectWithTag("COLOR_CONTENT_TAG").GetComponent<ColorsHolder>();
/*        for(int x = 0; x < playersManager.players.count; x++)
        {
            GameObject rowObject = Instantiate(scoreTableRowPrifab, transform);
            rowObject.GetComponent<ScoreTableRow>().scoreTableManager = this;
            rowObject.transform.localPosition = new Vector2(0, -(rowObject.GetComponent<RectTransform>().rect.height + offset) * x);

            RowMoveableContainer rowMovableContainer = new RowMoveableContainer(rowObject);
            rows.Add(rowMovableContainer);
        }*/

        RecreateTable();
    }

    void Update()
    {
        UpdateMoveRows();
        UpdateEventsOfRows();

        if (delete0)
        {
            //Destroy(rows[0].row);
            //rows.Remove(rows[0]);
            RemovePlayer(0);
            delete0 = false;
        }

        if (delete1)
        {
            RemovePlayer(1);
            delete1 = false;
        }

        if (delete2)
        {
            RemovePlayer(2);
            delete2 = false;
        }

        if (delete3)
        {
            RemovePlayer(3);
            delete3 = false;
        }
    }
}
