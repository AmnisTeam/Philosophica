using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icons : MonoBehaviour
{
    [SerializeField] private IconsContentHolder holder;

    void Awake()
    {
        holder = GameObject.FindGameObjectWithTag("ICONS_CONTENT_TAG").GetComponent<IconsContentHolder>();
    }

    public Sprite[] GetIconsSprite()
    {
        return holder.lobbyIcons;
    }

    public Sprite GetIconsSpriteByID(int ID)
    {
        return holder.lobbyIcons[ID];
    }
}
