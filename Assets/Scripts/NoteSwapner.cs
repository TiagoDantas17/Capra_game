using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject[] arrowPrefabs; 
    public Transform[] laneButtons;    
    public Transform noteHolder;       

    [Header("Спавн и повторы")]
    public float spawnY = 6f;       
    public float spawnInterval = 0.8f;
    public int maxRepeats = 2;        

    [Header("Плавное ускорение")]
    public float minSpawnInterval = 0.35f;       
    public float intervalDecreasePerSecond = 0.005f; 
    public float speedIncreasePerSecond = 0.04f;  
    public float maxFallSpeed = 7f;              

    private float timer;
    private int lastIndex = -1;
    private int repeatCount = 0;

    void Update()
    {
        if (GameManager.instance == null || !GameManager.instance.startPlaying)
            return;

        if (!GameManager.instance.theMusic.isPlaying)
            return;

        if (spawnInterval > minSpawnInterval)
        {
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - intervalDecreasePerSecond * Time.deltaTime);
        }

        if (GameManager.instance.theBS != null && GameManager.instance.theBS.beatTempo < maxFallSpeed)
        {
            GameManager.instance.theBS.beatTempo += speedIncreasePerSecond * Time.deltaTime;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomNote();
        }
    }

    void SpawnRandomNote()
    {
        if (arrowPrefabs.Length == 0 || laneButtons.Length != arrowPrefabs.Length)
            return;

        int index = Random.Range(0, arrowPrefabs.Length);

        if (arrowPrefabs.Length > 1 && index == lastIndex && repeatCount >= maxRepeats)
        {
            while (index == lastIndex)
            {
                index = Random.Range(0, arrowPrefabs.Length);
            }
        }

        if (index == lastIndex)
        {
            repeatCount++;
        }
        else
        {
            lastIndex = index;
            repeatCount = 1;
        }

        Vector3 spawnPos = new Vector3(laneButtons[index].position.x, spawnY, 0f);
        Instantiate(arrowPrefabs[index], spawnPos, arrowPrefabs[index].transform.rotation, noteHolder);
        GameObject newNote = Instantiate(arrowPrefabs[index], spawnPos, arrowPrefabs[index].transform.rotation, noteHolder);
        newNote.SetActive(true);
    }
}