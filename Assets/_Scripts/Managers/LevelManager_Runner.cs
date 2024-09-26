using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Unity.VisualScripting;

public class LevelManager_Runner : MonoBehaviour
{
    public PlayerController_Ball player { get; private set; }
    
    public Transform levelContainer;
    public List<GameObject> levels;
    public List<SOLevel> levels_SO;
    public ItemBase_Coin[] coins;
    public int levelIndex;
    public Vector3 levelPoint;
    public Vector3 startPiecePoint;
    public List<GameObject> levelPieces;


    private GameObject _currentLevel;

    private void Awake()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerController_Ball>();    

        SpawnNextLevel();
    } 

    private void Start()
    {
        
    }

    public void SpawnNextLevel()
    {
        /* if (levelContainer.childCount != 0)
        {
            levelContainer.MMDestroyAllChildren();    
        } */

        if (_currentLevel != null)
        {
            Destroy(_currentLevel);
            levelIndex ++;
        }

        if (levelIndex >= levels.Count)
        {
            ResetLevelIndex();
        }

        _currentLevel = Instantiate(levels[levelIndex], levelContainer);
        _currentLevel.transform.localPosition = levelPoint;

        ChangeColorByType(_currentLevel.GetComponent<LevelController>().artType);

        //StartCoroutine(SpawnCoins());

        player.transform.position = player.startPosition;
    }

    private void ResetLevelIndex()
    {
        levelIndex = 0;
    }

    public void SpawnCoins()
    {
        StartCoroutine(SpawnCoinsCoroutine());
    }

    private IEnumerator SpawnCoinsCoroutine()
    {
        if (_currentLevel.GetComponent<LevelController>().isRandomLevel)
        {
            coins = _currentLevel.GetComponent<LevelController>().currentCoins;
        }
        else
        {
            coins = _currentLevel.GetComponentsInChildren<ItemBase_Coin>();
        }
        
        // Sort coins by distance from the Player
        coins = coins.OrderBy(coin => Vector3.Distance(coin.transform.position, startPiecePoint)).ToArray();

        WaitForSeconds waitCoin = new WaitForSeconds(0.02f);

        foreach (var coin in coins)
        {
            coin.meshRenderer.enabled = true;
            coin.particleSystem_Aura.Play();
            MMF_Scale scale = coin.feedbacks.GetFeedbackOfType<MMF_Scale>();
            scale.Play(transform.position, 1);

            yield return waitCoin;
        }

        StopCoroutine(SpawnCoinsCoroutine()); 
        yield return null;
    }

    #region Art Manager

    public enum ArtType
    {
        Type_01,
        Type_02,
        Type_03
    }
    
    public List<ArtSetup> artSetups;

    public ArtSetup GetSetupByType(ArtType artType)
    {
        return artSetups.Find(i => i.artType == artType);
    }

    [Header("Color Manager")]
    public List<Material> materials;
    public List<ColorSetup> colorSetups;

    public void ChangeColorByType(LevelManager_Runner.ArtType artType)
    {
        var setup = colorSetups.Find(i => i.artType == artType);

        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetColor("_EmissionColor", setup.colors[i]);
        }
    }

    #endregion
}

[System.Serializable]
public class ArtSetup
{
    public LevelManager_Runner.ArtType artType;
    public GameObject gameObject;
}

[System.Serializable]
public class ColorSetup
{
    public LevelManager_Runner.ArtType artType;
    public List<Color> colors;
}
