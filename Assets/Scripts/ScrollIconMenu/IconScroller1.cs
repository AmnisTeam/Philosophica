using UnityEngine;
using UnityEngine.UI;

public class IconScroller1 : MonoBehaviour
{
    public GameObject dragField;
    public bool isDrag = false;
    public float force = 1;
    public Vector2 oldMousePos;
    public Vector2 dragPoint;
    public GameObject iconPrifab;
    private GameObject[] icons;
    public float offset = 20;
    public float borderedOffset = 20;
    public float value = 0;

    public bool Intersect()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Rect rect = dragField.GetComponent<RectTransform>().rect;
        Vector2 dragFieldPos = dragField.transform.position.ToXY() - rect.size / 100f / 2f;

        return  (mousePosition.x >= dragFieldPos.x && mousePosition.x <= dragFieldPos.x + rect.width / 100f) &&
                (mousePosition.y >= dragFieldPos.y && mousePosition.y <= dragFieldPos.y + rect.height / 100f);
    }

    void Start()
    {
        oldMousePos = Input.mousePosition;
        int countIcons = (int)(dragField.GetComponent<RectTransform>().rect.width / (iconPrifab.GetComponent<RectTransform>().rect.width + offset));
        icons = new GameObject[countIcons];
        for (int x = 0; x < countIcons; x++)
        {
            icons[x] = Instantiate(iconPrifab, dragField.transform);
            icons[x].SetActive(true);
        }

        offset = (float)(dragField.GetComponent<RectTransform>().rect.width - iconPrifab.GetComponent<RectTransform>().rect.width * countIcons) / countIcons;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Intersect())
        {
            isDrag = true;
            dragPoint = Input.mousePosition;
        }
        if (isDrag && Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            Vector2 delta = Input.mousePosition.ToXY() - oldMousePos;
        }

        if(isDrag)
        {
            
        }

            //float iconWidth = icons[x].GetComponent<RectTransform>().rect.width;
            //float dragFieldWidth = dragField.GetComponent<RectTransform>().rect.width;
            //float teleport = (iconWidth + offset) * icons.Length * (int)((value + icons.Length - x) / icons.Length);
            //float width = (iconWidth + offset) * icons.Length;
            //icons[x].transform.localPosition = new Vector2(-(x - value) * (iconWidth + offset) + dragFieldWidth / 2 - iconWidth / 2 - teleport, 0);
            //float posX = icons[x].transform.localPosition.x;
            //CanvasGroup canvasGroup = icons[x].GetComponent<CanvasGroup>();
            //float a = ((float)posX / ((iconWidth + offset) * (icons.Length - 2)));
            //a = 1 - Mathf.Abs(a);
            //canvasGroup.alpha = a;

        for(int x = 0; x < icons.Length; x++)
        {
            float iconWidth = icons[x].GetComponent<RectTransform>().rect.width;
            float dragFieldWidth = dragField.GetComponent<RectTransform>().rect.width;
            icons[x].transform.localPosition = new Vector2((x - value) * (iconWidth + offset) - dragFieldWidth / 2 + iconWidth / 2, 0);
        }

        oldMousePos = Input.mousePosition;
    }
}
