using System;
using System.Collections;
using UnityEngine;


namespace SpiralJumper
{
    public static class GoogleSheets
    {
        private const string VersionEntry = "";

        private const string SessionEntry = "";
        private const string PlayTimeEntry = "";
        private const string PropertyNameEntry = "";
        private const string ValueEntry = "";
        private const string LevelEntry = "";
        private const string ScoreEntry = "";

        private const string URL = "";


        public static void SendProperty(string propertyName, string value, int level, int score)
        {
            if (SpiralJumper.get.NeedSendStat)
                SpiralJumper.get.StartCoroutine(SendProperty_Post(propertyName, value, level, score));
        }

        public static void SendProperty(string propertyName, string value)
        {
            if (SpiralJumper.get.NeedSendStat)
                SpiralJumper.get.StartCoroutine(SendProperty_Post(propertyName, value));
        }

        private static IEnumerator SendProperty_Post(string propertyName, string value, int level, int score)
        {
            WWWForm form = new WWWForm();

            form.AddField(VersionEntry, Application.version);
            form.AddField(SessionEntry, SpiralJumper.get.SessionID.ToString());

            form.AddField(PlayTimeEntry, SpiralJumper.get.PlayTime.ToString());
            form.AddField(PropertyNameEntry, propertyName);
            form.AddField(ValueEntry, value);
            form.AddField(LevelEntry, level.ToString());
            form.AddField(ScoreEntry, score.ToString());

            WWW www = new WWW(URL, form.data);
            Debug.Log("GoogleSheets Send Begin");
            yield return www;
            Debug.Log("GoogleSheets Send End");
        }

        private static IEnumerator SendProperty_Post(string propertyName, string value)
        {
            WWWForm form = new WWWForm();

            form.AddField(VersionEntry, Application.version);
            form.AddField(SessionEntry, SpiralJumper.get.SessionID.ToString());

            form.AddField(PlayTimeEntry, SpiralJumper.get.PlayTime.ToString());
            form.AddField(PropertyNameEntry, propertyName);
            form.AddField(ValueEntry, value);

            WWW www = new WWW(URL, form.data);
            Debug.Log("GoogleSheets Send Begin");
            yield return www;
            Debug.Log("GoogleSheets Send End");
        }

    }
}