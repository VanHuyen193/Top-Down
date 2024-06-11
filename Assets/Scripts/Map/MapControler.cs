using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControler : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;  //Bán kính để kiểm tra chunk.
    public LayerMask terrainMask;
    public GameObject currentChunk;
    PlayerMovement pm;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject lastestChunk;
    public float maxOpDist;
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDur;

    Vector3 right;
    Vector3 left;
    Vector3 up;
    Vector3 down;
    Vector3 rightUp;
    Vector3 rightDown;
    Vector3 leftUp;
    Vector3 leftDown;

    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        if (pm.moveDir.y != 0 || pm.moveDir.x != 0)
        {
            right = currentChunk.transform.Find("Right").position;
            left = currentChunk.transform.Find("Left").position;
            up = currentChunk.transform.Find("Up").position;
            down = currentChunk.transform.Find("Down").position;
            rightUp = currentChunk.transform.Find("Right Up").position;
            rightDown = currentChunk.transform.Find("Right Down").position;
            leftUp = currentChunk.transform.Find("Left Up").position;
            leftDown = currentChunk.transform.Find("Left Down").position;

            if (!Physics2D.OverlapCircle(up, checkerRadius, terrainMask))
            {
                SpawnChunk(up);
            }
            if (!Physics2D.OverlapCircle(down, checkerRadius, terrainMask))
            {
                SpawnChunk(down);
            }
            if (!Physics2D.OverlapCircle(right, checkerRadius, terrainMask))
            {
                SpawnChunk(right);
            }
            if (!Physics2D.OverlapCircle(left, checkerRadius, terrainMask))
            {
                SpawnChunk(left);
            }
            if (!Physics2D.OverlapCircle(rightUp, checkerRadius, terrainMask))
            {
                SpawnChunk(rightUp);
            }
            if (!Physics2D.OverlapCircle(leftUp, checkerRadius, terrainMask))
            {
                SpawnChunk(leftUp);
            }
            if (!Physics2D.OverlapCircle(rightDown, checkerRadius, terrainMask))
            {
                SpawnChunk(rightDown);
            }
            if (!Physics2D.OverlapCircle(leftDown, checkerRadius, terrainMask))
            {
                SpawnChunk(leftDown);
            }
        }
    }

    void SpawnChunk(Vector3 positionToSpawn)
    {

        int rand = Random.Range(0, terrainChunks.Count);
        lastestChunk = Instantiate(terrainChunks[rand], positionToSpawn, Quaternion.identity);
        spawnedChunks.Add(lastestChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDur;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            // Nếu khoảng cách lớn hơn hoặc bằng opDist sẽ tắt chunk đó và ngược lại
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if(opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
