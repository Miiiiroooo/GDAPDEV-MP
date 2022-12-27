using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class AssetManager : MonoBehaviour
{

    public string gunshotAddress;
    private AsyncOperationHandle<AudioClip> handle_Audio;

    public string missileLaunchAddress;
    private AsyncOperationHandle<AudioClip> handle_MissleLaunch;

    public string explosionAddress;
    private AsyncOperationHandle<AudioClip> handle_Explosion;

    public string muzzleFlashAddress;
    private AsyncOperationHandle<GameObject> handle_GameObjects;

    public List<string> enemyAddresses;
    private AsyncOperationHandle<IList<GameObject>> handle_Enemies;

    public AudioClip gunshotClip;
    public AudioClip missileLaunchClip;
    public AudioClip explosionClip;
    public GameObject muzzleFlash;
    public List<GameObject> enemies;
    private int index = 0;

    public static AssetManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        handle_Audio = Addressables.LoadAssetAsync<AudioClip>(gunshotAddress);
        handle_Audio.Completed += Handle_Audio_Completed;

        handle_MissleLaunch = Addressables.LoadAssetAsync<AudioClip>(missileLaunchAddress);
        handle_MissleLaunch.Completed += Handle_MissileLaunch_Completed;

        handle_Explosion = Addressables.LoadAssetAsync<AudioClip>(explosionAddress);
        handle_Explosion.Completed += Handle_Explosion_Completed;

        handle_GameObjects = Addressables.LoadAssetAsync<GameObject>(muzzleFlashAddress);
        handle_GameObjects.Completed += Handle_GameObjects_Completed;

        handle_Enemies = Addressables.LoadAssetsAsync<GameObject>(enemyAddresses, LoadEnemies, Addressables.MergeMode.Union, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LoadEnemies(GameObject obj)
    {
        //Debug.Log("here");
        enemies.Add(obj);
    }

    private void Handle_Audio_Completed(AsyncOperationHandle<AudioClip> clip)
    {
        gunshotClip = clip.Result;
    }

    private void Handle_MissileLaunch_Completed(AsyncOperationHandle<AudioClip> clip)
    {
        missileLaunchClip = clip.Result;
    }

    private void Handle_Explosion_Completed(AsyncOperationHandle<AudioClip> clip)
    {
        explosionClip = clip.Result;
    }

    private void Handle_GameObjects_Completed(AsyncOperationHandle<GameObject> obj)
    {
        muzzleFlash = obj.Result;
    }

    private void OnDestroy()
    {
        Addressables.Release(handle_Audio);
        Addressables.Release(handle_GameObjects);
        Addressables.Release(handle_MissleLaunch);
        Addressables.Release(handle_Explosion);
    }
}
