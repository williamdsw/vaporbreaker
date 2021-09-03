using Controllers.Core;
using Luminosity.IO;
using MVC.Global;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Shooter : MonoBehaviour
{
    // Config
    [SerializeField] private GameObject[] cannons;
    [SerializeField] private Projectile[] projectiles;
    [SerializeField] private Transform[] shootingPoints;

    // Object pooling
    private List<GameObject> projectilesList = new List<GameObject>();
    private int numberOfObjects = 20;

    // Cached
    private AudioController audioController;
    private GameSession gameSession;
    private Paddle paddle;

    private void Start()
    {
        audioController = FindObjectOfType<AudioController>();
        gameSession = FindObjectOfType<GameSession>();
        paddle = FindObjectOfType<Paddle>();

        CreateProjectilesPool();
        DefineCannonsPosition();
    }

    private void Update()
    {
        if (gameSession.GetActualGameState() == GameState.GAMEPLAY)
        {
            if (InputManager.GetButtonDown(Configuration.InputsNames.Shoot))
            {
                Shoot();
            }
        }
    }

    private void CreateProjectilesPool()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            int index = UnityEngine.Random.Range(0, projectiles.Length);
            Projectile projectile = Instantiate(projectiles[index]);
            projectile.gameObject.SetActive(false);
            projectile.transform.SetParent(gameSession.FindOrCreateObjectParent(NamesTags.ProjectilesParentName).transform);
            projectilesList.Add(projectile.gameObject);
        }
    }

    // Shoots projectiles in shooting points
    private void Shoot()
    {
        if (shootingPoints.Length == 0) return;

        foreach (Transform point in shootingPoints)
        {
            for (int i = 0; i < projectilesList.Count; i++)
            {
                if (!projectilesList[i].activeInHierarchy)
                {
                    projectilesList[i].transform.SetPositionAndRotation(point.position, projectilesList[i].transform.rotation);
                    projectilesList[i].SetActive(true);
                    projectilesList[i].GetComponent<Projectile>().MoveProjectile();
                    break;
                }
            }
        }

        audioController.PlaySFX(audioController.LaserPewSound, 0.7f);
    }

    public void DefineCannonsPosition()
    {
        if (cannons.Length == 0) return;

        // Get components
        SpriteRenderer paddleSR = paddle.GetComponent<SpriteRenderer>();
        SpriteRenderer cannonSR = cannons[0].GetComponent<SpriteRenderer>();

        // Calculates
        float leftCannonX = (paddleSR.bounds.min.x + cannonSR.bounds.extents.x + 0.03f);
        float rightCannonX = (paddleSR.bounds.max.x - cannonSR.bounds.extents.x - 0.03f);
        float positionY = paddleSR.bounds.max.y;

        cannons[0].transform.position = new Vector3(leftCannonX, positionY, 1f);
        cannons[1].transform.position = new Vector3(rightCannonX, positionY, 1f);
    }
}