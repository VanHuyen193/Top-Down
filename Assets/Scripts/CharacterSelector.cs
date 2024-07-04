using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    // Lưu trữ data đã chọn
    public CharacterData characterData;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
    }

    // Dùng Static để gọi trực tiếp từ chính class này
    public static CharacterData GetData()
    {
        if(instance && instance.characterData)
        {
            return instance.characterData;
        }
        else
        {
            // Randomly pick a character if we are playing from the editor
            #if UNITY_EDITOR
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            List<CharacterData> characters = new List<CharacterData>();
            foreach (string assetPath in allAssetPaths)
            {
                if (assetPath.EndsWith(".asset"))
                {
                    CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                    if(characterData != null)
                    {
                        characters.Add(characterData);
                    }
                }
            }

            // Pick a random character if we have found any character.
            if (characters.Count > 0) return characters[Random.Range(0, characters.Count)];
            #endif
        }
        return null;
    }


    // Chọn data của player từ button
    public void SelectCharacter(CharacterData character)
    {
        characterData = character;
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
