using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Fish fishPrefab;
    [SerializeField] private Fish.FishType[] fishTypes;

    void Awake()
    {
        for (int i = 0; i < fishTypes.Length; i++)
        {
            for (int j = 0; j < fishTypes[i].fishCount; j++)
            {
                Fish fish = Instantiate<Fish>(fishPrefab);
                fish.Type = fishTypes[i];
                fish.ResetFish();
            }
        }
    }

}
