using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class objectiveCanvas : MonoBehaviour {

    public static objectiveCanvas instance;
    public GameObject Illuminate_Everything;
    public GameObject stairs;
    public monsterAI monsterAI;
    public Text txtPages;
    public int pagesTotal = 5;
    public string storyString;
    public GameObject monster;
    public GameObject door;
    public GameObject key;

    private int pagesFound = 0;
    private string pagesString;
    private string sceneName;
    private int objectiveNumber = 1;
    //private float monsterTimer = 60f; //1 minute of monster uptime atm

    void Awake()
    {
        instance = this;
    }

    public void updateCanvas()
    {
        if (sceneName == "HorrorGame")
        {
            pagesString = "Research papers: " + pagesFound.ToString() + "/" + pagesTotal.ToString();
            txtPages.text = pagesString;
        }
    }

    public void findPage()
    {
        pagesFound++;
        updateCanvas();


        //Spawn Monster//
        if(pagesFound == 2)
        {
            monsterAI.spawn(0);
        }

        //Win//
        if(pagesFound == pagesTotal)
        {
            //Alpha only//
            stairs.SetActive(false);
            monsterAI.death();
            txtPages.text = "The papers tell of a secret set of stairs next to Aud Max";
        }
    }

    public void spawnMonster()
    {
        if(objectiveNumber == 1)
        {
            monsterAI.spawn(objectiveNumber);
            txtPages.text = "There is something in the cave system, I should probably get out of here.";
            objectiveNumber++;
        }
        else if(objectiveNumber == 2)
        {
            txtPages.text = "The notes said something about a cargo room. I should probably look for it.";
            objectiveNumber++;
        }
        else if(objectiveNumber == 3)
        {
            //monster.SetActive(false);
            //monster.transform.position = new Vector3(-52.66f, 4.3f, 219.61f);
            //monster.SetActive(true);
            monsterAI.spawn(objectiveNumber);
            txtPages.text = "The door to the Cargo bay is locked. I think there was a keycard in the laboratory";
            key.SetActive(true);
            objectiveNumber++;
        }
        else if(objectiveNumber == 4)
        {
            txtPages.text = "This keycard belongs to the exit, I should find that instead.";
            objectiveNumber++;
            door.SetActive(false);
        }
        else if(objectiveNumber == 5)
        {
            txtPages.text = "The monster is on my heels, I gotta run for the exit!";
            //monster.transform.position = new Vector3(-64.8f, 4.3f, 309f);
            monsterAI.spawn(objectiveNumber);
            objectiveNumber++;
        }
    }

	// Use this for initialization
	void Start () {
        //updateCanvas();

        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        if(sceneName == "HorrorGame")
        {
            txtPages.text = storyString;
        }
        else if(sceneName == "HorrorGame_Level2")
        {
            txtPages.text = storyString;
        }
	}
}
