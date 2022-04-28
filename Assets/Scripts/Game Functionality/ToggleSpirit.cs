using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSpirit : MonoBehaviour
{
    public GameObject spirit;
    public GameObject sphere2;

    public void Toggle()
    {
        spirit.SetActive(true);
        sphere2.SetActive(false);
    }
}
