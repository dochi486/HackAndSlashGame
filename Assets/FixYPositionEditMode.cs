using UnityEngine;

[ExecuteInEditMode]
public class FixYPositionEditMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying) //플레이 중이면 항상 true로 나오는 문장!
            Destroy(gameObject);

    
    }

    // Update is called once per frame
    void Update()
    {

        var pos = transform.position;
        pos.y = 0;
        transform.position = pos;

    }
    public SpawnType spawnType;
    private void OnDrawGizmos()
    {
        spawnType = GetComponent<SpawnPoint>().spawnType;
        string spawnIconName; //스폰되는 프리팹 이름
        switch (spawnType)
        {
            case SpawnType.Player:
                spawnIconName = "Player";
                break;
            case SpawnType.Goblin:
                spawnIconName = "Goblin";
                break;
            case SpawnType.Skeleton:
                spawnIconName = "Skeleton";
                break;
            case SpawnType.Boss:
                spawnIconName = "Boss";
                break;
            default:
                spawnIconName = ""; //최적화 측면에서 스위치문 안에 할당하는 것이 좋다
                break; //default도 break필요
        }
        Gizmos.DrawIcon(transform.position, spawnIconName+".png", true);
    }

}
