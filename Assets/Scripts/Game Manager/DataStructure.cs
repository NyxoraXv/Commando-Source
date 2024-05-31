using System.Collections.Generic;
using System;

[Serializable]
public class StatisticData
{
    public Statistic data;
}

[Serializable]
public class Statistic
{
    public int score;
    public float lunc;
    public float frg;
    public int last_level;
    public int level;
    public int exp;
}

public class transaction
{
    public string type;
    public int amount;
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
