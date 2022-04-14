using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public int levelToLoad;
    public Material fadeMat;

    // Start is called before the first frame update
    void Start()
    {
        // start fade with full alpha
        fadeMat.color = new Color(0,0,0,255);
        StartLevel();
    }

    public void FadeToLevel()
    {
        while (fadeMat.color.a < 255)
        {
            fadeMat.color = new Color(0, 0, 0, Mathf.Lerp(fadeMat.color.a, 255, Time.deltaTime));
        }
        SceneManager.LoadScene(levelToLoad);
    }

    public void StartLevel()
    {
        while(fadeMat.color.a > 0)
        {
            fadeMat.color = new Color(0, 0, 0, Mathf.Lerp(fadeMat.color.a, 0, Time.deltaTime));
        }
        Debug.Log("faded in");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
