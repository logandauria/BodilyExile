

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
class MarkovTest: MonoBehaviour {

    // We need audioSources.length == clips.length because
    // in order to have multiple samples playing at the same time,
    // each needs its own `AudioSource`.
    // Rather than manually configuring each one to be mutually exclusive,
    // for now we'll just set n of them.
    private const int TOTAL_SOURCE = 46;
    private AudioSource[] audioSources = new AudioSource[TOTAL_SOURCE];

    // AudioClips from Bodily Exile (track1)

    public const int EXILE_NUM_GUITAR   = 8;
    public const int EXILE_NUM_HARMONY  = 6;
    public const int EXILE_NUM_LEAD     = 7;
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


    private bool running = false;
    private List<Coroutine> runningPhases = new List<Coroutine>();

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

        // TODO: @Logan Hook into Event system
        startPhase1Part1();
    }


    /// Stop any running phases (coroutines)
    private void clearPhases() {
        foreach (Coroutine coroutine in runningPhases) {
            StopCoroutine(coroutine);
        }
    }

    // MARK: Public phase hooks. **Must** call clearPhase() at start of each phase to prevent overlap.

    public void startPhase0() {
        clearPhases();
    }

    // Phase 1

    public void startPhase1Part0() {
        clearPhases();
        runningPhases.Add(StartCoroutine(PlayTrack(10, Track.BlueHarmony, 1)));
        runningPhases.Add(StartCoroutine(PlayTrack(10, Track.ExileLead, 7)));
        runningPhases.Add(StartCoroutine(PlayTrack(10, Track.ExileGuitar, 8)));
    }

    public void startPhase1Part1() {
        clearPhases();
        runningPhases.Add(StartCoroutine(PlayTrack(11, Track.BlueHarmony,  1)));
        runningPhases.Add(StartCoroutine(PlayTrack(11, Track.ExileLead,    7)));
        runningPhases.Add(StartCoroutine(PlayTrack(11, Track.ExileHarmony, 1)));
        runningPhases.Add(StartCoroutine(PlayTrack(11, Track.ExileHarmony, 2)));
        runningPhases.Add(StartCoroutine(PlayTrack(11, Track.ExileHarmony, 3)));
        runningPhases.Add(StartCoroutine(PlayTrack(11, Track.ExileWhisper, 2)));
        runningPhases.Add(StartCoroutine(PlayTrack(11, Track.ExileGuitar,  8)));
    }

    public void startPhase1Part2() {
        clearPhases();
        runningPhases.Add(StartCoroutine(PlayTrack(12, Track.BlueHarmony,  1)));
        runningPhases.Add(StartCoroutine(PlayTrack(12, Track.ExileHarmony, 1)));
        runningPhases.Add(StartCoroutine(PlayTrack(12, Track.ExileHarmony, 2)));
        runningPhases.Add(StartCoroutine(PlayTrack(12, Track.ExileHarmony, 3)));
        runningPhases.Add(StartCoroutine(PlayTrack(12, Track.ExileHarmony, 4)));
        runningPhases.Add(StartCoroutine(PlayTrack(12, Track.ExileGuitar,  8)));
    }

    // Phase 2

    public void startPhase2Part0() {
        clearPhases();
        // TODO
    }
    public void startPhase3Part0() {
        clearPhases();
        // TODO
    }

    public void startPhase4() {
        clearPhases();
        // TODO: Cute piano cover of Bodily Exile
    }


    private IEnumerator PlayTrack(int phase, Track track, int sample) {

        // Turn into zero-indexed for simpler array indexing.
        sample -= 1;

        while(true) {

            // Default curSource
            AudioSource curSource = audioSources[0];

            // Undo zero index for shouldPlay call
            if (Markov.shouldPlay(phase, track, sample + 1)) {
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
                    case Track.BlueHarmony: {
                        Debug.Log("[Markov] Playing BlueHarmony!");
                        curSource = audioSources[EXILE_OFFSET + BLUE_NUM_GUITAR + sample];
                        curSource.Play();
                        break;
                    }
                    case Track.BlueLead: {
                        Debug.Log("[Markov] Playing BlueLead!");
                        curSource = audioSources[EXILE_OFFSET + BLUE_NUM_GUITAR + BLUE_NUM_HARMONY + sample];
                        curSource.Play();
                        break;
                    }
                    default: {
                        Debug.LogError("[Markov] Unable to play phase " + phase + ", track " + track + ", sample " + sample);
                        break;
                    }
                }
            }

            // Retrigger the current sample's chance to play
            // at the end of every sample duration,
            // regardless if it played on the current cycle or not.
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

        offset += EXILE_NUM_WHISPERS;

        // MARK: Blue

        // Note: re-using above offset as `audioSources` is "globally" indexed
        // between the two tracks.

        for (int i = 0; i < BLUE_NUM_GUITAR; i++)   {
            blueGuitarClips[i] = Resources.Load<AudioClip>("AudioStems/Blue/Guitar-" + (i + 1));
            audioSources[offset + i].clip = blueGuitarClips[i];
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

    // TODO: Add piano cover track
}


class Markov {

    static readonly double[,] phase0prob;
    static readonly double[,] phase1part0prob;
    static readonly double[,] phase1part1prob;
    static readonly double[,] phase1part2prob;
    static readonly double[,] phase2prob;
    static readonly double[,] phase3prob;
    static readonly double[,] phase4prob;

    static Markov() {

        // We need to create probability matrices
        // for *each* phase of the project.

        // Phase 0 - (opening scene)
        //           Nothing is playing.
        //
        //  {}
        //
        //
        phase0prob = new double[,] {{}};


        // Phase 1 - (two vocals + guitar)
        //           Two vocals interact w.h.p, guitar is constant
        //           (flattened down)

        // This needs to be this long as the sample is an absolute index,
        // and writing code to convert this index would bring
        // the total number of index-conversion functions up to 3,
        // which is too damn high.
        //
        //                                  1    2    3    4    5    6    7    8
        phase1part0prob = new double[,] {{ 0.3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0  }, // blue-harmony
                                         { 0.9, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0  }, // exile-lead
                                         { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, }, // exile-harmony
                                         { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, }, // exile-whisper
                                         { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0 }}; // exile-guitar

        //                                  1    2    3    4    5    6    7    8
        phase1part1prob = new double[,] {{ 0.3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0  }, // blue-harmony
                                         { 0.9, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0  }, // exile-lead
                                         { 0.1, 0.3, 0.4, 0.0, 0.0, 0.0, 0.0, 0.0, }, // exile-harmony
                                         { 0.0, 0.2, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, }, // exile-whisper
                                         { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0 }}; // exile-guitar

        //                                  1    2    3    4    5    6    7    8
        phase1part2prob = new double[,] {{ 0.3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0  }, // blue-harmony
                                         { 0.9, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0  }, // exile-lead
                                         { 0.7, 0.3, 0.4, 0.3, 0.4, 0.5, 0.0, 0.0, }, // exile-harmony
                                         { 0.0, 0.2, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, }, // exile-whisper
                                         { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 }}; // exile-guitar


        // Phase 2 - (two vocals + guitar + other vocals )
        //           Two vocals interact w.h.p, guitar is constant
        //
        //                             1    2    3    4    5    6    7    8
        phase2prob = new double[,] {{ 0.5, 0.6, 0.4, 0.0, 0.0, 0.0, 0.0, 0.0},  // blue-lead
                                    { 0.5, 0.4, 0.1, 0.3, 0.5, 0.3, 0.0, 0.0},  // exile-harmony
                                    { 0.8, 0.2, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}}; // guitar

        // Phase 3 - (two vocals + guitar + other vocals )
        //           Two vocals interact w.h.p, guitar is constant
        //
        //  {                  sample1 sample2 sample3
        //   exile-lead      {   0.5     0.6    0.4    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   guitar          {   0.8     0.2    0.0    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //   track2-harmony  {   0.5     0.4    0.1    }
        //  }
        //
        //
        phase2prob = new double[,] {{ 0.5, 0.6, 0.4 },
                                    { 0.5, 0.4, 0.1 },
                                    { 0.8, 0.2, 0.0 }};

        // Phase 4 - (credits)
        //           Just a cute piano cover of Bodily Exile.
        //
        //  {                  sample1
        //   piano-cover     {   1.0   }
        //  }
        //
        //
        phase2prob = new double[,] {{ 1.0 }};

    }

    /// Determines if a given track+sample should be played
    /// on a phase.
    ///
    /// Note: Samples are NOT ZERO INDEXED.
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
        double curProb = 0.0;
        double[,] curProbArr = phase0prob;

        // Pick prob array to use
        switch (phase) {
            case 0:  curProbArr = phase0prob;      break;
            case 10: curProbArr = phase1part0prob; break;
            case 11: curProbArr = phase1part1prob; break;
            case 12: curProbArr = phase1part2prob; break;
            case 2:  curProbArr = phase2prob;      break;
            case 3:  curProbArr = phase3prob;      break;
            case 4:  curProbArr = phase4prob;      break;
            default: break;
        }

        // Retrieve probability from array
        curProb = curProbArr[index, sample - 1];

        var randVal = Random.value;
        return randVal <= curProb;
    }

    /// A little hacky -- maps track indices to the probability matrices defined
    /// manually in the constructor.
    private static int mapTrackToIndex(Track track, int phase) {
        if (phase == 10 || phase == 11 || phase == 12) {
            switch (track) {
                case Track.BlueHarmony:  { return 0; break; }
                case Track.ExileLead:    { return 1; break; }
                case Track.ExileHarmony: { return 2; break; }
                case Track.ExileWhisper: { return 3; break; }
                case Track.ExileGuitar:  { return 4; break; }
                default: break;
            }
        } else if (phase == 2) {
            // TODO
        }
        return 0;
    }
}
