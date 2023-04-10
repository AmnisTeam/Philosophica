using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGamePlayersTable : MonoBehaviour
{
    public GameObject playerRecordPrifab;
    public float offset;
    public PlayersManager personsManager;

    public List<EndGamePlayerRecord> playerRecords = null;

    public void AddRecord(Player person)
    {
        float playerRecordSizeY = playerRecordPrifab.GetComponent<RectTransform>().rect.height;
        GameObject record = Instantiate(playerRecordPrifab, transform);
        record.transform.localPosition = new Vector3(0, -(playerRecordSizeY + offset) * playerRecords.Count);
        EndGamePlayerRecord endGameRecord = record.GetComponent<EndGamePlayerRecord>();
        endGameRecord.SetRecord(playerRecords.Count + 1, person);
        playerRecords.Add(endGameRecord);
    }

    public void SetTable(List<Player> persons)
    {
        if (playerRecords != null)
        {
            for (int x = 0; x < playerRecords.Count; x++)
                Destroy(playerRecords[x].gameObject);
            playerRecords = new List<EndGamePlayerRecord>();
        }

        for (int x = 0; x < persons.Count; x++)
            AddRecord(persons[x]);
    }
}
