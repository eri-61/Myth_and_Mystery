using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GameConstants
{
    /// <summary>
    /// These are the saveable Scenes
    /// </summary>
    public static string[,] ChapterDialogs { get {
            string[,] chapDia = new string[4, 10];
            chapDia[0, 0] = "VN_Office";
            chapDia[0, 1] = "VN_OfficeD2";
            chapDia[0, 2] = "IS_Camp";
            chapDia[0, 3] = "IS_Camp";

            return chapDia;
        }     
    } 
}