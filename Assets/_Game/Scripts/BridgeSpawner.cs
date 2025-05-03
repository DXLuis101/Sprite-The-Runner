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

    int segsOnScreen = 5;
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
                //segment.segType = (enType)Random.Range(0, 3);
                if(segment.segType == enType.Straight) { segment.segPrefab = bridgePrefabs[Random.Range(0, 6)]; }
                else if(segment.segType == enType.L_Corner) { segment.segPrefab = bridgePrefabs[7]; }
                else if(segment.segType == enType.R_Corner) { segment.segPrefab = bridgePrefabs[6]; }
                break;
        }
    }

    void SpawnSegments()
    {
        GameObject prefabToInstantiate = segment.segPrefab;
        Quaternion prefabRotation = Quaternion.identity;

        switch (segCurrentDirection)
        {
            case enDirection.North:
                if(segment.segType == enType.Straight) { prefabRotation = Quaternion.Euler(000, 000, 000); segNextDirection = enDirection.North; spawnCord.z += 6.0f;}
                break;
        }

        if(prefabToInstantiate != null)
        {
            GameObject spawnedPrefab = Instantiate(prefabToInstantiate, spawnCord, prefabRotation) as GameObject;
            activeSegments.Add(spawnedPrefab);
            spawnedPrefab.transform.parent = this.transform;
        }

        segCurrentDirection = segNextDirection;
    }

    void RemoveSegments()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }
    void PlayerTrigger()
    {
        GameObject go = activeSegments[0];

        if(Vector3.Distance(playerTransform.position, go.transform.position) > 12.0f)
        {
            CreateSegments();
            SpawnSegments();
            RemoveSegments();
        }
    }
}
