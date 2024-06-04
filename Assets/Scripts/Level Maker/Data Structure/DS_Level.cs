using UnityEngine;

[System.Serializable]
public class Background
{
    public GameObject sky;
    public GameObject[] backgroundObstacle1;
    public GameObject[] backgroundObstacle2;
    public GameObject[] backgroundObstacle3;
}

[System.Serializable]
public class Land
{
    public GameObject[] leftEdgeLand;
    public GameObject[] fillerLand;
    public GameObject[] rightEdgeLand;
}

[System.Serializable]
public class Obstacle
{
    public GameObject[] destroyableObstacle;
    public GameObject[] staticObstacle;
}

[System.Serializable]
public class Foreground
{
    public Land land = new Land();
    public Obstacle obstacle = new Obstacle();
}
[System.Serializable]

public class AssetDataStructure
{
    public Background background = new Background();
    public Foreground foreground = new Foreground();
}
