using Foole.Mpq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class AppHandler : MonoBehaviour
{
    public static AppHandler Instance;
    public Image _image;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public List<MpqArchive> LoadedMPQs = new List<MpqArchive>();

    private Dictionary<string, int> xmlEntries = new Dictionary<string, int>();
    private Dictionary<string, int> adtEntries = new Dictionary<string, int>();
    private Dictionary<string, int> wdtEntries = new Dictionary<string, int>();
    private Dictionary<string, int> wdlEntries = new Dictionary<string, int>();
    private Dictionary<string, int> m2Entries = new Dictionary<string, int>();
    private Dictionary<string, int> wmoEntries = new Dictionary<string, int>();
    private Dictionary<string, int> skinEntries = new Dictionary<string, int>();
    private Dictionary<string, int> animEntries = new Dictionary<string, int>();
    private Dictionary<string, int> dbcEntries = new Dictionary<string, int>();
    private Dictionary<string, int> wdbEntries = new Dictionary<string, int>();
    private Dictionary<string, int> blpEntries = new Dictionary<string, int>();
    private Dictionary<string, int> blsEntries = new Dictionary<string, int>();
    private Dictionary<string, int> audioEntries = new Dictionary<string, int>();
    private Dictionary<string, int> luaEntries = new Dictionary<string, int>();

    private Dictionary<string, Texture2D> cursorCache = new Dictionary<string, Texture2D>();
    private Dictionary<string, Sprite> loadingScreenCache = new Dictionary<string, Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
            return;

        Instance = this;

        SettingsHandler.OnLoad();
        AudioHandler.OnLoad();
        NetworkHandler.OnLoad();
        SceneHandler.OnLoad();

        LoadMPQFiles();
        ParseXML(@"INTERFACE\GLUEXML\GLUEPARENT.XML");
        SetPointer(@"Interface\CURSOR\Point.blp");
        
        SceneHandler.Instance.SwitchToAuthScene();
    }
    public void LoadMPQFiles()
    {
        string[] _mpqFiles = Directory.GetFiles(SettingsHandler.Instance.LAST_KNOWN_MPQ_DATA_FOLDER, "*.MPQ", SearchOption.AllDirectories);
        for (int i = _mpqFiles.Length - 1; i >= 0; i--)
        {
            MpqArchive archive = new MpqArchive(_mpqFiles[i]);
            archive.AddListfileFilenames();
            LoadedMPQs.Add(archive);

            foreach (MpqEntry entry in archive._entries)
            {
                if (entry.Filename == null)
                    continue;

                string ext = Path.GetExtension(entry.Filename);
                string fileName = entry.Filename.ToUpper();

                switch (ext.ToUpper())
                {
                    case ".LUA":
                        if (luaEntries.ContainsKey(fileName))
                            luaEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            luaEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".XML":
                        if (xmlEntries.ContainsKey(fileName))
                            xmlEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            xmlEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".ADT":
                        if (adtEntries.ContainsKey(fileName))
                            adtEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            adtEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".WDT":
                        if (wdtEntries.ContainsKey(fileName))
                            wdtEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            wdtEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".WDL":
                        if (wdlEntries.ContainsKey(fileName))
                            wdlEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            wdlEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".WMO":
                        if (wmoEntries.ContainsKey(fileName))
                            wmoEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            wmoEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".M2":
                        if (m2Entries.ContainsKey(fileName))
                            m2Entries[fileName] = LoadedMPQs.Count - 1;
                        else
                            m2Entries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".BLP":
                        if (blpEntries.ContainsKey(fileName))
                            blpEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            blpEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".BLS":
                        if (blsEntries.ContainsKey(fileName))
                            blsEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            blsEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".SKIN":
                        if (skinEntries.ContainsKey(fileName))
                            skinEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            skinEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".DBC":
                        if (dbcEntries.ContainsKey(fileName))
                            dbcEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            dbcEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".ANIM":
                        if (animEntries.ContainsKey(fileName))
                            animEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            animEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                    case ".WAV":
                    case ".MP3":
                        if (audioEntries.ContainsKey(fileName))
                            audioEntries[fileName] = LoadedMPQs.Count - 1;
                        else
                            audioEntries.Add(fileName, LoadedMPQs.Count - 1);
                        break;
                }
            }
        }
    }
    public MpqStream SearchMPQ(string text)
    {
        text = text.ToUpper();
        string ext = Path.GetExtension(text);

        int value = -1;

        switch (ext.ToUpper())
        {
            case ".XML":
                if (xmlEntries.ContainsKey(text))
                    xmlEntries.TryGetValue(text, out value);
                break;
            case ".ADT":
                if (adtEntries.ContainsKey(text))
                    adtEntries.TryGetValue(text, out value);
                break;
            case ".WDT":
                if (wdtEntries.ContainsKey(text))
                    wdtEntries.TryGetValue(text, out value);
                break;
            case ".WDL":
                if (wdlEntries.ContainsKey(text))
                    wdlEntries.TryGetValue(text, out value);
                break;
            case ".WMO":
                if (wmoEntries.ContainsKey(text))
                    wmoEntries.TryGetValue(text, out value);
                break;
            case ".M2":
                if (m2Entries.ContainsKey(text))
                    m2Entries.TryGetValue(text, out value);
                break;
            case ".BLP":
                if (blpEntries.ContainsKey(text))
                    blpEntries.TryGetValue(text, out value);
                break;
            case ".BLS":
                if (blsEntries.ContainsKey(text))
                    blsEntries.TryGetValue(text, out value);
                break;
            case ".SKIN":
                if (skinEntries.ContainsKey(text))
                    skinEntries.TryGetValue(text, out value);
                break;
            case ".DBC":
                if (dbcEntries.ContainsKey(text))
                    dbcEntries.TryGetValue(text, out value);
                break;
            case ".ANIM":
                if (animEntries.ContainsKey(text))
                    animEntries.TryGetValue(text, out value);
                break;
            case ".WAV":
            case ".MP3":
                if (audioEntries.ContainsKey(text))
                    audioEntries.TryGetValue(text, out value);
                break;
        }

        if (value != -1)
        {
            return LoadedMPQs[value].OpenFile(text);
        }
        else
            return null;
    }
    void SetPointer(string pointerLocation)
    {
        Texture2D newPointer = null;

        if (cursorCache.ContainsKey(pointerLocation.ToUpper()))
            newPointer = cursorCache[pointerLocation.ToUpper()];
        else
        {
            cursorCache.Add(pointerLocation.ToUpper(), BLPLoader.ToTex(SearchMPQ(pointerLocation)));
            newPointer = cursorCache[pointerLocation.ToUpper()];
        }

        Cursor.SetCursor(newPointer, hotSpot, cursorMode);
    }
    void ParseXML(string xml)
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load(SearchMPQ(xml));

        // Get the root element
        XmlNode buttonNode = xDoc.SelectSingleNode("//Button[@name='AccountNameButton']");

        if (buttonNode != null)
        {
            // Get the Layers child element
            XmlNode layersNode = buttonNode.SelectSingleNode("Layers");

            // Get the ARTWORK Layer child element
            XmlNode artworkLayerNode = layersNode.SelectSingleNode("Layer[@level='ARTWORK']");

            // Get the BGHighlight Texture child element
            XmlNode bgHighlightNode = artworkLayerNode.SelectSingleNode("Texture[@name='$parentBGHighlight']");

            // Get the Anchors of the BGHighlight Texture element
            XmlNode anchorsNode = bgHighlightNode.SelectSingleNode("Anchors");

            // Get the Anchor element with point='LEFT'
            XmlNode leftAnchorNode = anchorsNode.SelectSingleNode("Anchor[@point='LEFT']");

            // Get the Offset child element of the left Anchor element
            XmlNode offsetNode = leftAnchorNode.SelectSingleNode("Offset");

            // Get the x and y attributes of the Offset element
            string xValue = offsetNode.Attributes["x"].Value;
            string yValue = offsetNode.Attributes["y"].Value;

            Console.WriteLine($"x = {xValue}, y = {yValue}");

            /*XmlNodeList scriptNodes = xDoc.GetElementsByTagName("Script");
        foreach (XmlNode scriptNode in scriptNodes)
        {
            string fileAttributeValue = scriptNode.Attributes["file"].Value;
        }
        xDoc.Save(Application.dataPath + "//sf.xml");*/
        }
    private void OnDisable()
    {
        foreach (MpqArchive mpq in LoadedMPQs)
        {
            mpq.Dispose();
        }

        LoadedMPQs.Clear();
    }
}
