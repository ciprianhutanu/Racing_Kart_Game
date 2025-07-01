using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerKart;
    public GameObject kartAI;
    public Material []materials;
    public RacerProgress progress;

    private Vector3 AISpawn = new Vector3(-24.52f, 0.21f, -23.46f);

    private void Start()
    {
        if (playerKart != null)
        {
            int savedIndex = PlayerPrefs.GetInt("KartColor", 0); 
            Renderer kartRenderer = playerKart.GetComponent<Renderer>();

            if (kartRenderer != null && materials.Length > savedIndex)
            {
                kartRenderer.material = materials[savedIndex];
            }
        }

        if(progress != null)
        {
            string savedName = PlayerPrefs.GetString("Name", "Player");
            progress.racerName = savedName;
        }

        kartAI.transform.position = AISpawn;
    }
}
