using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOLevel : ScriptableObject
{
    public LevelManager_Runner levelManager { get; private set; }
    
    public bool isRandomLevel;
    public List<GameObject> levelPieces;
    public int numberOfPieces;
    public List<GameObject> currentLevelPieces;

    void Awake()
    {
        /* GameObject levelManagerObject = GameObject.Find("LevelManager");
        levelManager = levelManagerObject.GetComponent<LevelManager_Runner>(); */
    }

    private void OnEnable()
    {
        /* if (isRandomLevel)
        {
            SpawnLevelPieces();
        } */
    }

    public void SpawnLevelPieces()
    {
        for (int i = 0; i < numberOfPieces; i ++)
        {
            var piece = Instantiate(levelPieces[Random.Range(0, levelManager.levelPieces.Count)], levelManager.levelContainer);
            
            currentLevelPieces.Add(piece);

            if (i == 0)
            {
                piece.transform.localPosition = levelManager.startPiecePoint;
                //piece.transform.SetParent(gameObject.transform, true);
            }
            else if (i > 0)
            {
                //piece.transform.SetParent(gameObject.transform, true);
                
                var startPosition = currentLevelPieces[i - 1].GetComponent<LevelPieceController>().endPoint.transform.position;
                piece.transform.localPosition = startPosition;
            }
        }
    }
} 

