using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float can;

    public int WalkSpeed;

    Transform karakter;
    GameOpt gameopt;
    Character_Opt charopt;

    Collider Capsulecollider;

    NavMeshAgent agent;

    Animator anim;

    Vector3 pos;

    bool follow;
    bool attack;
    bool deadSound;
    public bool HasarAldı;
    public bool HitYedi;

    public float mesafe;

    float Zaman;
    float attacktime;

    public bool dead;
    public bool agrolandı = true;

    // Sounds

    [SerializeField] AudioSource AngrySource;
    [SerializeField] AudioSource AttackSource;
    [SerializeField] AudioSource DeathSource;

    void Start()
    {
        charopt = GameObject.FindGameObjectWithTag("Player").GetComponent<Character_Opt>();
        gameopt = GameObject.FindGameObjectWithTag("GameOpt").GetComponent<GameOpt>();
        karakter = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        Capsulecollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        can = 100;

        deadSound = true;

    }


    private void Update()
    {

    }

    private void FixedUpdate()
    {
        // pistolü eline aldığında zombiyi takip et

        if (gameopt.pistolActive == true)

        {
            mesafe = Vector3.Distance(karakter.position, transform.position);

            if (mesafe <= 22)
            {
                if (charopt.joystick.Horizontal == 0 || charopt.joystick.Vertical == 0)
                {
                    if (!dead)
                    {
                        pos = new Vector3(transform.position.x, karakter.position.y, transform.position.z);
                        karakter.LookAt(pos);
                    }
                }
            }
        }

        if (can > 0)
        {
            if (!HitYedi)
            {
                Follow();
            }
            else
            {
                HitFollow();
            }
        }

        if (can <= 0)
        {
            Capsulecollider.enabled = false;
            anim.SetBool("Run", false);
            anim.SetBool("Death", true);

            follow = false;
            attack = false;
            agent.enabled = false;

            WalkSpeed = 0;

            AngrySource.Stop();
            AttackSource.Stop();

            if (dead)
            {
                if (deadSound == true)
                {
                    DeathSource.Play();
                    deadSound = false;
                }

            }

            Destroy(gameObject, 15);

        }

        if (HasarAldı)
        {
            anim.SetTrigger("Damaged");
            HasarAldı = false;
        }

    }

    void Follow()
    {
        pos = new Vector3(karakter.position.x, transform.position.y, karakter.position.z);

        mesafe = Vector3.Distance(karakter.position, transform.position);

        if (mesafe > 3 && mesafe < 15)
        {
            follow = true;
            attack = false;
            agent.enabled = true;

        }
        if (mesafe < 3)
        {
            follow = false;
            attack = true;
            agent.enabled = false;
        }
        if (mesafe > 15)
        {
            follow = false;
            attack = false;
            agrolandı = true;
            agent.enabled = false;

            RastgeleHareket();
        }

        // Opt

        if (follow)
        {
            transform.LookAt(pos);
            anim.SetBool("Run", true);
            agent.SetDestination(karakter.position);

            if (agrolandı)
            {
                AngrySource.Play();
                agrolandı = false;
            }
        }
        if (attack)
        {
            transform.LookAt(pos);

            if (Time.time >= attacktime)
            {
                anim.SetBool("Attack", true);

                AngrySource.Stop();
                AttackSource.Play();

                attacktime = Time.time + 1.8f;
            }
        }
    }

    void RastgeleHareket()
    {
        if (Zaman <= 15)
        {
            anim.SetBool("Run", false);
            Zaman += Time.deltaTime;

            transform.Translate(0, 0, WalkSpeed * Time.deltaTime);

        }
        if (Zaman >= 15)
        {
            float rast = Random.Range(-180, 180);

            transform.Rotate(new Vector2(0, rast));

            Zaman = 0;
        }
    }
    
    // Eğer hasar alırsa
    
    void HitFollow()
    {
        pos = new Vector3(karakter.position.x, transform.position.y, karakter.position.z);

        mesafe = Vector3.Distance(karakter.position, transform.position);

        StartCoroutine(hityedi(5));

        if (mesafe > 3 && mesafe < 30)
        {
            follow = true;
            attack = false;
            agent.enabled = true;

        }
        if (mesafe < 3)
        {
            follow = false;
            attack = true;
            agent.enabled = false;
        }
        if (mesafe > 30)
        {
            follow = false;
            attack = false;
            agrolandı = true;
            agent.enabled = false;

            RastgeleHareket();
        }

        // Opt

        if (follow)
        {
            transform.LookAt(pos);
            anim.SetBool("Run", true);
            agent.SetDestination(karakter.position);

            if (agrolandı)
            {
                AngrySource.Play();
                agrolandı = false;
            }
        }
        if (attack)
        {
            transform.LookAt(pos);

            if (Time.time >= attacktime)
            {
                anim.SetBool("Attack", true);

                AngrySource.Stop();
                AttackSource.Play();

                attacktime = Time.time + 1.8f;
            }
        }
    }

    IEnumerator hityedi(float sure)
    {
        yield return new WaitForSeconds(sure);

        HitYedi = false;
    }
}

