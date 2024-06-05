using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]public AssetDataStructure levelStructure;
    public class ObjectStructure
    {
        public GameObject prefab;
        public Vector3 position;
    }

    public void generateLevel(int length = 0, int margin = 0)
    {
        print("level generated");
    }

    private ObjectStructure[] calculateObstacle(GameObject prefab, int length)
    {
        ObjectStructure[] objectStructureArray = new ObjectStructure[0];
        for (int i = 0; i<=length; i++)
        {
            ObjectStructure objectStructure = new ObjectStructure
            {

            };
            objectStructureArray.Append(objectStructure);
        };

        return objectStructureArray;
    }
}

