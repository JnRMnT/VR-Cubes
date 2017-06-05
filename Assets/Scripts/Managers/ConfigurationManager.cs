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
}
