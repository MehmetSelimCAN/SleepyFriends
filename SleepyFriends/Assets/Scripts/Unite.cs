using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unite : ICommand
{
    private int childCount = 1;
    private Transform parent;
    private Sprite sleepyPlayerSprite = Resources.Load<Sprite>("Sprites/Player/SleepyPlayer");

    public Unite(Transform parent, int childCount) {
        this.parent = parent;
        this.childCount = childCount;
    }

    public void Execute() {

    }

    public void Undo() {
        for (int i = parent.childCount; i > childCount; i--) {
            Transform child = parent.GetChild(i - 1);
            child.tag = "SleepyPlayer";
            child.GetComponent<Animator>().SetTrigger("SleepySleepy");
            child.GetComponent<SpriteRenderer>().sprite = sleepyPlayerSprite;
            child.SetParent(null);
        }
    }
}
