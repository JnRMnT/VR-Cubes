using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ConfigurationManager : JMBehaviour
{
    public static bool IsGazeClickEnabled
    {
        get
        {
            return PlayerPrefs.GetInt("IsGazeClickEnabled", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("IsGazeClickEnabled", value ? 1 : 0);
        }
    }

    public static string ActiveLanguage
    {
        get
        {
            string activeLanguage = PlayerPrefs.GetString("ActiveLanguage", string.Empty);
            if (string.IsNullOrEmpty(activeLanguage))
            {
                if (Application.systemLanguage == SystemLanguage.Turkish)
                {
                    activeLanguage = "tr";
                }
                else
                {
                    activeLanguage = "en";
                }
            }
            return activeLanguage;
        }
        set
        {
            PlayerPrefs.SetString("ActiveLanguage", value);
        }
    }

    public static int ActiveLanguageIndex
    {
        get
        {
            string activeLanguage = ActiveLanguage;
            for (int i = 0; i < AvailableLanguages.Length; i++)
            {
                if (AvailableLanguages[i] == activeLanguage)
                {
                    return i;
                }
            }

            return 0;
        }
        set
        {
            ActiveLanguage = AvailableLanguages[value % AvailableLanguages.Length];
        }
    }

    public static readonly string[] AvailableLanguages = new string[] { "en", "tr" };
}
