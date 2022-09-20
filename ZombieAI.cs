using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float Health;

    public int WalkSpeed;

    Transform Character;
    GameOpt gameopt;
    Character_Opt charopt;

    Collider Capsulecollider;

    NavMeshAgent agent;

    Animator anim;

    Vector3 pos;

    bool follow;
    bool attack;
    bool deadSound;
    public bool Damaged;
    public bool HitYedi;

    public float Distance;

    float time;
    float attacktime;

    public bool dead;
    public bool agro = true;

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
        Character = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        Capsulecollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();

        barcontrol = GameObject.FindGameObjectWithTag("BarControl").GetComponent<BarControl>();
        HealthBar = GameObject.FindGameObjectWithTag("healthbar").transform.GetChild(0).GetComponent<Image>();

        Health = 100;

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
                Distance = Vector3.Distance(Character.position, transform.position);

                if (Distance <= 22)
                {
                    if (charopt.joystick.Horizontal == 0 || charopt.joystick.Vertical == 0)
                    {
                        if (!dead)
                        {
                            pos = new Vector3(transform.position.x, Character.position.y, transform.position.z);
                            Character.LookAt(pos);
                        }
                    }
                }
            }





            if (Health > 0)
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

            if (Health <= 0)
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

            if (Damaged)
            {
                anim.SetTrigger("Damaged");
                Damaged = false;
            }
        }
        else
        {
            anim.SetBool("Victory", true);
        }

    }

    void Follow()
    {
        pos = new Vector3(Character.position.x, transform.position.y, Character.position.z);

        Distance = Vector3.Distance(Character.position, transform.position);

        if (Distance > 3 && Distance < 20)
        {
            follow = true;
            attack = false;
            agent.enabled = true;

        }
        if (Distance < 3)
        {
            follow = false;
            attack = true;
            agent.enabled = false;
        }
        if (Distance > 20)
        {
            follow = false;
            attack = false;
            agro = true;
            agent.enabled = false;

            RastgeleHareket();
        }

        // Opt

        if (follow)
        {
            transform.LookAt(pos);
            anim.SetBool("Run", true);
            agent.SetDestination(Character.position);

            if (agro)
            {
                AngrySource.Play();
                agro = false;
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
        if (time <= 15)
        {
            anim.SetBool("Run", false);
            time += Time.deltaTime;

            transform.Translate(0, 0, WalkSpeed * Time.deltaTime);

        }
        if (time >= 15)
        {
            float rast = Random.Range(-180, 180);

            transform.Rotate(new Vector2(0, rast));

            time = 0;
        }
    }

    void HitFollow()
    {
        pos = new Vector3(Character.position.x, transform.position.y, Character.position.z);

        Distance = Vector3.Distance(Character.position, transform.position);

        StartCoroutine(hityedi(7));

        if (Distance > 3 && Distance < 35)
        {
            follow = true;
            attack = false;
            agent.enabled = true;

        }
        if (Distance < 3)
        {
            follow = false;
            attack = true;
            agent.enabled = false;
        }
        if (Distance > 35)
        {
            follow = false;
            attack = false;
            agro = true;
            agent.enabled = false;

            RastgeleHareket();
        }

        // Opt

        if (follow)
        {
            transform.LookAt(pos);
            anim.SetBool("Run", true);
            agent.SetDestination(Character.position);

            if (agro)
            {
                AngrySource.Play();
                agro = false;
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

