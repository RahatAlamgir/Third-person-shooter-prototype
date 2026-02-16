using System.Collections.Generic;
using UnityEngine;

public class ZombieCustomizer : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private List<GameObject> ZombieModels;

    [Header("Materials")]
    [SerializeField] private List<Material> availableMaterials;

    [Header("Weapon")]
    [SerializeField] private List<GameObject> availableWeapon;


    public void SetZombie(int modelIndex = 0, int materialsIndex = 0, int weaponIndex = 0)
    {
        // 1. Handle Models
        for (int i = 0; i < ZombieModels.Count; i++)
        {
            ZombieModels[i].SetActive(i == modelIndex);

            // Apply material to the active model
            if (i == modelIndex && availableMaterials.Count > materialsIndex)
            {
                // Note: Assumes the model has a Renderer on it or its first child
                Renderer rend = ZombieModels[i].GetComponent<Renderer>();
                if (rend != null) rend.material = availableMaterials[materialsIndex];
            }
        }

        // 2. Handle Weapons
        // weaponIndex = 0 is "No Weapon"
        for (int i = 0; i < availableWeapon.Count; i++)
        {
            // If weaponIndex is 1, it activates availableWeapon[0]
            // If weaponIndex is 0, it deactivates all
            availableWeapon[i].SetActive(i == (weaponIndex - 1));
        }
    }

    public void SetRandomZombie()
    {
        int randModel = Random.Range(0, ZombieModels.Count);
        int randMat = Random.Range(0, availableMaterials.Count);

        // 0 = no weapon, higher = weapon index
        int randWeapon = Random.Range(0, availableWeapon.Count + 1);

        SetZombie(randModel, randMat, randWeapon);
    }

}
