using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public LevelManager_Runner levelManager { get; private set; }
    
    public bool isRandomLevel;
    public int numberOfPieces;
    public List<GameObject> currentLevelPieces;
    public ItemBase_Coin[] currentCoins;

    [Header("Art")]
    public LevelManager_Runner.ArtType artType;

    void Awake()
    {
        GameObject levelManagerObject = GameObject.Find("LevelManager");
        levelManager = levelManagerObject.GetComponent<LevelManager_Runner>();
    }

    void Start()
    {
        if (isRandomLevel)
        {
            SpawnLevelPieces();
        }
        else
        {
            levelManager.SpawnCoins();
        }
    }

    public void SpawnLevelPieces()
    {
        for (int i = 0; i < numberOfPieces; i ++)
        {
            var piece = Instantiate(levelManager.levelPieces[Random.Range(0, levelManager.levelPieces.Count)], levelManager.levelContainer);
            
            currentLevelPieces.Add(piece);

            if (i == 0)
            {
                piece.transform.localPosition = levelManager.startPiecePoint;
                piece.transform.SetParent(gameObject.transform, true);
            }
            else if (i > 0)
            {
                piece.transform.SetParent(gameObject.transform, true);
                
                var startPosition = currentLevelPieces[i - 1].GetComponent<LevelPieceController>().endPoint.transform.position;
                piece.transform.localPosition = startPosition;
            }
        }

        currentCoins = GetComponentsInChildren<ItemBase_Coin>();
        levelManager.SpawnCoins();
    }
}
