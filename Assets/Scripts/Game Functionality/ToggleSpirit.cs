using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to change the inner orb into a particle person
public class ToggleSpirit : MonoBehaviour
{
    public GameObject spirit;
    public GameObject sphere2;

    /// <summary>
    /// Called with a unity event to toggle off inner sphere, toggle on spirit
    /// </summary>
    public void Toggle()
    {
        spirit.SetActive(true);
        sphere2.SetActive(false);
    }
}
