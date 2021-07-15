using UnityEngine;

public enum SpawnType
{
    Player,
    Goblin,
    Skeleton,
    Boss
}

public class SpawnPoint : MonoBehaviour
{
    public SpawnType spawnType;

    void Awake()
    {
        string spawnPrefabName; //스폰되는 프리팹 이름
        switch (spawnType)
        {
            case SpawnType.Player: spawnPrefabName = "Player";
                break;
            case SpawnType.Goblin: spawnPrefabName = "Goblin";
                break;
            case SpawnType.Skeleton: spawnPrefabName = "Skeleton";
                break;
            case SpawnType.Boss: spawnPrefabName = "Boss";
                break;
            default: spawnPrefabName = ""; //최적화 측면에서 스위치문 안에 할당하는 것이 좋다
                break; //default도 break필요
        }
        Instantiate(Resources.Load(spawnPrefabName), transform.position, Quaternion.identity);
    }
}
