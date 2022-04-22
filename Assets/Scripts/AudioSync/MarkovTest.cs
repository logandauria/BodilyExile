

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
class MarkovTest: MonoBehaviour {

    // We need audioSources.length == clips.length because
    // in order to have multiple samples playing at the same time,
    // each needs its own `AudioSource`.
    // Rather than manually configuring each one to be mutually exclusive,
    // for now we'll just set n of them.
    private AudioSource[] audioSources = new AudioSource[28];

    // AudioClips from Bodily Exile (track1)

    public const int NUM_GUITAR   = 7;
    public const int NUM_HARMONY  = 6;
    public const int NUM_LEAD     = 6;
    public const int NUM_WHISPERS = 9;

    public AudioClip[,] exileClips;
    public AudioClip[] exileGuitarClips    = new AudioClip[NUM_GUITAR];
    public AudioClip[] exileHarmonyClips   = new AudioClip[NUM_HARMONY];
    public AudioClip[] exileLeadClips      = new AudioClip[NUM_LEAD];
    public AudioClip[] exileWhispersClips  = new AudioClip[NUM_WHISPERS];

    // TODO: Samples from the other track

    private bool running = false;

    void Start() {

        GameObject child = new GameObject("Player");
        child.transform.parent = gameObject.transform;

        for (int i = 0; i < 28; i++) {
            audioSources[i] = child.AddComponent<AudioSource>();
        }

        // Load in all audio data
        loadAudioClips();
        running = true;



        // exileClips = new AudioClip[,];
        // (Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length);
        // Array.Copy(exileGuitarClips)

        // damn



        // TODO: Make into array of coroutines or something

        // Play track0 on a simulated loop,
        // re-triggering after each interval.
        //
        // Note that the system needs around a ~1s buffer to
        // load the file into memory (src: https://bit.ly/35EWiI4),
        // so timers should be triggered to start *after* all
        // sounds have been initialized.
        setupPhase0();
    }

    private void setupPhase0() {

        // Phase 0 is defined to play two vocal tracks
        StartCoroutine(PlayTrack(0, Track.ExileLead, 0));
        StartCoroutine(PlayTrack(0, Track.ExileHarmony, 0));

    }

    private void setupPhase1() {

        // StopCoroutine(PlayTrack(Track.ExileLead, 0));
        // StopCoroutine(PlayTrack(Track.ExileHarmony, 0));

        StartCoroutine(PlayTrack(1, Track.ExileLead, 0));
    }


    private IEnumerator PlayTrack(int phase, Track track, int sample) {

        while(true) {

            // Phase 0 has two vocals, so, attempt to play the two vocals
            if (Markov.shouldPlay(phase, track, sample)) {
                Debug.Log("[Markov] Playing!");
                switch (track) {
                    case Track.ExileGuitar: {
                        Debug.Log("[Markov] Playing Guitar!");
                        audioSources[0 + sample].Play();
                        break;
                    }
                    case Track.ExileHarmony: {
                        Debug.Log("[Markov] Playing Harmony!");
                        audioSources[NUM_GUITAR + sample].Play();
                        break;
                    }
                    case Track.ExileLead: {
                        Debug.Log("[Markov] Playing Lead!");
                        audioSources[NUM_GUITAR + NUM_HARMONY + sample].Play();
                        break;
                    }
                    case Track.ExileWhisper: {
                        Debug.Log("[Markov] Playing Whisper!");
                        audioSources[NUM_GUITAR + NUM_HARMONY + NUM_LEAD + sample].Play();
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(audioSources[0].clip.length);
        }
    }

    void Update() {
        if (!running) {
            return;
        }

        // TODO: May move track coroutines in here
        // FIXME: Starting coroutines in Start() vs. Update()?
        // Enable playing of audio now that we've initialized everything
    }

    private void loadAudioClips() {
        int offset = 0;

        for (int i = 0; i < NUM_GUITAR; i++)   {
            exileGuitarClips[i] = Resources.Load<AudioClip>("AudioStems/Guitar-" + (i + 1));
            audioSources[i].clip = exileGuitarClips[i];
        }

        offset += NUM_GUITAR;
        for (int i = 0; i < NUM_HARMONY; i++)  {
            exileHarmonyClips[i] = Resources.Load<AudioClip>("AudioStems/Harmony-" + (i + 1));
            audioSources[offset + i].clip = exileHarmonyClips[i];
        }

        offset += NUM_HARMONY;
        for (int i = 0; i < NUM_LEAD; i++)     {
            exileLeadClips[i] = Resources.Load<AudioClip>("AudioStems/LeadVocal-" + (i + 1));
            audioSources[offset + i].clip = exileLeadClips[i];
        }

        offset += NUM_LEAD;
        for (int i = 0; i < NUM_WHISPERS; i++) {
            exileWhispersClips[i] = Resources.Load<AudioClip>("AudioStems/Whispers-" + (i + 1));
            audioSources[offset + i].clip = exileWhispersClips[i];
        }

    }
}

/// The type of track per song
enum Track: int {
    ExileGuitar  = 0,
    ExileHarmony = 1,
    ExileLead    = 2,
    ExileWhisper = 3
    // TODO: More for MilkyBlue
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
        //  {                  sample1 sample2 sample3
        //   track1-lead     {   0.5     0.6    0.4    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   guitar          {   0.8     0.2    0.0    }
        //  }
        //
        //
        phase1prob = new double[,] {{ 0.5, 0.6, 0.4 },
                                    { 0.5, 0.4, 0.1 },
                                    { 0.8, 0.2, 0.0 }};


        // Phase 2 - (two vocals + guitar + other vocals )
        //           Two vocals interact w.h.p, guitar is constant
        //
        //  {                  sample1 sample2 sample3
        //   track1-lead     {   0.5     0.6    0.4    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   guitar          {   0.8     0.2    0.0    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //  }
        //
        //
        phase1prob = new double[,] {{ 0.5, 0.6, 0.4 },
                                    { 0.5, 0.4, 0.1 },
                                    { 0.8, 0.2, 0.0 }};




        // phases = new double[,,] {phase0prob, phase1prob, {{}}};
        // TODO: Add more as implemented.
    }

    /// Determines if a given track+sample should be played
    /// on a phase.
    public static bool shouldPlay(int phase, Track track, int sample) {

        // TODO: make dynamic phases (see constructor)

        var index = mapTrackToIndex(track, phase);
        if (index == -1) {
            Debug.LogError("[Markov] Invalid index for phase" + phase + ", track " + track + ", sample " + sample);
            return false;
        }

        Debug.Log("[Markov] phase " + phase + ", track " + track + ", sample " + sample);
        double curProb = phase0prob[index, sample];
        var randVal = Random.value;
        return randVal <= curProb;
    }

    /// A little hacky -- maps track indices to the probability matrices defined
    /// manually in the constructor.
    private static int mapTrackToIndex(Track track, int phase) {
        if (phase == 0) {
            switch (track) {
                case Track.ExileGuitar:  { return -1; break; }
                case Track.ExileHarmony: { return 0; break;  }
                case Track.ExileLead:    { return 1; break;  }
                case Track.ExileWhisper: { return -1; break; }
            }
        } else if (phase == 1) {
            // TODO
        }
        return 0;
    }
}
