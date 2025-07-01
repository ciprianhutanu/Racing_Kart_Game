using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartColorSelector : MonoBehaviour
{
    public Material[] materials; 
    private int selectedMaterialIndex = 0;
    private string selectedName = "Player";

    public void SelectColor(int index)
    {
        if (index >= 0 && index < materials.Length)
        {
            selectedMaterialIndex = index;
            PlayerPrefs.SetInt("KartColor", selectedMaterialIndex);
        }
    }

    public void SelectName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            selectedName = name;
            PlayerPrefs.SetString("Name", selectedName);
        }
    }
}
