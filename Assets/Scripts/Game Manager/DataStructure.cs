using System.Collections.Generic;
using System;

[Serializable]
public class Statistic
{
    public string ID;
    public int Score;
    public float Lunc;
    public float Frg;
    public int CreatedAt;
}

[Serializable]
public class UserResponse
{
    public string ID;
    public string Username;
    public int CreatedAt;
    public int UpdatedAt;
}

[Serializable]
public class UserAuthResponse
{
    public string AccessToken;
    public string RefreshToken;
}

[Serializable]
public class Achievement
{
    public string UserID;
    public int LastLevel;
    public int PlayerLevel;
    public int PlayerExp;
}

public class CharacterInfo
{
    public Character SelectedCharacter { get; set; }
    public Dictionary<Character, int> OwnedCharacters { get; set; } = new Dictionary<Character, int>();
}

public class OwnedCharacterInformation
{
    public Character Character { get; set; }
    public int Level { get; set; }
}

[Serializable]
public class LeaderboardResponse
{
    public int Score;
    public string Username;
    public string Address;
}

public class LeaderboardData
{
    public LeaderboardResponse[] data;
    public object errors;
    public string message;
    public int code;
}