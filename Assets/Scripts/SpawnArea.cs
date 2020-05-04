using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void SpawnEnemy(int amount, GameObject prefab)
    {
        for (int i = 0; i < amount; i++)
        {
            Utils.AddEnemy();
            Instantiate(
                prefab,
                new Vector3(Random.Range(rectTransform.rect.xMin, rectTransform.rect.xMax),
                            Random.Range(rectTransform.rect.yMin, rectTransform.rect.yMax),
                            0) + rectTransform.transform.position , Quaternion.identity);
        }
    }

    public void SpawnEnemies(SpawnInstruction spawn)
    {
        SpawnEnemy(spawn.Gangsters, Utils.Instance.GangsterEnemy);
        SpawnEnemy(spawn.Gamers, Utils.Instance.GamerEnemy);
        SpawnEnemy(spawn.Shotguns, Utils.Instance.ShotgunEnemy);
    }
}
