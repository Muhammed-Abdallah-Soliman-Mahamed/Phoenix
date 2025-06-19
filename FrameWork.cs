using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class Phoenix : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public int maxJumps = 2;
    private int jumpCount = 0;
    public bool isGrounded = true;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform spawnPoint;

    [Header("Enemy Spawning")]
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 3f;
    private float enemyTimer = 0f;

    [Header("Score & Timer")]
    public int score = 0;
    public TMP_Text scoreText;
    private float gameTime = 0f;
    public TMP_Text timerText;

    [Header("Power Ups")]
    public float powerUpDuration = 5f;
    private bool hasShield = false;

    // ================================================
    // Player Movement & Rotation
    public void MovePlayer(Rigidbody rb, Transform playerTransform)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // ÍÑßÉ ááÃãÇã ÈäÇÁð Úáì ÇÊÌÇå ÇáÌÓã
        Vector3 moveDirection = playerTransform.forward * vertical * moveSpeed;

        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);

        Rotate(rb, horizontal);
    }

    public void Rotate(Rigidbody rb, float horizontalInput)
    {
        // ÞÑÇÁÉ ÇáãÏÎáÇÊ ÇáãÑÓáÉ ááÏæÑÇä Íæá ÇáãÍæÑíä X æ Y
        float rotationY = horizontalInput * rotationSpeed * Time.deltaTime;

        // ÅäÔÇÁ ÏæÑÇä ÌÏíÏ ÈÇÓÊÎÏÇã ÇáãÍÇæÑ
        Vector3 rotation = new Vector3(0f, rotationY, 0f);

        // ÊØÈíÞ ÇáÏæÑÇä Úáì Rigidbody
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
    }

    // ================================================
    // Jump & Double Jump
    public void Jump(Rigidbody rb)
    {
        if (jumpCount < maxJumps && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    public void CheckGround(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            Debug.Log("Touched Ground. jumpCount reset.");
        }
    }


    // ================================================
    // Shooting
    public void Shoot()
    {
        SpawnBullet();
    }

    public void SpawnBullet()
    {
        if (bulletPrefab != null && spawnPoint != null)
        {
            Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    // ================================================
    // Enemy Spawning
    public void EnemySpawnTimer()
    {
        enemyTimer += Time.deltaTime;
        if (enemyTimer >= spawnInterval)
        {
            SpawnEnemy();
            enemyTimer = 0f;
        }
    }

    public void SpawnEnemy()
    {
        if (enemyPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }
    }

    // ================================================
    // Score & Timer Update
    public void IncreaseScore(int points)
    {
        score += points;
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateTimer()
    {
        gameTime += Time.deltaTime;
        if (timerText != null)
            timerText.text = "Time: " + gameTime.ToString("F2");
    }

    // ================================================
    // PowerUps Handling
    public void HandlePowerUp(Collider other)
    {
        if (other.CompareTag("SpeedBoost"))
        {
            ActivateSpeedBoost();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("JumpBoost"))
        {
            ActivateJumpBoost();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("ExtraScore"))
        {
            ActivateExtraScore();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Shield"))
        {
            ActivateShield();
            Destroy(other.gameObject);
        }
    }

    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedBoostRoutine());
    }
    IEnumerator SpeedBoostRoutine()
    {
        moveSpeed *= 2f;
        yield return new WaitForSeconds(powerUpDuration);
        moveSpeed /= 2f;
    }

    public void ActivateJumpBoost()
    {
        StartCoroutine(JumpBoostRoutine());
    }
    IEnumerator JumpBoostRoutine()
    {
        jumpForce *= 1.5f;
        yield return new WaitForSeconds(powerUpDuration);
        jumpForce /= 1.5f;
    }

    public void ActivateExtraScore()
    {
        IncreaseScore(50);
    }

    public void ActivateShield()
    {
        hasShield = true;
    }

    public void TakeDamage(int damage)
    {
        if (hasShield)
        {
            hasShield = false;
            return;
        }
        // åäÇ íãßäß ÅÖÇÝÉ ãäØÞ ÊÞáíá ÇáÕÍÉ
    }

    // ================================================
    // Animator Functions
    public void Anim_SetBool(Animator anim, string param, bool value)
    {
        Debug.Log("check if there is this code Animator anim = obj.GetComponent<Animator>();");
        if (anim != null)
            anim.SetBool(param, value);
    }

    public void Anim_SetTrigger(Animator anim, string param)
    {
        Debug.Log("check if there is this code Animator anim = obj.GetComponent<Animator>();");
        if (anim != null)
            anim.SetTrigger(param);
    }

    public void Anim_SetFloat(Animator anim, string param, float value)
    {
        Debug.Log("check if there is this code Animator anim = obj.GetComponent<Animator>();");
        if (anim != null)
            anim.SetFloat(param, value);
    }

    public void Anim_SetInt(Animator anim, string param, int value)
    {
        Debug.Log("check if there is this code Animator anim = obj.GetComponent<Animator>();");
        if (anim != null)
            anim.SetInteger(param, value);
    }

    // ================================================
    // AudioSource Functions
    public void Audio_Play(AudioSource audio)
    {
        Debug.Log("check if there is this code AudioSource audio = obj.GetComponent<AudioSource>();");
        if (audio != null && !audio.isPlaying)
            audio.Play();
    }

    public void Audio_Stop(AudioSource audio)
    {
        Debug.Log("check if there is this code AudioSource audio = obj.GetComponent<AudioSource>();");
        if (audio != null && audio.isPlaying)
            audio.Stop();
    }

    public void Audio_Pause(AudioSource audio)
    {
        Debug.Log("check if there is this code AudioSource audio = obj.GetComponent<AudioSource>();");
        if (audio != null && audio.isPlaying)
            audio.Pause();
    }

    // ================================================
    // ParticleSystem Functions
    public void Particle_Play(ParticleSystem ps)
    {
        Debug.Log("check if there is this code ParticleSystem ps = obj.GetComponent<ParticleSystem>();");
        if (ps != null && !ps.isPlaying)
            ps.Play();
    }

    public void Particle_Stop(ParticleSystem ps)
    {
        Debug.Log("check if there is this code ParticleSystem ps = obj.GetComponent<ParticleSystem>();");
        if (ps != null && ps.isPlaying)
            ps.Stop();
    }

    public void Particle_Pause(ParticleSystem ps)
    {
        Debug.Log("check if there is this code ParticleSystem ps = obj.GetComponent<ParticleSystem>();");
        if (ps != null && ps.isPlaying)
            ps.Pause();
    }
    public void SetIsGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    public void CameraFollowing(Transform cam, Vector3 offset)
{
    cam.position = transform.position + transform.rotation * offset;
    cam.rotation = transform.rotation;
}
}
