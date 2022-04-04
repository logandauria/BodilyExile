

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
class MarkovTest: MonoBehaviour {


    // We need audioSources.length == clips.length because
    // in order to have multiple samples playing at the same time,
    // each needs its own `AudioSource`.
    // Rather than manually configuring each one to be mutually exclusive,
    // for now we'll just set n of them.
    //
    // (for me: note that audioClips is public so we can set the files in Unity for testing)
    private AudioSource[] audioSources = new AudioSource[NUM_FILES];
    public AudioClip[] audioClips      = new AudioClip[NUM_FILES];

    private const int NUM_FILES = 6;
    private bool running = false;

    void Start() {

        // Instantiate all the AudioSources
        for (int i = 0; i < NUM_FILES; i++) {
            GameObject child = new GameObject("Player");
            child.transform.parent = gameObject.transform;
            audioSources[i] = child.AddComponent<AudioSource>();
        }


        // Trigger all clips to play at once (for now)
        for (int i = 0; i < NUM_FILES; i++) {
            audioSources[i].clip = audioClips[i];
        }

        // FIXME: Starting coroutines in Start() vs. Update()?
        // Enable playing of audio now that we've initialized everything
        running = true;

        // TODO: Make into array of coroutines or something

        // Play track0 on a simulated loop,
        // re-triggering after each interval.
        //
        // Note that the system needs around a ~1s buffer to
        // load the file into memory (src: https://bit.ly/35EWiI4),
        // so timers should be triggered to start *after* all
        // sounds have been initialized.
        StartCoroutine(PlayTrack0Loop());
    }


    private IEnumerator PlayTrack0Loop() {
        while(true) {
            Debug.Log("Playing track0");
            audioSources[0].Play();
            // TODO: add hook here into prob matrix for ith phase of project
            yield return new WaitForSeconds(audioSources[0].clip.length);
        }
    }

    void Update() {
        if (!running) {
            return;
        }

        // TODO: May move track coroutines in here
    }

}


class Markov {

    private double[,] phase0prob;
    private double[,] phase1prob;

    public Markov() {

        // We need to create probability matrices
        // for *each* phase of the project.

        // Phase 0 - (two vocals)
        //           Very low probability of second vocal triggering.
        //
        //  {                sample1 sample2 sample3
        //   track1-vocals {   0.5     0.6    0.4    }
        //   track2-vocals {   0.2     0.1    0.1    }
        //  }
        //
        //
        phase0prob = new double[,] {{ 0.5, 0.6, 0.4 },
                                    { 0.2, 0.9, 0.1 }};


        // Phase 1 - (two vocals + guitar)
        //           Two vocals interact w.h.p, guitar is constant
        //
        //  {                sample1 sample2 sample3
        //   track1-vocals {   0.5     0.6    0.4    }
        //   track2-vocals {   0.5     0.4    0.1    }
        //   guitar        {   0.8     0.2    ___    }
        //  }
        //
        //
        phase1prob = new double[,] {{ 0.5, 0.6, 0.4 },
                                    { 0.5, 0.4, 0.1 },
                                    { 0.8, 0.2, 0.0 }};

        // TODO: Add more as implemented.


    }

}
