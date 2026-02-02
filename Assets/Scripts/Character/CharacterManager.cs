using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public PlayerSettings settings; // Drag your SO here

    [Header("Models")]
    [SerializeField] private List<GameObject> characterModels;

    [Header("Materials")]
    [SerializeField] private List<Material> availableMaterials;

    void Start()
    {
        // Load the saved state when the scene starts
        UpdateAppearance();
    }
    public void CycleCharacter(int direction)
    {
        int nextIndex = settings.selectedCharacterIndex + direction;

        // Loop back around if we go out of bounds
        if (nextIndex >= characterModels.Count) nextIndex = 0;
        if (nextIndex < 0) nextIndex = characterModels.Count - 1;

        ChangeSkin(nextIndex, settings.selectedMaterialIndex);
    }

    // Helper for Material Cycling
    public void CycleMaterial(int direction)
    {
        int nextIndex = settings.selectedMaterialIndex + direction;

        if (nextIndex >= availableMaterials.Count) nextIndex = 0;
        if (nextIndex < 0) nextIndex = availableMaterials.Count - 1;

        ChangeSkin(settings.selectedCharacterIndex, nextIndex);
    }
    public void ChangeSkin(int modelIndex, int matIndex)
    {
        settings.selectedCharacterIndex = modelIndex;
        settings.selectedMaterialIndex = matIndex;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        for (int i = 0; i < characterModels.Count; i++)
        {
            bool isActive = (i == settings.selectedCharacterIndex);
            characterModels[i].SetActive(isActive);

            if (isActive)
            {
                // Get the renderer of the active model and swap the material
                SkinnedMeshRenderer smr = characterModels[i].GetComponent<SkinnedMeshRenderer>();
                if (smr != null && availableMaterials.Count > settings.selectedMaterialIndex)
                {
                    smr.material = availableMaterials[settings.selectedMaterialIndex];
                }
            }
        }
    }
}