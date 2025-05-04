using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject[] bridgePrefabs;

    enum enType
    {
        L_Corner,
        Straight,
        R_Corner,
    }

    enum enDirection
    {
        North,
        East,
        West,
    }

    class Segments
    {
        public GameObject segPrefab;
        public enType segType;

        public Segments(GameObject segPrefab, enType segType)
        {
            this.segPrefab = segPrefab;
            this.segType = segType;
        }
    };

    List<GameObject> activeSegments = new List<GameObject>();
    Segments segment;
    Vector3 spawnCord = new Vector3(0, 0, -6);
    enDirection segCurrentDirection = enDirection.North;
    enDirection segNextDirection = enDirection.North;
    Transform playerTransform;

    float segLength = 6.0f;
    float segWidth = 3.0f;

    int segsOnScreen = 5;
    bool stopGame = false;

    // Start is called before the first frame update
    void Start()
    {
        segment = new Segments(bridgePrefabs[0], enType.Straight);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeSegments();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerTrigger();
    }
    void InitializeSegments()
    {
        for (int i = 0; i < segsOnScreen; i++)
        {
            SpawnSegments();
        }
    }
    void CreateSegments()
    {
        switch (segCurrentDirection)
        {
            case enDirection.North:
                segment.segType = (enType)Random.Range(0, 3);
                if(segment.segType == enType.Straight) { segment.segPrefab = bridgePrefabs[Random.Range(0, 6)]; }
                else if(segment.segType == enType.L_Corner) { segment.segPrefab = bridgePrefabs[7]; }
                else if(segment.segType == enType.R_Corner) { segment.segPrefab = bridgePrefabs[6]; }
                break;
            case enDirection.East:
                segment.segType = (enType)Random.Range(0, 2);
                if(segment.segType == enType.Straight) { segment.segPrefab = bridgePrefabs[Random.Range(0, 6)]; }
                else if (segment.segType == enType.L_Corner) { segment.segPrefab = bridgePrefabs[7]; }
                break;
            case enDirection.West:
                segment.segType = (enType)Random.Range(1, 3);
                if (segment.segType == enType.Straight) { segment.segPrefab = bridgePrefabs[Random.Range(0, 6)]; }
                else if (segment.segType == enType.R_Corner) { segment.segPrefab = bridgePrefabs[6]; }
                break;
        }
    }

    void SpawnSegments()
    {
        GameObject prefabToInstantiate = segment.segPrefab;
        Quaternion prefabRotation = Quaternion.identity;
        Vector3 offset = new Vector3(0, 0, 0);

        switch (segCurrentDirection)
        {
            case enDirection.North:
                if (segment.segType == enType.Straight) { prefabRotation = Quaternion.Euler(000, 000, 000); segNextDirection = enDirection.North; spawnCord.z += segLength; }
                else if (segment.segType == enType.R_Corner) { prefabRotation = Quaternion.Euler(000, 000, 000); segNextDirection = enDirection.East; spawnCord.z += segLength; offset.z += segLength + segWidth / 2; offset.x += segWidth / 2; }
                else if (segment.segType == enType.L_Corner) { prefabRotation = Quaternion.Euler(000, 000, 000); segNextDirection = enDirection.West; spawnCord.z += segLength; offset.z += segLength + segWidth / 2; offset.x -= segWidth / 2; }
                break;

            case enDirection.East:
                if (segment.segType == enType.Straight) { prefabRotation = Quaternion.Euler(000, 090, 000); segNextDirection = enDirection.East; spawnCord.x += segLength; }
                else if (segment.segType == enType.L_Corner) { prefabRotation = Quaternion.Euler(000, 090, 000); segNextDirection = enDirection.North; spawnCord.x += segLength; offset.z += segWidth / 2; offset.x += segLength + segWidth / 2; }
                break;

            case enDirection.West:
                if (segment.segType == enType.Straight) { prefabRotation = Quaternion.Euler(000,-090, 000); segNextDirection = enDirection.West; spawnCord.x -= segLength; }
                else if (segment.segType == enType.R_Corner) { prefabRotation = Quaternion.Euler(000,-090, 000); segNextDirection = enDirection.North; spawnCord.x -= segLength; offset.z += segWidth / 2; offset.x -= segLength + segWidth / 2; }
                break;
        }

        if(prefabToInstantiate != null)
        {
            GameObject spawnedPrefab = Instantiate(prefabToInstantiate, spawnCord, prefabRotation) as GameObject;
            activeSegments.Add(spawnedPrefab);
            spawnedPrefab.transform.parent = this.transform;
        }

        segCurrentDirection = segNextDirection;
        spawnCord += offset;
    }

    void RemoveSegments()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }
    void PlayerTrigger()
    {
        if (stopGame)
            return;

        GameObject go = activeSegments[0];

        if (Mathf.Abs(Vector3.Distance(playerTransform.position, go.transform.position)) > 15.0f)
        {
            CreateSegments();
            SpawnSegments();
            RemoveSegments();
        }
    }

    public void CleanTheScene()
    {
        stopGame = true;
        for(int j = activeSegments.Count -1; j>=0; j--)
        {
            Destroy(activeSegments[j]);
            activeSegments.RemoveAt(j);
        }

        spawnCord = new Vector3(0, 0, -6);
        segCurrentDirection = enDirection.North;
        segNextDirection = enDirection.North;
        segment = new Segments(bridgePrefabs[0], enType.Straight);
        InitializeSegments();
        stopGame = false;
    }
}
