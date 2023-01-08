using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    SpriteRenderer playerRenderer;
    public GameObject levelManager;
    public GameObject currentSoil;
    public float moveCooldownTimer = -1.0f;
    public bool isMoving = false;

    public bool isWateringAllowed;
    public bool isFertilizingAllowed;
    public bool isPlantingAllowed;

    public Vector2 startPosition;
    public Vector2 targetPosition;
    public Vector2 arcControlPosition;
    public float arcHeight = 0.5f;

    public Vector2 vectorA;
    public Vector2 vectorB;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<SpriteRenderer>();
        CheckAllowedActions();
    }

    void Update()
    {
        if (moveCooldownTimer >= 0) {
            moveCooldownTimer -= Time.deltaTime;
            vectorA = Vector2.Lerp(startPosition, arcControlPosition, moveCooldownTimer);
            vectorB = Vector2.Lerp(arcControlPosition, targetPosition, moveCooldownTimer);
            transform.position = Vector2.Lerp(vectorA, vectorB, moveCooldownTimer);

            if (moveCooldownTimer < 0) {
                // float movement = Input.GetAxis("Horizontal");
                // if (movement > 0.5) {
                //     Move(ArrowDirection.Right);
                // } else if (movement < -0.5) {
                //     Move(ArrowDirection.Left);
                // }
            }
        } else {
            float movement = Input.GetAxis("Horizontal");
            if (movement > 0.5) {
                Move(ArrowDirection.Right);
            } else if (movement < -0.5) {
                Move(ArrowDirection.Left);
            }
        }
    }

    // void FixedUpdate() {
    //     if (moveCooldownTimer >= 0) {
    //         playerRigidbody.MovePosition(Vector2.Lerp(vectorA, vectorB, moveCooldownTimer));
    //     }
    // }

    void Move(ArrowDirection arrowDirection)
    {
        if (levelManager.GetComponent<LevelManager>().actionsLeft > 0) {
            isWateringAllowed = false;
            isFertilizingAllowed = false;
            isPlantingAllowed = false;
            moveCooldownTimer = 0.5f;

            int currentSpaceLayer = LayerMask.NameToLayer("Current Space");
            int soilLayer = LayerMask.NameToLayer("Soil");
            int layerMask = (1 << soilLayer);
            Vector2 direction = new Vector2(1.0f, 0.0f);
            if (arrowDirection == ArrowDirection.Left) {
                direction = new Vector2(-1.0f, 0.0f);
            }
            RaycastHit2D hit = Physics2D.Raycast(playerRigidbody.position, direction, 1.0f, layerMask);
            if (hit.collider != null)
            {
                startPosition = playerRigidbody.position;
                targetPosition = new Vector2(playerRigidbody.position.x + direction.x, playerRigidbody.position.y);
                arcControlPosition = new Vector2(playerRigidbody.position.x + (direction.x/2), arcHeight);

                if (arrowDirection == ArrowDirection.Left) {
                    playerRenderer.flipX = false;
                } else if (arrowDirection == ArrowDirection.Right) {
                    playerRenderer.flipX = true;
                }

                // transform.position = new Vector2(hit.transform.position.x, playerRigidbody.position.y);
                currentSoil.layer = soilLayer;
                currentSoil = hit.transform.gameObject;
                currentSoil.layer = currentSpaceLayer;
                levelManager.GetComponent<LevelManager>().OnMove();

                CheckAllowedActions();
            } else {
                moveCooldownTimer = -1.0f;
                CheckAllowedActions();
            }
        }
    }

    public void CheckAllowedActions() {
        if (
            currentSoil.GetComponent<SoilController>().ownPlant != null
            && currentSoil.GetComponent<SoilController>().ownPlant.GetComponent<PlantController>().waterNeeded > 0
        ) {
            isWateringAllowed = true;
        } else {
            isWateringAllowed = false;
        }

        if (
            currentSoil.GetComponent<SoilController>().ownPlant != null
            && currentSoil.GetComponent<SoilController>().ownPlant.GetComponent<PlantController>().fertilizerNeeded > 0
        ) {
            isFertilizingAllowed = true;
        } else {
            isFertilizingAllowed = false;
        }

        if (currentSoil.GetComponent<SoilController>().ownPlant == null) {
            isPlantingAllowed = true;
        } else {
            isPlantingAllowed = false;
        }
    }
}