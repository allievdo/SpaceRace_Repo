using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerMovement : MonoBehaviourPun
{
    [HideInInspector]
    public int id;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;

    public string Level03;

    // cubethon
    public Rigidbody rb;

    public float forwardForce = 2000f;
    public float sidewaysForce = 500f;

    public float jumpForce;

    private float startTime;
    private float timeTaken;

    public TextMeshProUGUI curTimeText;

    public bool movementEnabled = true;


    // local player
    public static PlayerMovement me;

    [PunRPC]
    public void Initialize (Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        if (photonView.IsMine)
        {
            me = this;
            Camera.main.transform.SetParent(transform);
            Begin();
        }

        else
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = false;
            curTimeText.gameObject.SetActive(false);
        }
    }

    public void Begin ()
    {
        startTime = Time.time;
        Debug.Log("Game is playing");
    }

    public void End ()
    {
        timeTaken = Time.time - startTime;
        Leaderboard.instance.SetLeaderboardEntry(-Mathf.RoundToInt(timeTaken * 1000.0f));
    }

        void FixedUpdate()
    {
        // Add a forward force
        if (!photonView.IsMine || !movementEnabled)
            return;

        rb.AddForce(0, 0,forwardForce * Time.deltaTime);

        if (Input.GetKey("d"))
        {
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        if (Input.GetKey("a"))
        {
            rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        if (rb.position.y < -1f)
        {
            //FindObjectOfType<GameManager>().EndGame();
            if (movementEnabled)
            {
                movementEnabled = false;
                StartCoroutine(Respawn(0));
            }
        }
    }

    void Update()
    {
        // only the local player can control this player controller
        if (!photonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        curTimeText.text = (Time.time - startTime).ToString("F2");
    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 0.7f))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public IEnumerator Respawn (float time)
    {
        yield return new WaitForSeconds(time);
        transform.position = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;
        rb.velocity = Vector3.zero;
        movementEnabled = true;
    }
}
