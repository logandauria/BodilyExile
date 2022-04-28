using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls level operations and changes through unity objects. 
public class LevelChanger : MonoBehaviour
{

    // List input for the different phases/levels of the game
    public GameObject[] phases;
    // Track what level game is on
    public int currLevel = 1;
    //public int levelToLoad;
    //public GameObject fadeBox;
    //private Renderer fadeRender;

    // Object lists for game objects that are not attached to their respective phases/levels
    public GameObject[] phase1objs;
    public GameObject[] phase2objs;
    public GameObject[] phase3objs;
    public GameObject[] phase4objs;


    //private bool fadeComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        // start fade with full alpha
        //fadeRender = fadeBox.GetComponent<Renderer>();
        //fadeRender.material.color = new Color(0,0,0,255);
        //StartLevel();
    }

    /// <summary>
    /// Called through unity events to trigger a level change. Will deactivate and activate all necessary objects based on the current level
    /// </summary>
    public void nextLevel()
    {
        phases[currLevel-1].SetActive(false);
        currLevel += 1;
        phases[currLevel-1].SetActive(true);
        //else
        //{
        //    foreach (GameObject g in phase1objs)
        //    {
        //        g.SetActive(false);
        //    }
        //}
        // if phase 2 activate phase 2 objects, if not, deactivate them
        if (currLevel == 3)
        {
            // activate phase 2 objs
            foreach(GameObject g in phase2objs)
            {
                g.SetActive(true);
            }
        } else
        {
            foreach (GameObject g in phase2objs)
            {
                g.SetActive(false);
            }
        }
        // if phase 3 activate phase 3 objects, if not, deactivate them
        if (currLevel == 4)
        {
            // activate phase 3 objs
            foreach (GameObject g in phase3objs)
            {
                g.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject g in phase3objs)
            {
                g.SetActive(false);
            }
        }
        // if phase 4 activate phase 4 objects, if not, deactivate them
        if (currLevel == 5)
        {
            // activate phase 4 objs
            foreach (GameObject g in phase4objs)
            {
                g.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject g in phase4objs)
            {
                g.SetActive(false);
            }
        }
        // if phase 1 activate phase 1 objects, if not, deactivate them
        if (currLevel == 2)
        {
            // activate phase 1 objs
            foreach (GameObject g in phase1objs)
            {
                g.SetActive(true);
            }
        }
        //FadeToLevel();
        //SceneManager.LoadScene(sceneToLoad);
    }

    /*public void FadeToLevel()
    {
        while (fadeRender.material.color.a < 255)
        {
            fadeRender.material.color = new Color(0, 0, 0, Mathf.Lerp(fadeRender.material.color.a, 255, Time.deltaTime));
        }
        //SceneManager.LoadScene(levelToLoad);
    }

    public void StartLevel()
    {
        while(fadeRender.material.color.a > 0)
        {
            fadeRender.material.color = new Color(0, 0, 0, Mathf.Lerp(fadeRender.material.color.a, 0, Time.deltaTime));
        }
        Debug.Log("faded in");
    }*/


    // Update is called once per frame
    void Update()
    {
        
    }
}
