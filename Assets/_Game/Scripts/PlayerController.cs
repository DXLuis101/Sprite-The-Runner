using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum enDirection { North, East, West};

    CharacterController characterController;
    Vector3 playerVector; // player's direction
    enDirection playerDirection = enDirection.North;
    enDirection playerNextDirection = enDirection.North;
    Animator anim;

    float playerStartSpeed = 10.0f;
    float playerSpeed; // player's speed
    float gValue = 10.0f;
    float translationfactor = 10.0f;
    float jumpForce = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = playerStartSpeed;
        characterController = this.GetComponent<CharacterController>();
        anim = this.GetComponent<Animator>();
        playerVector = new Vector3(0, 0, 1) * playerSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLogic();
    }

    void PlayerLogic()
    {
        playerSpeed += 0.005f * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G))
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
        }
        else if (Input.GetKeyDown(KeyCode.F))
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
            anim.SetTrigger("isJumping");
            playerVector.y = Mathf.Sqrt(jumpForce * gValue);
        }
        characterController.Move(playerVector);
    }

    void DoSliding()
    {
        characterController.height = 1.0f;
        characterController.center = new Vector3(0, 0.5f, 0);
        characterController.radius = 0;
        StartCoroutine(ReEnableCC());
        anim.SetTrigger("isSliding");
    }

    IEnumerator ReEnableCC()
    {

        yield return new WaitForSeconds(0.5f);
        characterController.height = 2.0f;
        characterController.center = new Vector3(0, 1.0f, 0);
        characterController.radius = 0.2f;
    }
}
