using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    public GameObject[] phases;
    public int currLevel = 0;
    //public int levelToLoad;
    //public GameObject fadeBox;
    //private Renderer fadeRender;



    //private bool fadeComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        // start fade with full alpha
        //fadeRender = fadeBox.GetComponent<Renderer>();
        //fadeRender.material.color = new Color(0,0,0,255);
        //StartLevel();
    }

    public void nextLevel()
    {
        phases[currLevel].SetActive(false);
        currLevel += 1;
        phases[currLevel].SetActive(true);

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
