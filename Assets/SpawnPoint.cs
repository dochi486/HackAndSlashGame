using UnityEngine;

//[ExecuteInEditMode] 
//에디터에서만 실행되는 스크립트라는 뜻
//이걸 사용하면 플레이를 중단해도 스폰된 오브젝트가 남아있어서 삭제해야함..!

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
            case SpawnType.Player:
                spawnPrefabName = "Player";
                break;
            case SpawnType.Goblin:
                spawnPrefabName = "Goblin";
                break;
            case SpawnType.Skeleton:
                spawnPrefabName = "Skeleton";
                break;
            case SpawnType.Boss:
                spawnPrefabName = "Boss";
                break;
            default:
                spawnPrefabName = ""; //최적화 측면에서 스위치문 안에 할당하는 것이 좋다
                break; //default도 break필요
        }
        Instantiate(Resources.Load(spawnPrefabName), transform.position, Quaternion.identity);
    }


}
