using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTableManager : MonoBehaviour
{

    public class RowMoveableContainer
    {
        public float timeToEvent = float.NaN;
        public GameObject row;

        public RowMoveableContainer(GameObject row)
        {
            this.row = row;
        }
    }

    public GameObject scoreTableRowPrifab;
    public List<RowMoveableContainer> rows;
    public PlayersManager playersManager;
    public GameplayManager gameplayManager;
    public IconsContent iconsContent;

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
            float distance = upPos - pos;

            float velocity = curve.Evaluate((distanceToLerp - distance) / distanceToLerp) * vel;
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

    public void ChangeStateOfScoreTable(GameObject gameObject)
    {
        int id = findRowId(gameObject);
        rows[id].timeToEvent = -1;
    }

    public void RemovePlayer(int id)
    {
        Destroy(rows[id].row);
        rows.RemoveAt(id);
    }

    public void UpdateTable()
    {
        for(int x = 0; x < rows.Count; x++)
        {
            ScoreTableRow scoreTableRow = rows[x].row.GetComponent<ScoreTableRow>();
            Player player = playersManager.players.get(x);
            scoreTableRow.FillRow(
                iconsContent.icons[player.iconId].sprite, 
                player.nickname, 
                player.claimedRegions.Count, 
                gameplayManager.regionSystem.regionSerds.Count, 
                player.scores, 
                player.color);
        }
    }

    void Awake()
    {
        rows = new List<RowMoveableContainer>();
    }

    void Start()
    {
        iconsContent = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContent>();
        for(int x = 0; x < playersManager.players.count; x++)
        {
            GameObject rowObject = Instantiate(scoreTableRowPrifab, transform);
            rowObject.GetComponent<ScoreTableRow>().scoreTableManager = this;
            rowObject.transform.localPosition = new Vector2(0, -(rowObject.GetComponent<RectTransform>().rect.height + offset) * x);

            RowMoveableContainer rowMovableContainer = new RowMoveableContainer(rowObject);
            rows.Add(rowMovableContainer);
        }

        UpdateTable();
    }

    void Update()
    {
        UpdateMoveRows();
        UpdateEventsOfRows();

        if (delete0)
        {
            Destroy(rows[0].row);
            rows.Remove(rows[0]);
            delete0 = false;
        }

        if (delete1)
        {
            Destroy(rows[1].row);
            rows.Remove(rows[1]);
            delete1 = false;
        }

        if (delete2)
        {
            Destroy(rows[2].row);
            rows.Remove(rows[2]);
            delete2 = false;
        }

        if (delete3)
        {
            Destroy(rows[3].row);
            rows.Remove(rows[2]);
            delete3 = false;
        }
    }
}
