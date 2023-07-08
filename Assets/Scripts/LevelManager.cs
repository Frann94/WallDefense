using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] enemies;
    GameObject superiorSpawnerPoint;
    GameObject inferiorSpawnerPoint;
    float spawnTime = 2;
    float spawnCount = 0;
    GameObject player;
    Camera camera;
    float playingTime = 0f;
    int inferiorArrayLimit = 0;
    int superiorArrayLimit = 0;
    float roundInterval = 30f;
    float roundTimer;
    int round = 1;
    int spawnAmount = 1;
    int LevelNumber;
    [SerializeField]
    TMPro.TextMeshProUGUI levelTime;

    // Start is called before the first frame update
    void Start()
    {
        roundTimer = roundInterval;
        camera = Camera.main;
        float xPos = camera.ScreenToWorldPoint(new Vector3(Screen.width + 5, 0, 0)).x;
        float yPos = camera.ScreenToWorldPoint(new Vector3(0, Screen.height-10, 0)).y;

        inferiorSpawnerPoint = new GameObject();
        inferiorSpawnerPoint.transform.position = new Vector3(xPos, -yPos, 0);
        superiorSpawnerPoint = new GameObject();
        superiorSpawnerPoint.transform.position = new Vector3(xPos, yPos, 0);
    }

    // Update is called once per frame
    void Update()
    {
        /*levelTime.text = playingTime.ToString();*/
        playingTime += Time.deltaTime;
        roundTimer -= Time.deltaTime;
        spawnCount -= Time.deltaTime;

        if (roundTimer <= 0) {
            AdvanceRound();
            roundTimer = roundInterval;
        }

        if (spawnCount <= 0)
        {
            Spawn();
            spawnCount = spawnTime;
        }
       
    }

    void AdvanceRound() {
        if (round >= 3) {
            Application.Quit();
        }
        else{
            round += 1;
            superiorArrayLimit += 1;
            spawnAmount += 1;
        }
    }

    void Spawn() {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject enemy = enemies[Random.Range(inferiorArrayLimit, superiorArrayLimit+1)];
            Vector3 pos = new Vector3(superiorSpawnerPoint.transform.position.x,
                                      Random.Range(inferiorSpawnerPoint.transform.position.y, superiorSpawnerPoint.transform.position.y),
                                      transform.position.z);

            Instantiate(enemy, pos, Quaternion.identity);
        }
    }
}
