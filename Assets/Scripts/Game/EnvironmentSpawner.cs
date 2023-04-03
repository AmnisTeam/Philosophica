using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Spawner
{
    public float minTimeToSpawn;
    public float maxTimeToSpawn;
    public float minSpeed;
    public float maxSpeed;
    public float minRotationSpeed;
    public float maxRotationSpeed;
    public GameObject prifab;
}

public class SpawnTimer
{
    public float timer;
    public float timeToSpawn;

    public SpawnTimer(float timer, float timeToSpawn)
    {
        this.timer = timer;
        this.timeToSpawn = timeToSpawn;
    }
}

public class EnvironmentSpawner : MonoBehaviour
{
    public Spawner[] spawners;
    private SpawnTimer[] timers;

    private void Start()
    {
        timers = new SpawnTimer[spawners.Length];
        for (int x = 0; x < spawners.Length; x++)
            timers[x] = new SpawnTimer(Random.Range(0, spawners[x].maxTimeToSpawn), Random.Range(spawners[x].minTimeToSpawn, spawners[x].maxTimeToSpawn));
    }

    private void Update()
    {
        for(int x = 0; x < spawners.Length; x++)
        {
            timers[x].timer += Time.deltaTime;
            if(timers[x].timer >= timers[x].timeToSpawn)
            {
                timers[x].timer = 0;
                timers[x].timeToSpawn = Random.Range(spawners[x].minTimeToSpawn, spawners[x].maxTimeToSpawn);

                int side = Random.Range(0, 4); //Начало - верх, по часовой стрелке

                float speed = Random.Range(spawners[x].minSpeed, spawners[x].maxSpeed);
                float rotationSpeed = Random.Range(spawners[x].minRotationSpeed, spawners[x].maxRotationSpeed);

                //Определение направления движения объекта
                float velocityAngle = Random.Range(0, Mathf.PI);
                Vector2 hemicircleDirection = new Vector3(Mathf.Cos(velocityAngle), Mathf.Sin(velocityAngle), 0);

                Vector2 normal = Vector2.zero;
                if (side == 0) normal = Vector2.up;
                else if (side == 1) normal = Vector2.right;
                else if (side == 2) normal = Vector2.down;
                else if (side == 3) normal = Vector2.left;
                Vector2 tangent = Vector3.Cross(normal, new Vector3(0, 0, 1));
                Vector2 direction = hemicircleDirection.x * tangent + hemicircleDirection.y * normal;

                //Размеры
                float cameraHeight = Camera.main.orthographicSize * 2f;
                float cameraWidth = cameraHeight * Screen.width / Screen.height;
                Vector2 cameraSize = new Vector2(cameraWidth, cameraHeight);

                Bounds spriteBounds = spawners[x].prifab.GetComponent<SpriteRenderer>().bounds;

                float spriteHeight = spriteBounds.max.y - spriteBounds.min.y;
                float spriteWidth = spriteBounds.max.x - spriteBounds.min.x;
                Vector2 spriteSize = new Vector2(spriteWidth, spriteHeight);

                //Локальная позиция
                Vector2 localPosition = -normal * (cameraSize / 2.0f + spriteSize / 2.0f) + tangent * (cameraSize / 2.0f - spriteSize / 2.0f) * Random.Range(0, 1);

                GameObject spawnedObject = Instantiate(spawners[x].prifab, Camera.main.transform.position.ToXY() + localPosition, Quaternion.Euler(0, 0, Random.Range(0, 2 * Mathf.PI)));
                
                //Задание физических параметров
                Rigidbody2D rigidbody = spawnedObject.GetComponent<Rigidbody2D>();
                rigidbody.velocity = direction * speed;
                rigidbody.angularVelocity = rotationSpeed;
            }
        }
    }
}
