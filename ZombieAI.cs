using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    //HealtBar

    BarControl barcontrol;
    Image HealthBar;

    private void Awake()
    {
        charopt = GameObject.FindGameObjectWithTag("Player").GetComponent<Character_Opt>();
        gameopt = GameObject.FindGameObjectWithTag("GameOpt").GetComponent<GameOpt>();
        karakter = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        Capsulecollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();

        barcontrol = GameObject.FindGameObjectWithTag("BarControl").GetComponent<BarControl>();
        HealthBar = GameObject.FindGameObjectWithTag("healthbar").transform.GetChild(0).GetComponent<Image>();

        can = 100;

        deadSound = true;
    }

    private void FixedUpdate()
    {
        if (!barcontrol.MainCharacterisDeath)
        {


            // pistolü eline aldiginda zombiyi takip et
            // Birden cok zombi olduğu için bu kodu pistol scriptine yazmadik eger yazsaydik istenilen gibi calismazdi bu yuzden bu kodun tum zombilerde olmasi lazim

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
        else
        {
            anim.SetBool("Victory", true);
        }

    }

    void Follow()
    {
        pos = new Vector3(karakter.position.x, transform.position.y, karakter.position.z);

        mesafe = Vector3.Distance(karakter.position, transform.position);

        if (mesafe > 3 && mesafe < 20)
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
        if (mesafe > 20)
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

                StartCoroutine(Damage(1.2f));

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

    void HitFollow()
    {
        pos = new Vector3(karakter.position.x, transform.position.y, karakter.position.z);

        mesafe = Vector3.Distance(karakter.position, transform.position);

        StartCoroutine(hityedi(7));

        if (mesafe > 3 && mesafe < 35)
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
        if (mesafe > 35)
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

                StartCoroutine(Damage(1.2f));

                AngrySource.Stop();
                AttackSource.Play();

                attacktime = Time.time + 1.8f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rotate"))
        {
            float rast = Random.Range(-180, 180);

            transform.Rotate(new Vector2(0, rast));
        }
    }

    IEnumerator hityedi(float sure)
    {
        yield return new WaitForSeconds(sure);

        HitYedi = false;
    }

    IEnumerator Damage(float sure)
    {
        yield return new WaitForSeconds(sure);

        HealthBar.fillAmount -= 0.35f;
    }
}

