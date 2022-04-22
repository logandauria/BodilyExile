

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
class MarkovTest: MonoBehaviour {

    // We need audioSources.length == clips.length because
    // in order to have multiple samples playing at the same time,
    // each needs its own `AudioSource`.
    // Rather than manually configuring each one to be mutually exclusive,
    // for now we'll just set n of them.
    private const int TOTAL_SOURCE = 44;
    private AudioSource[] audioSources = new AudioSource[TOTAL_SOURCE];

    // AudioClips from Bodily Exile (track1)

    public const int EXILE_NUM_GUITAR   = 7;
    public const int EXILE_NUM_HARMONY  = 6;
    public const int EXILE_NUM_LEAD     = 6;
    public const int EXILE_NUM_WHISPERS = 9;

    public const int EXILE_OFFSET = EXILE_NUM_GUITAR + EXILE_NUM_HARMONY + EXILE_NUM_LEAD + EXILE_NUM_WHISPERS;

    public AudioClip[,] exileClips;
    public AudioClip[] exileGuitarClips    = new AudioClip[EXILE_NUM_GUITAR];
    public AudioClip[] exileHarmonyClips   = new AudioClip[EXILE_NUM_HARMONY];
    public AudioClip[] exileLeadClips      = new AudioClip[EXILE_NUM_LEAD];
    public AudioClip[] exileWhispersClips  = new AudioClip[EXILE_NUM_WHISPERS];

    // AudioClips from Milky Blue (track2)

    public const int BLUE_NUM_GUITAR   = 6;
    public const int BLUE_NUM_HARMONY  = 6;
    public const int BLUE_NUM_LEAD     = 4;

    public AudioClip[,] blueClips;
    public AudioClip[] blueGuitarClips    = new AudioClip[BLUE_NUM_GUITAR];
    public AudioClip[] blueHarmonyClips   = new AudioClip[BLUE_NUM_HARMONY];
    public AudioClip[] blueLeadClips      = new AudioClip[BLUE_NUM_LEAD];

    // TODO: Samples from the other track

    private bool running = false;

    void Start() {

        GameObject child = new GameObject("Player");
        child.transform.parent = gameObject.transform;

        for (int i = 0; i < TOTAL_SOURCE; i++) {
            audioSources[i] = child.AddComponent<AudioSource>();
        }

        // Load in all audio data
        loadAudioClips();
        running = true;

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
        StartCoroutine(PlayTrack(0, Track.BlueGuitar, 0));

    }

    private void setupPhase1() {

        // StopCoroutine(PlayTrack(Track.ExileLead, 0));
        // StopCoroutine(PlayTrack(Track.ExileHarmony, 0));

        StartCoroutine(PlayTrack(1, Track.ExileLead, 0));
    }


    private IEnumerator PlayTrack(int phase, Track track, int sample) {

        while(true) {

            // Default curSource
            AudioSource curSource = audioSources[0];

            // Phase 0 has two vocals, so, attempt to play the two vocals
            if (Markov.shouldPlay(phase, track, sample)) {
                Debug.Log("[Markov] Playing!");
                switch (track) {
                    case Track.ExileGuitar: {
                        Debug.Log("[Markov] Playing ExileGuitar!");
                        curSource = audioSources[0 + sample];
                        curSource.Play();
                        break;
                    }
                    case Track.ExileHarmony: {
                        Debug.Log("[Markov] Playing ExileHarmony!");
                        curSource = audioSources[EXILE_NUM_GUITAR + sample];
                        curSource.Play();
                        break;
                    }
                    case Track.ExileLead: {
                        Debug.Log("[Markov] Playing ExileLead!");
                        curSource = audioSources[EXILE_NUM_GUITAR + EXILE_NUM_HARMONY + sample];
                        curSource.Play();
                        break;
                    }
                    case Track.ExileWhisper: {
                        Debug.Log("[Markov] Playing ExileWhisper!");
                        curSource = audioSources[EXILE_NUM_GUITAR + EXILE_NUM_HARMONY + EXILE_NUM_LEAD + sample];
                        curSource.Play();
                        break;
                    }
                    case Track.BlueGuitar: {
                        Debug.Log("[Markov] Playing BlueGuitar!");
                        curSource = audioSources[EXILE_OFFSET + sample];
                        curSource.Play();
                        break;
                    }
                    default: {
                        Debug.LogError("[Markov] Unable to play phase " + phase + ", track " + track + ", sample " + sample);
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(curSource.clip.length);
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

        // MARK: Exile

        for (int i = 0; i < EXILE_NUM_GUITAR; i++)   {
            exileGuitarClips[i] = Resources.Load<AudioClip>("AudioStems/Exile/Guitar-" + (i + 1));
            audioSources[i].clip = exileGuitarClips[i];
        }

        offset += EXILE_NUM_GUITAR;
        for (int i = 0; i < EXILE_NUM_HARMONY; i++)  {
            exileHarmonyClips[i] = Resources.Load<AudioClip>("AudioStems/Exile/Harmony-" + (i + 1));
            audioSources[offset + i].clip = exileHarmonyClips[i];
        }

        offset += EXILE_NUM_HARMONY;
        for (int i = 0; i < EXILE_NUM_LEAD; i++)     {
            exileLeadClips[i] = Resources.Load<AudioClip>("AudioStems/Exile/LeadVocal-" + (i + 1));
            audioSources[offset + i].clip = exileLeadClips[i];
        }

        offset += EXILE_NUM_LEAD;
        for (int i = 0; i < EXILE_NUM_WHISPERS; i++) {
            exileWhispersClips[i] = Resources.Load<AudioClip>("AudioStems/Exile/Whispers-" + (i + 1));
            audioSources[offset + i].clip = exileWhispersClips[i];
        }

        // MARK: Blue

        // Note: re-using above offset as `audioSources` is "globally" indexed
        // between the two tracks.

        for (int i = 0; i < BLUE_NUM_GUITAR; i++)   {
            blueGuitarClips[i] = Resources.Load<AudioClip>("AudioStems/Blue/Guitar-" + (i + 1));
            audioSources[i].clip = blueGuitarClips[i];
        }

        offset += BLUE_NUM_GUITAR;
        for (int i = 0; i < BLUE_NUM_HARMONY; i++)  {
            blueHarmonyClips[i] = Resources.Load<AudioClip>("AudioStems/Blue/Harmony-" + (i + 1));
            audioSources[offset + i].clip = blueHarmonyClips[i];
        }

        offset += BLUE_NUM_HARMONY;
        for (int i = 0; i < BLUE_NUM_LEAD; i++)     {
            blueLeadClips[i] = Resources.Load<AudioClip>("AudioStems/Blue/Lead-" + (i + 1));
            audioSources[offset + i].clip = blueLeadClips[i];
        }

    }
}

/// The type of track per song
enum Track: int {
    ExileGuitar  = 0,
    ExileHarmony = 1,
    ExileLead    = 2,
    ExileWhisper = 3,

    BlueGuitar   = 4,
    BlueHarmony  = 5,
    BlueLead     = 6
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
                                    { 0.9, 0.9, 0.1 }};


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
        // TODO: Clip Delays??
        //  -- opt1: Manualy re-export sound to include fixed delay
        //  -- opt2: PlayScheduled api >:(
        //  -- opt3: Thread.sleep
        //  -- opt4: The church down the street has a Friday service

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
                case Track.ExileLead:    { return 0; break;  } // FIXME: set to 0 for testing
                case Track.ExileWhisper: { return -1; break; }
                case Track.BlueGuitar:   { return 1; break;  } // FIXME: set to 1 for testing (flip above)
                default: break;
            }
        } else if (phase == 1) {
            // TODO
        }
        return 0;
    }
}
