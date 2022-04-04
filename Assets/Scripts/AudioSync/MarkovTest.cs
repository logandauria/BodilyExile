

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
            audioSources[i].Play();
        }
    }

    void Update() {

        if (!running) {
            return;
        }


        // audioSources[0].clip = clips[0];
        // audioSources[0].Play();
        // play1 = true;

        // Debug.Log("Clip length is " + audioSources[0].clip.length);
        // Debug.Log("Clip length is " + audioSources[1].clip.length);

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

// class Sample {

//     public String filename {get; set;}

//     public Sample(String filename) {
//         this.filename = filename;
//     }


//     public void play() {

//     }


// }

