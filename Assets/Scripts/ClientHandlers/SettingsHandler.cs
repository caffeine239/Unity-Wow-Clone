using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    public static SettingsHandler Instance;

    public Dictionary<string[], int> realmListEntries;

    public string REALM_LIST_ADDRESS = "127.0.0.1";
    public string LAST_KNOWN_REALMNAME = " ";
    public int LAST_KNOWN_REALM_PORT = 3724;
    public string LAST_KNOWN_MPQ_DATA_FOLDER = " ";
    public string WEBSITE_LINK = " ";
    public string MANAGE_ACCOUNT_LINK = " ";

    // Start is called before the first frame update
    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Settings Handler");
        SettingsHandler newManager = go.AddComponent<SettingsHandler>();        
        newManager.LAST_KNOWN_MPQ_DATA_FOLDER = @Application.dataPath + "/Data/";
        newManager.WEBSITE_LINK = @"https://wowclassic.blizzard.com/en-us/";
        newManager.MANAGE_ACCOUNT_LINK = @"https://account.battle.net/";
        newManager.realmListEntries = new Dictionary<string[], int>
        {
            { new string[] { newManager.REALM_LIST_ADDRESS, newManager.LAST_KNOWN_REALMNAME }, newManager.LAST_KNOWN_REALM_PORT }
        };

        newManager.LoadDataFile();

        Instance = newManager;
    }

    public void LoadDataFile()
    {
        string path = @Application.dataPath + "/data.wtf";
        if (!File.Exists(path))
        {
            File.Create(path).Close();

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALMNAME " + LAST_KNOWN_REALMNAME);
                w.WriteLine("LAST_KNOWN_REALM_PORT " + LAST_KNOWN_REALM_PORT.ToString());
                w.WriteLine("LAST_KNOWN_MPQ_DATA_FOLDER " + LAST_KNOWN_MPQ_DATA_FOLDER);
                w.WriteLine("WEBSITE_LINK " + WEBSITE_LINK);
                w.WriteLine("MANAGE_ACCOUNT_LINK " + MANAGE_ACCOUNT_LINK);
                w.Close();
            }
        }

        string[] Config = File.ReadAllLines(path);

        foreach (string line in Config)
        {
            if (line.Contains("REALM_LIST_ADDRESS "))
            {
                REALM_LIST_ADDRESS = line.Substring(19);
            }
            if (line.Contains("LAST_KNOWN_REALMNAME "))
            {
                LAST_KNOWN_REALMNAME = line.Substring(21);
            }
            if (line.Contains("LAST_KNOWN_REALM_PORT "))
            {
                LAST_KNOWN_REALM_PORT = int.Parse(line.Substring(22));
            }
            if (line.Contains("LAST_KNOWN_MPQ_DATA_FOLDER "))
            {
                LAST_KNOWN_MPQ_DATA_FOLDER = line.Substring(27);
            }
            if (line.Contains("WEBSITE_LINK "))
            {
                WEBSITE_LINK = line.Substring(13);
            }
            if (line.Contains("MANAGE_ACCOUNT_LINK "))
            {
                MANAGE_ACCOUNT_LINK = line.Substring(20);
            }
        }
    }
    public void WriteWTFfile()
    {
        string path = @Application.dataPath + "/data.wtf";
        if (!File.Exists(path))
        {
            File.Create(path).Close();

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALMNAME " + LAST_KNOWN_REALMNAME);
                w.WriteLine("LAST_KNOWN_REALM_PORT " + LAST_KNOWN_REALM_PORT.ToString());
                w.WriteLine("LAST_KNOWN_MPQ_DATA_FOLDER " + LAST_KNOWN_MPQ_DATA_FOLDER);
                w.WriteLine("WEBSITE_LINK " + WEBSITE_LINK);
                w.WriteLine("MANAGE_ACCOUNT_LINK " + MANAGE_ACCOUNT_LINK);
            }
        }
        else
        {
            File.Delete(path);
            File.Create(path).Close();

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALMNAME " + LAST_KNOWN_REALMNAME);
                w.WriteLine("LAST_KNOWN_REALM_PORT " + LAST_KNOWN_REALM_PORT.ToString());
                w.WriteLine("LAST_KNOWN_MPQ_DATA_FOLDER " + LAST_KNOWN_MPQ_DATA_FOLDER);
                w.WriteLine("WEBSITE_LINK " + WEBSITE_LINK);
                w.WriteLine("MANAGE_ACCOUNT_LINK " + MANAGE_ACCOUNT_LINK);
            }
        }
    }
}
