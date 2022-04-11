

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
    private AudioSource[] audioSources = new AudioSource[28];

    // AudioClips from Bodily Exile (track1)

    public const int NUM_GUITAR   = 7;
    public const int NUM_HARMONY  = 6;
    public const int NUM_LEAD     = 6;
    public const int NUM_WHISPERS = 9;

    public AudioClip[] exileGuitarClips    = new AudioClip[NUM_GUITAR];
    public AudioClip[] exileHarmonyClips   = new AudioClip[NUM_HARMONY];
    public AudioClip[] exileLeadClips      = new AudioClip[NUM_LEAD];
    public AudioClip[] exileWhispersClips  = new AudioClip[NUM_WHISPERS];

    // TODO: Samples from the other track

    private bool running = false;

    void Start() {

        for (int i = 0; i < 28; i++) {
            GameObject child = new GameObject("Player");
            child.transform.parent = gameObject.transform;
            audioSources[i] = child.AddComponent<AudioSource>();
        }

        // Load in all audio data
        loadAudioClips();

        Debug.Log("thing: " + exileGuitarClips[0]);

        // Trigger all clips to play at once (for now)

        int offset = 0;
        for (int i = 0; i < NUM_GUITAR; i++)   {
            audioSources[i].clip = exileGuitarClips[i];
        }
        offset += NUM_GUITAR - 1;
        for (int i = 0; i < NUM_HARMONY; i++)  { audioSources[offset + i].clip = exileHarmonyClips[i];  }
        offset += NUM_HARMONY - 1;
        for (int i = 0; i < NUM_LEAD; i++)     { audioSources[offset + i].clip = exileLeadClips[i];     }
        offset += NUM_LEAD - 1;
        for (int i = 0; i < NUM_WHISPERS; i++) { audioSources[offset + i].clip = exileWhispersClips[i]; }



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
        StartCoroutine(PlayTrack0Loop(0));
    }


    private IEnumerator PlayTrack0Loop(int phase) {

        // TODO: Generalize to all tracks
        while(true) {
            if (Markov.shouldPlay(phase, Track.Track0, 0)) {
                Debug.Log("ASDF Playing!");
                audioSources[0].Play();
            }
            yield return new WaitForSeconds(audioSources[0].clip.length);
        }
    }

    void Update() {
        if (!running) {
            return;
        }

        // TODO: May move track coroutines in here
    }

    private void loadAudioClips() {
        for (int i = 0; i < NUM_GUITAR; i++)   { exileGuitarClips[i]   = Resources.Load<AudioClip>("Stems/Guitar-" + (i + 1));   }
        for (int i = 0; i < NUM_HARMONY; i++)  { exileHarmonyClips[i]  = Resources.Load<AudioClip>("Stems/Harmony-" + (i + 1));    }
        for (int i = 0; i < NUM_LEAD; i++)     { exileLeadClips[i]     = Resources.Load<AudioClip>("Stems/LeadVocal-" + (i + 1));  }
        for (int i = 0; i < NUM_WHISPERS; i++) { exileWhispersClips[i] = Resources.Load<AudioClip>("Stems/Whispers-" + (i + 1));   }

    }
}

enum Track: int {
    Track0 = 0,
    Track1 = 1,
    Track2 = 2,
    Track3 = 3
}


class Markov {

    static readonly double[,] phase0prob;
    static readonly double[,] phase1prob;
    static readonly double[,,] phases;

    static Markov() {

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
        phase0prob = new double[,] {{ 1.0, 0.6, 0.4 },
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


        // phases = new double[,,] {phase0prob, phase1prob, {{}}};
        // TODO: Add more as implemented.
    }

    public static bool shouldPlay(int phase, Track track, int sample) {

        // FIXME: sample vs. track audiosources (needs med refactor)
        // FIXME: Global indices for track values vs. per-phase index
        // TODO: make dynamic phases (see constructor)

        double curProb = phase0prob[(int) track, sample];
        var randVal = Random.value;

        Debug.Log("ASDF curProb: " + curProb);
        Debug.Log("ASDF randVal: " + randVal);

        return randVal <= curProb;
    }

}
