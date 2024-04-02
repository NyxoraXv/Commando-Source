using System.Collections.Generic;
using System;
using JetBrains.Annotations;

[Serializable]
public class StatisticData
{
    public Statistic data;
}

[Serializable]
public class Statistic
{
    public string id;
    public int score;
    public float lunc;
    public float frg;
    public long created_at;
}

[Serializable]
public class UserResponseData{
    public UserResponse data;
}

[Serializable]
public class UserResponse
{
    public string id;
    public string username;
    public string device;
    public long created_at;
    public long updated_at;
}

[System.Serializable]
public class NFTData
{
    public string owner;
    public string token_id;
    public string ask_price;
    public string ask_denom;
    public string name;
    public string image;
    public string video;
    public string rarity;
    public string boost;
    public string level;
    public string collection;
    public string mystery_pack;
}

public class AchievementData{
    public Achievement data;
}

[System.Serializable]
public class NFTResponse
{
    public bool status;
    public int total;
    public List<NFTData> data;
}

public class WalletAdressData
{
    public WalletAdressDetails data;
}

[Serializable]
public class WalletAdressDetails
{
    public string user_agent;
    public string address;
    public bool is_connected;
    public bool request_disconnected;
    public int created_at;
    public int updated_at;
}
public class AccessTokenResponse
{
    public UserAuthResponseData data;
}

[Serializable]
public class UserAuthResponseData
{
    public string access_token;
    public string refresh_token;
}

[Serializable]
public class Achievement
{
    public string id;
    public int last_level;
    public int player_level;
    public int player_exp;
    public long created_at;
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

[System.Serializable]
public class LeaderboardEntry
{
    public string username;
    public string user_id;
    public int score;
}

[System.Serializable]
public class LeaderboardData
{
    public LeaderboardEntry[] data;
}
