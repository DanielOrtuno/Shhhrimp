using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


//public static class SaveLoad
//{

//    public static void SavePlayerPos(PlayerController Player)
//    {
//        if (!Directory.Exists("Saves"))
//        {
//            Directory.CreateDirectory("Saves");
//        }
//        BinaryFormatter bf = new BinaryFormatter();
//        FileStream newStream = new FileStream(Application.persistentDataPath + "/player.sav", FileMode.Create);

//        //PlayerData playuerData = new PlayerData(Player);
//        PlayerPos playerPos = new PlayerPos(Player);

//        //bf.Serialize(newStream, playuerData);
//        bf.Serialize(newStream, playerPos);
//        newStream.Close();
//    }

//    public static Vector3 LoadPlayerPos()
//    {
//        if (File.Exists(Application.persistentDataPath + "/player.sav"))
//        {
//            BinaryFormatter bf = new BinaryFormatter();
//            FileStream newStream = new FileStream(Application.persistentDataPath + "/player.sav", FileMode.Open);

//            //PlayerData playuerData = bf.Deserialize(newStream) as PlayerData;
//            PlayerPos playerPos = (PlayerPos)bf.Deserialize(newStream);


//            newStream.Close();
//            return playerPos.playersPos;
//        }
//        else
//        {
//            Debug.LogError("File does not exist");
//            return new Vector3();
//        }
//        //return playuerData.playerStats;
//    }

//    //[Serializable]
//    //public class PlayerData
//    //{
//    //    public int[] playerStats;
//    //    public PlayerData(PlayerController player)
//    //    {
//    //        playerStats = new int[5];
//    //        playerStats[0] = player.playerHP;
//    //        playerStats[1] = player.playerScore;
//    //        playerStats[2] = player.dartCount;
//    //        playerStats[3] = player.noiseMakerCount;
//    //        playerStats[4] = player.smokeBombCount;
//    //    }
//    //}
//    [Serializable]
//    public class PlayerPos
//    {
//        public Vector3 playersPos;
//        public PlayerPos(PlayerController player)
//        {
//            playersPos = new Vector3(playerX, player.playerY, player.playerZ);
//        }
//    }
//}
