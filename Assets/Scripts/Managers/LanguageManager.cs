using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LanguageManager : JMBehaviour
{
    public static bool Ready;
    public static string playerLanguage;
    protected static Dictionary<string, string> dictionary;

    protected void Start()
    {
        UpdateLanguage(ConfigurationManager.ActiveLanguage);
    }

    public static void UpdateLanguage(string language = null)
    {
        dictionary = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(language))
        {
            ConfigurationManager.ActiveLanguageIndex++;
            language = ConfigurationManager.ActiveLanguage;
        }
        playerLanguage = language;
        readFromLanguageFile();

        JMLocalizable[] localizables = GameObject.FindObjectsOfType<JMLocalizable>();
        if(localizables != null)
        {
            foreach(JMLocalizable localizable in localizables)
            {
                localizable.UpdateText();
            }
        }
    }

    private static void readFromLanguageFile()
    {
        FileInfo theSourceFile = null;

        TextReader reader = null;  // NOTE: TextReader, superclass of StreamReader and StringReader

        // Read from plain text file if it exists

        theSourceFile = new FileInfo(Application.dataPath + "../Resources/Languages/" + playerLanguage + ".txt");
        if (theSourceFile != null && theSourceFile.Exists)
        {
            reader = theSourceFile.OpenText();  // returns StreamReader
        }
        else
        {
            // try to read from Resources instead
            TextAsset puzdata = (TextAsset)Resources.Load("Languages/" + playerLanguage, typeof(TextAsset));
            reader = new StringReader(puzdata.text);  // returns StringReader
        }

        if (reader == null)
        {
            Debug.Log("'" + Application.dataPath + "/Languages/" + playerLanguage + ".txt' Language file not found or not readable");
        }
        else
        {
            // Read each line from the file/resource
            string txt;
            while ((txt = reader.ReadLine()) != null)
            {
                string[] key_value = txt.Split('=');
                if (dictionary.ContainsKey(key_value[0])) dictionary[key_value[0]] = key_value[1];
                else dictionary.Add(key_value[0], key_value[1]);
            }
        }

        Ready = true;
    }

    public static string GetString(string key)
    {
        string value;
        key = key.ToUpperInvariant();
        dictionary.TryGetValue(key, out value);
        if (string.IsNullOrEmpty(value))
        {
            return key;
        }
        else
        {
            return value;
        }
    }
}