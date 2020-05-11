using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour {

    public bool alive = true;
    void OnTriggerEnter(Collider other)
    {
        if (alive)
        {
            if (other.gameObject.name == "eyes")
            {
                other.transform.parent.GetComponent<monsterAI>().spotPlayer();
            }
            else if (other.gameObject.name == "eyesArea")
            {
                other.transform.parent.GetComponent<monsterAI>().spotPlayer();
            }


            else if (other.CompareTag("bookObjective"))
            {

                Destroy(other.gameObject);
                objectiveCanvas.instance.findPage();
            }
            else if (other.CompareTag("spawnTrigger"))
            {
                objectiveCanvas.instance.spawnMonster();
                Destroy(other.gameObject);
            }
            else if (other.CompareTag("Finish"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else if (other.CompareTag("winCondition"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
