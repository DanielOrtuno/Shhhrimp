using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour {
    public static GameController gametroller;

    public float playerZ;
    public float playerY;
    public float playerX;


    private void Awake()
    {
        if(gametroller == null)
        {
            DontDestroyOnLoad(gameObject);
            gametroller = this;
        }
        else if (gametroller != this)
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        BinaryFormatter newBinaryFile = new BinaryFormatter();
        FileStream newFile = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerInfo info = new PlayerInfo
        {
            playerPosX = playerX,
            playerPosY = playerY,
            playerPosZ = playerZ
        };

        newBinaryFile.Serialize(newFile, info);
        newFile.Close();
    }


    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter newBinaryFile = new BinaryFormatter();
            FileStream newFile = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            PlayerInfo info = (PlayerInfo)newBinaryFile.Deserialize(newFile);
            newFile.Close();

            playerX = info.playerPosX;
            playerY = info.playerPosY;
            playerZ = info.playerPosZ;
        }
       
    }

    public void Delete()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            File.Delete(Application.persistentDataPath + "/playerInfo.dat");
        }
    }

    [Serializable]
    class PlayerInfo
    {
        public float playerPosX;
        public float playerPosY;
        public float playerPosZ;
    }
}
