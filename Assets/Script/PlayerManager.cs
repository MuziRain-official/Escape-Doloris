using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerManager : MonoBehaviour
{
    public AssetReference playerPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerPrefab.InstantiateAsync().Completed += (handle) =>
        {
            print("Player instantiated");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
