﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Core.Player;
using Core.Shared.Enum;
using UnityEngine;

namespace Core.Shared.SaveSystem
{
    public static class SaveSystem
    {
        //paths
        public static string dataPath = Application.persistentDataPath;
        public const string playerStateFileName = "/player_stats.bin";
        //keys:
        private const string key = "5ai2M8OF19lfMV590dvy8iU1a23ZXa21";
        private static byte[] keyInBytes = System.Text.Encoding.ASCII.GetBytes(key);
        private const string iv = "a57T0s5UOp1wS4x9";
        private static byte[] ivInBytes = System.Text.Encoding.ASCII.GetBytes(iv);

        //pre: --
        //post: returns true if saved game exists
        public static bool SaveGameExists()
        {
            return File.Exists(dataPath + playerStateFileName);
        }

        //pre: --
        //post: saves player state
        public static void SavePlayerState(PlayerState playerData)
        {
            string path = dataPath + playerStateFileName;
            Encrypt(path, playerData);
        }

        //pre: files exist
        //post: loads player state and returns PlayerState obj
        public static PlayerState LoadPlayerState()
        {
            string path = Application.persistentDataPath + "/player_stats.bin";
            if (File.Exists(path))
            {
                try
                {
                    PlayerState playerData = (PlayerState)Decrypt(path);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error loading player state: " + e.Message);
                    return PlayerStateDefaultValues();
                }
                return (PlayerState)Decrypt(path);
            }
            else
            {
                return PlayerStateDefaultValues();
            }
        }

        //pre: --
        //post: init files for new game seted and saved
        public static void InitializeGame()
        {
            SavePlayerState(PlayerStateInitValues());
        }

        public static void SaveGameAfterFirstCinematographic()
        {
            SavePlayerState(PlayerStateDefaultValues());
        }

        //pre: -- 
        //post: returns a PlayerState with default values
        private static PlayerState PlayerStateInitValues()
        {
            //TODO: Configure an extern file (or files) for new game default values
            return new PlayerState((int)SceneID.Cinematographic_Intro_P1,
                                    3,
                                    3,
                                    new Vector3(109f, -3.6f, 0),
                                    new Dictionary<Ability, bool>{
                                        {Ability.Dash, false},
                                        {Ability.Ray, false},
                                    });
        }

        //pre: -- 
        //post: returns a PlayerState with default values
        private static PlayerState PlayerStateDefaultValues()
        {
            //TODO: Configure an extern file (or files) for new game default values
            return new PlayerState((int)SceneID.OmedIsland_Zone1,
                                    3,
                                    3,
                                    new Vector3(109f, -3.6f, 0),
                                    new Dictionary<Ability, bool>{
                                        {Ability.Dash, false},
                                        {Ability.Ray, false},
                                    });
        }

        #region "save and encryption"

        //pre: filepath corrent and objectToSave != null
        //post: encrypted file with objectToSave info saved
        private static void Encrypt(string filePath, System.Object objectTosave)
        {

            Aes aes = Aes.Create();
            aes.Key = keyInBytes;
            aes.IV = ivInBytes;

            var encryptor = aes.CreateEncryptor();
            MemoryStream ms = new MemoryStream();
            byte[] data = ObjectToByteArray(objectTosave);

            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
            }

            System.IO.File.WriteAllBytes(filePath, ms.ToArray());
        }

        //pre: filepath corrent 
        //post: dencrypts file and returns object with file info
        private static System.Object Decrypt(string filePath)
        {
            Aes aes = Aes.Create();
            aes.Key = keyInBytes;
            aes.IV = ivInBytes;

            byte[] data = FileToByteArray(filePath);

            var decryptor = aes.CreateDecryptor();
            MemoryStream ms = new MemoryStream();

            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
            }

            return ByteArrayToObject(ms.ToArray());
        }

        //pre: filename correct
        //post: returns byte array of file
        private static byte[] FileToByteArray(string FileName)
        {
            byte[] fileBytes = null;

            if (File.Exists(FileName))
            {
                fileBytes = System.IO.File.ReadAllBytes(FileName);
            }
            else
            {
                Console.WriteLine("File '{0}' not found", FileName);
            }

            return fileBytes;
        }

        //pre: obj != null
        //post: returns byte array of object
        private static byte[] ObjectToByteArray(System.Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        //pre: arrBytes correct
        //post: returns object of byte array 
        private static System.Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            System.Object obj = (System.Object)binForm.Deserialize(memStream);

            return obj;
        }

        #endregion

    }
}
