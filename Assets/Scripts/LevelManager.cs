using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private GameObject _boss;
    [SerializeField] private GameObject _spawnPoint;
    [SerializeField] private TextMeshProUGUI _levelTimeText;
    [SerializeField] private TextMeshProUGUI _pointsText;

    public int TotalPointsEarned { get; private set; }
    public int Points { get; set; }

    private float _spawnTime = 7.5f;
    private float _spawnCount = 10f;
    private float _playingTime = 0f;
    private float _roundInterval = 20f;
    private float _roundTimer;
    private int _spawnAmount = 2;
    private int _bossAmount = 1;
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
        Points = 15;
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

        if (_pointsIncreaseTimer >= 2f)
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
        if (_enemies.Length>_superiorArrayLimit)
        {
            _superiorArrayLimit++;
        }
        _spawnAmount++;
        if (_round >= 4)
        {
            for (int i = 0; i < _bossAmount; i++)
            {
                Vector3 pos = new Vector3(_spawnPoint.transform.position.x+Random.Range(-2f,2f),
                    _spawnPoint.transform.position.y + Random.Range(-2f, 2f),
                    transform.position.z);
                Instantiate(_boss, pos, Quaternion.identity);
            }
            _bossAmount++;
            _spawnTime--;
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < _spawnAmount; i++)
        {
            GameObject enemy = _enemies[Random.Range(_inferiorArrayLimit, _superiorArrayLimit)];
            Vector3 pos = new Vector3(_spawnPoint.transform.position.x + Random.Range(-2f, 2f),
                    _spawnPoint.transform.position.y + Random.Range(-2f, 2f),
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
