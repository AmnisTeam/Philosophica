using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorsHolder : MonoBehaviour {
    public static ColorsHolder instance;
    public List<Color32> colors;
    public List<int> freeIndicies;

    public ColorsHolder() {
        instance = this;
    }

    void Start() {
        colors = new List<Color32> {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.magenta
        };

        freeIndicies = new List<int>(Enumerable.Range(0, colors.Count).ToArray());
    }

    void Update() {
        
    }

    public Color32 getRandomColor() {
        System.Random rnd = new System.Random();
        int index = rnd.Next(0, colors.Count);

        Color32 savedColor = colors[index];
        colors.RemoveAt(index);

        return savedColor;
    }

    public int getRandomIndex() {
        System.Random rnd = new System.Random();
        int index = rnd.Next(0, freeIndicies.Count);

        return index;
    }

    public void refillFreeIndicies() {
        freeIndicies = new List<int>(Enumerable.Range(0, colors.Count).ToArray());
    }

    public void putColorBack(Color32 color) {
        colors.Add(color);
    }
}
