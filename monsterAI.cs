using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class monsterAI : MonoBehaviour
{
    public GameObject player;
    public AudioClip[] footsounds;
    public AudioClip roar;
    public AudioClip chase;
    public AudioClip detected;
    public AudioClip level1Theme;
    public AudioClip level2Theme;
    public float navSpeed;
    public float animSpeed;
    public float searchArea;
    public float aggroRange;
    public float attackRange;
    public float minAggroRange;
    public float despawnTimer;
    public float despawnTimerMax;
    public Transform eyes;
    public Transform eyesArea;
    public GameObject deathCam;
    public Transform camPos;
    
    private NavMeshAgent nav;
    private AudioSource sound;
    private Animator anim;
    private string state = "walk";
    private string sceneName;
    private bool alive = true;
    private bool highAlert = false;
    private bool playChaseSound = false;
    private bool playMusic = false;
    private float chaseXSeconds;

    // Use this for initialization//
    void Start ()
    {
        nav = GetComponent<NavMeshAgent>();
        sound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        nav.speed = navSpeed;
        anim.speed = animSpeed;
        despawnTimerMax = despawnTimer;
	}
	
	// Update is called once per frame//
	void Update ()
    {
        if (alive)
        {
            anim.SetFloat("velocity", nav.velocity.magnitude);

            //Walking//
            if(state == "walk")
            {
                anim.SetBool("isIdle", true);
                anim.SetBool("isSpotted", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isDead", false);

                Vector3 randomPos = Random.insideUnitSphere * searchArea;
                NavMeshHit navHit;
                NavMesh.SamplePosition(transform.position + randomPos, out navHit, searchArea, NavMesh.AllAreas);

                //Go near player//
                if (highAlert)
                {
                    NavMesh.SamplePosition(player.transform.position + randomPos, out navHit, searchArea, NavMesh.AllAreas);

                    //Increase search area over time after player got spotted//
                    searchArea += 15f;

                    if(searchArea > 30f)
                    {
                        highAlert = false;
                        nav.speed = navSpeed;
                    }
                }
                
                nav.SetDestination(navHit.position);
                state = "patrol";
            }

            //Patrol//
            if(state == "patrol")
            {
                //float distance = Vector3.Distance(transform.position, player.transform.position);
                if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending){
                    state = "walk";
                    anim.SetBool("isIdle", true);
                    anim.SetBool("isSpotted", false);
                    anim.SetBool("isAttacking", false);
                    anim.SetBool("isDead", false);
                }
            }

            //Chase//
            if(state == "chase")
            {
                nav.speed = navSpeed * 2.5f;
                nav.destination = player.transform.position;
                anim.SetBool("isSpotted", true);
                anim.SetBool("isIdle", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isDead", false);


                float distance = Vector3.Distance(transform.position, player.transform.position);

                if (playChaseSound)
                {
                    AudioManager.instance.PlaySound(detected, transform.position);
                    AudioManager.instance.PlayMusic(chase);
                    playChaseSound = false;
                }

                
                if (distance > aggroRange)
                {
                    playMusic = true;
                    state = "hunt";
                }
                else if(distance < attackRange)
                {
                    //state = "attack"; //
                    if (player.GetComponent<player>().alive)
                    {
                        state = "kill";
                        player.GetComponent<player>().alive = false;
                        player.GetComponent<FirstPersonController>().enabled = false;
                        deathCam.SetActive(true);
                        deathCam.transform.position = Camera.main.transform.position;
                        deathCam.transform.rotation = Camera.main.transform.rotation;
                        Camera.main.gameObject.SetActive(false);
                        Invoke("reset", 1.5f);
                    }
                }
            }

            //Permachase//
            if(state == "permachase")
            {
                nav.speed = navSpeed * 2.5f;
                nav.destination = player.transform.position;
                anim.SetBool("isSpotted", true);
                anim.SetBool("isIdle", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isDead", false);

                if (playChaseSound)
                {
                    AudioManager.instance.PlaySound(detected, transform.position);
                    AudioManager.instance.PlayMusic(chase);
                    playChaseSound = false;
                }

                float distance = Vector3.Distance(transform.position, player.transform.position);

                if(distance < attackRange)
                {
                    if (player.GetComponent<player>().alive)
                    {
                        state = "kill";
                        player.GetComponent<player>().alive = false;
                        player.GetComponent<FirstPersonController>().enabled = false;
                        deathCam.SetActive(true);
                        deathCam.transform.position = Camera.main.transform.position;
                        deathCam.transform.rotation = Camera.main.transform.rotation;
                        Camera.main.gameObject.SetActive(false);
                        print("supposed to play sound...");
                        Invoke("reset", 1.5f);
                    }
                }
            }

            if(state != "chase" && state != "permachase" && state != "chaseForSeconds")
            {
                if (playMusic)
                {
                    string sceneName;
                    sceneName = SceneManager.GetActiveScene().name;
                    if(sceneName == "HorrorGame")
                    {
                        AudioManager.instance.PlayMusic(level1Theme, 1);
                    }
                    else
                    {
                        AudioManager.instance.PlayMusic(level2Theme, 1);
                    }
                    
                    playMusic = false;
                }
            }

            //hunt//
            if(state == "hunt")
            {
                if(nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending)
                {
                    state = "walk";
                    highAlert = true;
                    searchArea = 20f;
                    spotPlayer();
                }
            }

            //kill//
            if(state == "kill")
            {
                anim.SetBool("isSpotted", false);
                anim.SetBool("isIdle", false);
                anim.SetBool("isAttacking", true);
                nav.isStopped = true;
                anim.speed = 0.8f;

                deathCam.transform.position = Vector3.Slerp(deathCam.transform.position, camPos.position, 10f * Time.deltaTime);
                deathCam.transform.rotation = Quaternion.Slerp(deathCam.transform.rotation, camPos.rotation, 10f * Time.deltaTime);
                anim.speed = 1f;
                nav.SetDestination(deathCam.transform.position);
            }

            //chase for seconds//
            if(state == "chaseForSeconds")
            {
                if (chaseXSeconds > 1)
                {
                    chaseXSeconds -= Time.deltaTime;

                    nav.speed = navSpeed * 2.5f;
                    nav.destination = player.transform.position;
                    anim.SetBool("isSpotted", true);
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isAttacking", false);
                    anim.SetBool("isDead", false);

                    if (playChaseSound)
                    {
                        AudioManager.instance.PlaySound(detected, transform.position);
                        AudioManager.instance.PlayMusic(chase);
                        playChaseSound = false;
                    }

                    float distance = Vector3.Distance(transform.position, player.transform.position);

                    if (distance < attackRange)
                    {
                        if (player.GetComponent<player>().alive)
                        {
                            state = "kill";
                            player.GetComponent<player>().alive = false;
                            player.GetComponent<FirstPersonController>().enabled = false;
                            deathCam.SetActive(true);
                            deathCam.transform.position = Camera.main.transform.position;
                            deathCam.transform.rotation = Camera.main.transform.rotation;
                            Camera.main.gameObject.SetActive(false);
                            print("supposed to play sound...");
                            Invoke("reset", 1.5f);
                        }
                    }
                }
                else
                {
                    state = "chase";
                }
            }
     
        }
        else if (!alive)
        {
            //dead//
            if (state == "dead")
            {
                if (despawnTimer >= 0)
                {
                    despawnTimer -= Time.deltaTime;
                    if (despawnTimer < 1)
                    {
                        string sceneName;
                        sceneName = SceneManager.GetActiveScene().name;
                        if (sceneName == "HorrorGame")
                        {
                            AudioManager.instance.PlayMusic(level1Theme, 1);
                        }
                        else
                        {
                            AudioManager.instance.PlayMusic(level2Theme, 1);
                        }

                        anim.SetBool("isDead", false);
                        anim.SetBool("isIdle", true);
                        gameObject.SetActive(false);
                    }
                }
            }
        } 
    }

    //Spawn the monster//
    public void spawn(int level)
    {
        alive = true;
        gameObject.SetActive(true);
        despawnTimer = despawnTimerMax;
        AudioManager.instance.PlaySound(roar, transform.position);
        //Level 1//
        if (level == 0)
        {
            state = "walk";
        }
        //Level 2 objective 1//
        else if(level == 1)
        {
            playChaseSound = true;
            state = "chase";
        }
        //Level 2 objective 2//
        else if(level == 3)
        {
            playChaseSound = true;
            chaseXSeconds = 10;
            state = "chaseForSeconds";
            nav.Warp(new Vector3(-52.66f, 4.3f, 219.61f));
        }
        //Level 2 objective 5//
        else if(level == 5)
        {
            nav.Warp(new Vector3(-66.668f, 4.36f, 308.857f));
            playChaseSound = true;
            state = "permachase";
        }
    }

    //Despawn and remove the monster//
    public void death()
    {
        alive = false;
        anim.SetBool("isDead", true);
        anim.SetBool("isIdle", false);
        anim.SetBool("isSpotted", false);
        nav.isStopped = true;
        state = "dead";
    }

    //Reset//
    void reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void spotPlayer()
    {
        if (alive)
        {
            RaycastHit rayHit;
            //Monster frontal cone//
            if(Physics.Linecast(eyes.position, player.transform.position, out rayHit))
            {
                if(rayHit.collider.gameObject.name == "Player")
                {
                    if(state == "chaseForSeconds" || state == "permachase")
                    {
                        playChaseSound = true;
                    }
                    else
                    {
                        state = "chase";
                        playChaseSound = true;
                    }
                }
            }
            //Monster area cone//
            else if(Physics.Linecast(eyesArea.position,player.transform.position, out rayHit))
            {
                if(rayHit.collider.gameObject.name == "Player")
                {
                    if (state == "chaseForSeconds" || state == "permachase") 
                    {
                        playChaseSound = true;
                    }
                    else
                    {
                        state = "chase";
                        playChaseSound = true;
                    }
                }
            }
        }
    }

    public void footstep(int _num)
    {
        sound.clip = footsounds[_num];
        //sound.Play();
        AudioManager.instance.PlaySound(sound.clip, transform.position);
    }
}