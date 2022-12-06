using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
   
    public float speed;
    public float normalSpeed = 3.0f;
    public float speedBoost = 6.0f;
    private float speedBoostDuration = 5;
    
    public int maxHealth = 5;
    public int health { get { return currentHealth; }}
    int currentHealth;

    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}
    public int currentAmmo;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal; 
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioSource backgroundManager;
    public AudioClip winClip;
    public AudioClip loseClip;
    
    public ParticleSystem damageEffect;

    public TextMeshProUGUI fixedText;
    private int scoreFixed = 0;

    public TextMeshProUGUI ammoText;

    public TextMeshProUGUI bookFoundText;
    public int books { get { return booksHeld; }}
    public int booksHeld = 0;
    public GameObject returnBookText;

    public GameObject WinText;
    public GameObject LoseText;
    bool gameOver;
    bool winGame;
    public static int level = 0;
    
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        audioSource= GetComponent<AudioSource>();

        rigidbody2d = GetComponent<Rigidbody2D>();
        AmmoText();

        currentAmmo = 4;
        ammoText.text = "Ammo: " + currentAmmo;

        fixedText.text = "Robots Fixed: " + scoreFixed.ToString() + "/4";

        BookText();
        bookFoundText.enabled = false;
        bookFoundText.text = "Book Found: " + booksHeld.ToString() + "/1";
        returnBookText.SetActive(false);

        WinText.SetActive(false);
        LoseText.SetActive(false);
        gameOver = false;
        winGame = false;
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();

            if (currentAmmo > 0)
            {
                ChangeAmmo(-1);
                AmmoText();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (hit.collider != null)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        if (scoreFixed >= 4)
                        {
                            SceneManager.LoadScene("Level2");
                            level = 1;
                        }
                        
                        else
                        {
                            character.DisplayDialog();
                        }
                    }

                    CatNPC cat = hit.collider.GetComponent<CatNPC>();
                    if (cat != null)
                    {
                        if (booksHeld >= 1)
                        {
                            cat.DisplayThanks();
                            bookFoundText.enabled = false;
                            returnBookText.SetActive(false);
                        }
                        else
                        {
                            cat.DisplayDialog();
                            bookFoundText.enabled = true;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver == true)
            {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (winGame == true)
            {
                SceneManager.LoadScene("MainScene");
                level = 0;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitSound);

            animator.SetTrigger("Hit");

            damageEffect = Instantiate(damageEffect, transform.position, Quaternion.identity);
            damageEffect.Play();
        }

        if (currentHealth == 1)
        {
            transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            LoseText.SetActive(true);
            speed = 0;
            Destroy(gameObject.GetComponent<SpriteRenderer>());

            gameOver = true;

            backgroundManager.Stop();

            PlaySound(loseClip);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeAmmo(int amount)
    {
        currentAmmo = Mathf.Abs(currentAmmo + amount);
    }

    public void AmmoText()
    {
        ammoText.text = "Ammo: " + currentAmmo.ToString();
    }

    public void GrabBook(int amount)
    {
        booksHeld = Mathf.Abs(booksHeld + amount);

        if (booksHeld == 1)
        {
            returnBookText.SetActive(true);
        }
    }

    public void BookText()
    {
        bookFoundText.text = "Book Found: " + booksHeld.ToString() + "/1";
    }

    void Launch()
    {
        if (currentAmmo > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

            PlaySound(throwSound);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void FixedRobots(int amount)
    {
        scoreFixed += amount;
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/4";

        if (scoreFixed == 4 && level == 0)
        {
            WinText.SetActive(true);
        }

        if (scoreFixed == 4 && level == 1)
        {
            WinText.SetActive(true);
            winGame = true;

            speed = 0;

            backgroundManager.Stop();

            PlaySound(winClip);
        }
    }

    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedBoostCooldown());
    }

    IEnumerator SpeedBoostCooldown()
    {
        speed = speedBoost;
        yield return new WaitForSeconds(speedBoostDuration);
        speed = normalSpeed;
    }
}
