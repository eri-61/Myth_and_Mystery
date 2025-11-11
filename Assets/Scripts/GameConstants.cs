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
            chapDia[0, 0] = "";
            chapDia[0, 1] = "VN_Office";
            chapDia[0, 2] = "VN_OfficeD2";
            chapDia[0, 3] = "IS_Camp";
            chapDia[0, 4] = "VN_Camp";
            chapDia[0, 5] = "VN_Lambago";
            chapDia[0, 6] = "VN_Clearing";
            chapDia[0, 7] = "Battle_Scene";

            return chapDia;
        }     
    } 
}