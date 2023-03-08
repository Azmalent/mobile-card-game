using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LogGUI : MonoBehaviour {
    private struct Item {
        public float destroyTime;
        public string text;
    }

    public Text text;

    LinkedList<Item> queue = new LinkedList<Item>();
    uint qsize = 15;
    float lifetime = 10;

    private void Awake() {
        DiGro.Check.NotNull(text);
    }

    private void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    private void Update() {
        //bool hasChanges = false;
        //while (queue.Count > 0 && queue.First.Value.destroyTime <= Time.time) {
        //    queue.RemoveFirst();
        //    hasChanges = true;
        //}
        //if (hasChanges)
        //    UpdateText();
    }

    private void HandleLog(string logString, string stackTrace, LogType type) {
        if (type == LogType.Warning)
            return;

        queue.AddLast(new Item () {
           destroyTime = Time.time + lifetime,
           text = "[" + type + "] : " + logString
        });

        while (queue.Count > qsize)
            queue.RemoveFirst();

        UpdateText();
    }

    private void UpdateText() {
        string str = "";
        foreach(var item in queue)
            str += item.text + "\n";

        text.text = str;
    }

}
