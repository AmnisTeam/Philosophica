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
    public float value = 0;
    public float b;

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

        
        for(int x = 0; x < icons.Length; x++)
        {
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


        }

        oldMousePos = Input.mousePosition;
    }
}
