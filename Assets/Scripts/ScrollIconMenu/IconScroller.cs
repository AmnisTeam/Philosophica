using UnityEngine;
using UnityEngine.UI;

public class IconScroller : MonoBehaviour
{
    public GameObject dragField;
    public bool isDrag = false;
    public float force = 1;
    public Vector2 oldMousePos;
    public Image[] icons;
    public float offset = 20;
    public Vector2 dragPoint;
    public float value;
    public float valueVelocity = 0;
    public float friction = 0.01f;
    public float gravityVelCof = 0;
    public int selectedId;
    public Sprite[] sprites;

    public Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public bool Intersect()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Rect rect = dragField.GetComponent<RectTransform>().rect;
        Vector2 dragFieldPos = dragField.transform.position.ToXY() - rect.size / 100f / 2f;

        return  (mousePosition.x >= dragFieldPos.x && mousePosition.x <= dragFieldPos.x + rect.width / 100f) &&
                (mousePosition.y >= dragFieldPos.y && mousePosition.y <= dragFieldPos.y + rect.height / 100f);
    }

    public float getDelta()
    {
        Vector2 delta = Input.mousePosition.ToXY() - oldMousePos;
        return -delta.x / 100;
    }

    public void setIconGravity()
    {
        float iconWidth = icons[0].GetComponent<RectTransform>().rect.width;
        float width = (iconWidth + offset) * icons.Length;
        float minDistance = 99999;
        int minId = 0;

        for(int x = 0; x < icons.Length; x++)
        {
            float distance = Mathf.Abs(icons[x].transform.localPosition.x / (width / 2));
            if (distance < minDistance)
            {
                minDistance = distance;
                minId = x;
            }
        }

        valueVelocity = Mathf.Lerp(valueVelocity, -icons[minId].transform.localPosition.x / (width / 2) * gravityVelCof, gravityVelCof);
    }

    public void updateVelocity()
    {
        value += valueVelocity * Time.deltaTime * 100;
        valueVelocity *= (1 - friction);
        setIconGravity();
    }

    void Start()
    {
        oldMousePos = Input.mousePosition;
        value = sprites.Length * 1000;
    }

    void Update()
    {
        updateVelocity();
        if (Input.GetMouseButtonDown(0) && Intersect())
        {
            isDrag = true;
            dragPoint = Input.mousePosition;
        }
        if (isDrag && Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            valueVelocity = getDelta();
        }

        if(isDrag)
        {
            value += getDelta();
            valueVelocity = 0;
        }

        
        for(int x = 0; x < icons.Length; x++)
        {
            float iconWidth = icons[x].GetComponent<RectTransform>().rect.width;
            float dragFieldWidth = dragField.GetComponent<RectTransform>().rect.width;
            float teleport = (iconWidth + offset) * icons.Length * (int)((value + icons.Length - x) / icons.Length);
            float width = (iconWidth + offset) * icons.Length;
            icons[x].transform.localPosition = new Vector2(-(x - value) * (iconWidth + offset) + dragFieldWidth / 2 - iconWidth / 2 - teleport, 0);
            int id = x + (int)((value + icons.Length - x) / icons.Length) * icons.Length;
            icons[x].GetComponent<Image>().sprite = sprites[id % sprites.Length];
            float posX = icons[x].transform.localPosition.x;
            CanvasGroup canvasGroup = icons[x].GetComponent<CanvasGroup>();
            float a = ((float)posX / ((iconWidth + offset) * (icons.Length - 2)));
            a = 1 - Mathf.Abs(a);
            canvasGroup.alpha = a;
        }
        selectedId = ((int)(Mathf.Round(value)) + 1) % sprites.Length;

        oldMousePos = Input.mousePosition;
    }
}
