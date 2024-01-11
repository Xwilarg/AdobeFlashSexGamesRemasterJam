﻿using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine;

namespace FlashSexJam.Persistency
{
    public class PersistencyManager
    {
        private readonly string _key = "晒しあい あいチクリ合い 白黒に塗れテレビの中 もがいている＜ヒュー ヒュー　ヒューマノイド＞日に日に増大する肥えたアイデンティティそびえ立った ネオンの塔＜ヒューヒューマン＞";

        private string Encrypt(string s)
        {
            StringBuilder str = new();
            for (var i = 0; i < s.Length; i++)
            {
                str.Append((char)(s[i] ^ _key[i % _key.Length]));
            }
            return str.ToString();
        }

        private static PersistencyManager _instance;
        public static PersistencyManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log($"[PER] Persistency Manager created, data will be saved at {Application.persistentDataPath}/save.bin");
                    _instance = new();
                }
                return _instance;
            }
        }

        private SaveData _saveData;
        public SaveData SaveData
        {
            get
            {
                if (_saveData == null)
                {
                    if (File.Exists($"{Application.persistentDataPath}/save.bin"))
                    {
                        _saveData = JsonConvert.DeserializeObject<SaveData>(Encrypt(File.ReadAllText($"{Application.persistentDataPath}/save.bin")));
                    }
                    else
                    {
                        _saveData = new();
                    }
                }
                return _saveData;
            }
        }

        public void Save()
        {
            File.WriteAllText($"{Application.persistentDataPath}/save.bin", Encrypt(JsonConvert.SerializeObject(_saveData)));
        }

        public void DeleteSaveFolder()
        {
            _saveData = new();
            File.Delete($"{Application.persistentDataPath}/save.bin");
        }
    }
}