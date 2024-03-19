using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private GameObject _superiorSpawnerPoint;
    [SerializeField] private GameObject _inferiorSpawnerPoint;
    [SerializeField] private TextMeshProUGUI _levelTimeText;
    [SerializeField] private TextMeshProUGUI _pointsText;

    public int TotalPointsEarned { get; private set; }
    public int Points { get; set; }

    private float _spawnTime = 2f;
    private float _spawnCount = 10f;
    private float _playingTime = 0f;
    private float _roundInterval = 30f;
    private float _roundTimer;
    private int _spawnAmount = 1;
    private int _inferiorArrayLimit = 0;
    private int _superiorArrayLimit = 0;
    private int _round;
    private float _pointsIncreaseTimer = 0f;

    private void Start()
    {
        ResetGame();
        _roundTimer = _roundInterval;
    }

    private void ResetGame()
    {
        _spawnCount = 10f;
        _playingTime = 0f;
        _round = 1;
        _spawnAmount = 1;
        _superiorArrayLimit = 0;
        Points = 0;
        UpdatePointsUI();
    }

    public void UpdatePointsUI()
    {
        _pointsText.text = $"Points: {Points}";
    }

    private void Update()
    {
        _playingTime += Time.deltaTime;
        _roundTimer -= Time.deltaTime;
        _spawnCount -= Time.deltaTime;
        UpdateLevelTimeUI();
        _pointsIncreaseTimer += Time.deltaTime;

        if (_pointsIncreaseTimer >= 1f)
        {
            IncrementPoints(1);
            _pointsIncreaseTimer = 0f;
        }

        if (_roundTimer <= 0)
        {
            AdvanceRound();
            _roundTimer = _roundInterval;
        }

        if (_spawnCount <= 0)
        {
            Spawn();
            _spawnCount = _spawnTime;
        }
    }

    private void UpdateLevelTimeUI()
    {
        int minutes = Mathf.FloorToInt(_playingTime / 60f);
        int seconds = Mathf.FloorToInt(_playingTime % 60f);
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        _levelTimeText.text = $"Timer: {formattedTime}";
    }

    public void IncrementPoints(int amount)
    {
        Points += amount;
        TotalPointsEarned += amount;
        UpdatePointsUI();
    }

    private void AdvanceRound()
    {
        _round++;
        if (_enemies.Length>_superiorArrayLimit + 1)
        {
            _superiorArrayLimit++;
        }
        _spawnAmount++;
    }

    private void Spawn()
    {
        for (int i = 0; i < _spawnAmount; i++)
        {
            GameObject enemy = _enemies[Random.Range(_inferiorArrayLimit, _superiorArrayLimit)];
            Vector3 pos = new Vector3(_superiorSpawnerPoint.transform.position.x,
                                      Random.Range(_inferiorSpawnerPoint.transform.position.y, _superiorSpawnerPoint.transform.position.y),
                                      transform.position.z);

            Instantiate(enemy, pos, Quaternion.identity);
        }
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(_playingTime / 60f);
        int seconds = Mathf.FloorToInt(_playingTime % 60f);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
