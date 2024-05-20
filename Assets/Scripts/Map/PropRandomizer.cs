using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints;
    public List<GameObject> propPrefabs;
    bool isPropsSpawned = false;
    void Start()
    {
        if (gameObject.activeSelf && !isPropsSpawned)
        {
            SpawnProps();
            isPropsSpawned = true;
        }
    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnPoints)
        {
            int rand = Random.Range(0, propPrefabs.Count);
            GameObject prop = Instantiate(propPrefabs[rand], sp.transform.position, Quaternion.identity);
            
            // Gán prop mới tạo là con của propSpawnPoints
            prop.transform.parent = sp.transform;
        }
    }
}
