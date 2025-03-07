using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSaveData", menuName = "ScriptableObjects/GameSaveData")]
public class GameSaveData : ScriptableObject
{
    [Header("Player Data")]
    public int _playerAppearance;

    [Header("Game Data")]
    public int currentLevel;
    public bool isGamePaused;

    [Header("Settings Data")]
    public float _mouseSensitivity;
    
    public float _masterVolume;
    public float _musicVolume;
    public float _sfxVolume;
}
