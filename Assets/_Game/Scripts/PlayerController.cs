using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    enum enDirection { North, East, West};
    public AudioClip[] soundFXClips;
    public Text distanceScoreText;
    public Text coinScoreText;
    public Text bestDistanceScoreText;
    public Text bestCoinScoreText;
    public GameObject deathMenu;

    CharacterController characterController;
    Vector3 playerVector; // player's direction
    enDirection playerDirection = enDirection.North;
    enDirection playerNextDirection = enDirection.North;
    Animator anim;
    BridgeSpawner bridgeSpawner;
    AudioSource audioSource;

    int coinsCollected = 0;
    int coinsCollectedBest;
    int distanceRun = 0;
    int distanceRunBest;
    float playerStartSpeed = 10.0f;
    float playerSpeed; // player's speed
    float gValue = 10.0f;
    float translationfactor = 10.0f;
    float jumpForce = 1.5f;
    float timer = 0;
    float distance = 0;
    bool canTurnRight = false;
    bool canTurnLeft = false;
    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = playerStartSpeed;
        characterController = this.GetComponent<CharacterController>();
        anim = this.GetComponent<Animator>();
        bridgeSpawner = GameObject.Find("BridgeManager").GetComponent<BridgeSpawner>();
        audioSource = this.GetComponent<AudioSource>();
        playerVector = new Vector3(0, 0, 1) * playerSpeed * Time.deltaTime;
        deathMenu.SetActive(false);
        distanceRunBest = PlayerPrefs.GetInt("highscoreD");
        coinsCollectedBest = PlayerPrefs.GetInt("highscoreC");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLogic();
        distanceScoreText.text = "Distance: " + distanceRun.ToString();
        coinScoreText.text = "Coins: " + coinsCollected.ToString();
    }

    void PlayerLogic()
    {
        if (isDead)
            return;

        if (!characterController.enabled) { characterController.enabled = true; }
        timer += Time.deltaTime;

        playerSpeed += 0.1f * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && canTurnRight)
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.East;
                    this.transform.rotation = Quaternion.Euler(000, 090, 000);
                    break;
                case enDirection.West:
                    playerNextDirection = enDirection.North;
                    this.transform.rotation = Quaternion.Euler(000, 000, 000);
                    break;
            }

            audioSource.PlayOneShot(soundFXClips[6], 0.4f); ;
        }
        else if (Input.GetKeyDown(KeyCode.F) && canTurnLeft)
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.West;
                    this.transform.rotation = Quaternion.Euler(000, -090, 000);
                    break;
                case enDirection.East:
                    playerNextDirection = enDirection.North;
                    this.transform.rotation = Quaternion.Euler(000, 000, 000);
                    break;
            }

            audioSource.PlayOneShot(soundFXClips[6], 0.4f); ;
        }

        playerDirection = playerNextDirection;

        if(playerDirection == enDirection.North) { playerVector = Vector3.forward * playerSpeed * Time.deltaTime; }
        else if(playerDirection == enDirection.East) { playerVector = Vector3.right * playerSpeed * Time.deltaTime; }
        else if (playerDirection == enDirection.West) { playerVector = Vector3.left * playerSpeed * Time.deltaTime; }

        switch (playerDirection)
        {
            case enDirection.North:
                playerVector.x = Input.GetAxisRaw("Horizontal") * translationfactor * Time.deltaTime;
                break;
            case enDirection.East:
                playerVector.z = -Input.GetAxisRaw("Horizontal") * translationfactor * Time.deltaTime;
                break;
            case enDirection.West:
                playerVector.z = Input.GetAxisRaw("Horizontal") * translationfactor * Time.deltaTime;
                break;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && characterController.isGrounded)
        {
            DoSliding();
        }

        if (characterController.isGrounded)
        {
            playerVector.y = -0.2f;
        }
        else playerVector.y = playerVector.y - (gValue * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(soundFXClips[3], 0.4f); ;
            anim.SetTrigger("isJumping");
            playerVector.y = Mathf.Sqrt(jumpForce * gValue);
        }

        if(this.transform.position.y < -0.5f)
        {
            isDead = true;
            audioSource.PlayOneShot(soundFXClips[2], 0.4f); ;
            anim.SetTrigger("isTripping");
        }
        characterController.Move(playerVector);
        distance = playerSpeed * timer;
        distanceRun = (int)distance;
    }

    void DoSliding()
    {
        characterController.height = 1.0f;
        characterController.center = new Vector3(0, 0.5f, 0);
        characterController.radius = 0;
        StartCoroutine(ReEnableCC());
        audioSource.PlayOneShot(soundFXClips[5], 0.4f); ;
        anim.SetTrigger("isSliding");
    }

    IEnumerator ReEnableCC()
    {

        yield return new WaitForSeconds(0.5f);
        characterController.height = 2.0f;
        characterController.center = new Vector3(0, 1.0f, 0);
        characterController.radius = 0.2f;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "LCorner")
        {
            canTurnLeft = true;
        }
        else if(hit.gameObject.tag == "RCorner")
        {
            canTurnRight = true;
        }
        else
        {
            canTurnLeft = false;
            canTurnRight = false;
        }

        if(hit.gameObject.tag == "Obstacle")
        {
            isDead = true;
            audioSource.PlayOneShot(soundFXClips[1], 0.4f);
            anim.SetTrigger("isTripping");
            SavedScore();
        }
    }

    private void OnGUI()
    {
        if (isDead)
        {
            deathMenu.SetActive(true);
        }
    }

    public void DeathEvent()
    {
        deathMenu.SetActive(false);
        bestCoinScoreText.text = "";
        bestDistanceScoreText.text = "";
        characterController.enabled = false;
        this.transform.position = new Vector3(0,2,0);
        this.transform.rotation = Quaternion.Euler(000, 000, 000);
        playerDirection = enDirection.North;
        playerNextDirection = enDirection.North;
        playerSpeed = playerStartSpeed;
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime;
        bridgeSpawner.CleanTheScene();
        coinsCollected = 0;
        timer = 0;
        anim.SetTrigger("isSpawned");
        isDead = false;
    }

    void FootstepEvent()
    {
        audioSource.PlayOneShot(soundFXClips[0], 0.4f);
    }

    void FootstepEventB()
    {
        audioSource.PlayOneShot(soundFXClips[0], 0.4f);
    }

    void JumpLandEvent()
    {
        audioSource.PlayOneShot(soundFXClips[4], 0.4f);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Coin")
        {
            Destroy(col.gameObject);
            audioSource.PlayOneShot(soundFXClips[7], 0.4f);
            coinsCollected += 1;
        }
    }

    void SavedScore()
    {
        if(coinsCollected > coinsCollectedBest)
        {
            coinsCollectedBest = coinsCollected;
            PlayerPrefs.SetInt("highscoreC", coinsCollectedBest);
            PlayerPrefs.Save();
            bestCoinScoreText.text = "Congrats! Your new coin score is: " + coinsCollectedBest.ToString();
        }

        if(distanceRun > distanceRunBest)
        {
            distanceRunBest = distanceRun;
            PlayerPrefs.SetInt("highscoreD", distanceRunBest);
            PlayerPrefs.Save();
            bestDistanceScoreText.text = "Radical! Your new distance score is: " + distanceRunBest.ToString() + "M";
        }
    }

    public void ExitApp()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
