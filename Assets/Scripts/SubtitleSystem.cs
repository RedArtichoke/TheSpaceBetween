using UnityEngine;
using TMPro;
using System.Collections;

// This makes the component appear under UI menu in Unity for easy addition
[AddComponentMenu("UI/Subtitle Text")]
public class SubtitleText : TextMeshProUGUI
{
    private bool isPlaying = false;
    private Coroutine currentSequence;

    // Struct to hold subtitle data
    [System.Serializable]
    public struct SubtitleLine
    {
        public string text;
        public float delay;

        public SubtitleLine(string text, float delay)
        {
            this.text = text;
            this.delay = delay;
        }
    }

    /*——————————————————————————————————————————————————————————————————————————————————————————————————————————————
    Automated Suit System
    ——————————————————————————————————————————————————————————————————————————————————————————————————————————————*/

    private SubtitleLine[] suit100 = new SubtitleLine[]
    {
        new SubtitleLine("Suit power levels restored to 100%.", 3.0f),
    };

    private SubtitleLine[] suit75 = new SubtitleLine[]
    {
        new SubtitleLine("Suit Power Levels at 75%.", 2.5f),
    };

    private SubtitleLine[] suit50 = new SubtitleLine[]
    {
        new SubtitleLine("Suit Power Levels at 50%.", 2.5f),
    };

    private SubtitleLine[] suit25 = new SubtitleLine[]
    {
       new SubtitleLine("Suit Power Levels at 25%.", 2.5f),
    };

    private SubtitleLine[] suit10 = new SubtitleLine[]
    {
        new SubtitleLine("Warning: Suit Power Levels are below 10%.", 5.71f),
    };

    private SubtitleLine[] suitDamaged = new SubtitleLine[]
    {
        new SubtitleLine("Warning: Critical Injury Sustained, seek safety immediately.", 4.5f),
    };
    
    private SubtitleLine[] suitEnemy = new SubtitleLine[]
    {
        new SubtitleLine("Warning, unknown lifeform detected.", 3.5f),
    };
    
    private SubtitleLine[] suitShifter = new SubtitleLine[]
    {
        new SubtitleLine("The dimension shifter has been acquired, the dark dimension can help you find your ship components.", 10.5f),
    };

    private SubtitleLine[] suitHeal = new SubtitleLine[]
    {
        new SubtitleLine("Automatic medical systems engaged.", 2.5f),
    };

    public void PlaySuit100()
    {
        PlaySubtitles(suit100);
        Debug.Log("Playing suit 100");
    }

    public void PlaySuit75()
    {
        PlaySubtitles(suit75);
        Debug.Log("Playing suit 75");
    }

    public void PlaySuit50()
    {
        PlaySubtitles(suit50);
        Debug.Log("Playing suit 50");
    }   

    public void PlaySuit25()
    {
        PlaySubtitles(suit25);
        Debug.Log("Playing suit 25");
    }   

    public void PlaySuit10()
    {
        PlaySubtitles(suit10);
        Debug.Log("Playing suit 10");
    }   

    public void PlaySuitDamaged()
    {
        PlaySubtitles(suitDamaged);
        Debug.Log("Playing suit damaged");
    }   

    public void PlaySuitEnemy()
    {
        PlaySubtitles(suitEnemy);
        Debug.Log("Playing suit enemy");
    }      

    public void PlaySuitShifter()
    {
        PlaySubtitles(suitShifter);
        Debug.Log("Playing suit shifter");
    }      
    
    public void PlaySuitHeal()
    {
        PlaySubtitles(suitHeal);
        Debug.Log("Playing suit heal");
    }


    /*——————————————————————————————————————————————————————————————————————————————————————————————————————————————
    Intro Sequence
    ——————————————————————————————————————————————————————————————————————————————————————————————————————————————*/

    // Stored sequence for intro dialogue
    private SubtitleLine[] introSequence = new SubtitleLine[]
    {
        new SubtitleLine("", 5.65f),
        new SubtitleLine("Good morning captain, all systems are optimal and ready for today's ___.", 4.53f),
        new SubtitleLine("The ship's diagnostics confirm that propulsion, __________, and life support are fully operational", 6.1f),
        new SubtitleLine("and our current trajectory is set toward the Intervallum Research Freighter.", 4.0f),
        new SubtitleLine("According to the limited data available, you will need to wear your R.A.M.P suit for this ______ mission.", 6.13f),
        new SubtitleLine("Your R.A.M.P suit is located in the closet; please proceed to put it on.", 5.41f),
        new SubtitleLine("", 1.0f)
    };

    // RAMP suit sequence
    private SubtitleLine[] rampSuitSequence = new SubtitleLine[]
    {
        new SubtitleLine("Before we begin, let's verify that the suit systems are in proper order.", 5.55f),
        new SubtitleLine("Please turn on your flashlight.", 2.13f),
    };

    // Flashbang sequence
    private SubtitleLine[] flashbangSequence = new SubtitleLine[]
    {
        new SubtitleLine("Next, check that the flash defensive systems are operational.", 5.4f),
    };

    // Power drain sequence
    private SubtitleLine[] powerDrainSequence = new SubtitleLine[]
    {
        new SubtitleLine("Excellent. The R.A.M.P suit lighting systems are functioning correctly.", 6.71f),
        new SubtitleLine("Remember, using these systems will drain the suit's power levels, so manage them carefully.", 6.19f),
        new SubtitleLine("Before departure, replenish your suit's battery by retrieving a spare from the boxes.", 5.23f)
    };

    // Battery sequence
    private SubtitleLine[] landingSequence = new SubtitleLine[]
    {
        new SubtitleLine("When you are ready to board the Intervallum, press the button to initiate the landing sequence.", 7.0f)
    };

    // Play a sequence of subtitles
    public void PlaySubtitles(SubtitleLine[] lines)
    {
        // Don't start if already playing or if subtitles are disabled
        if (isPlaying || !settingsManager.SubtitlesEnabled) return;

        currentSequence = StartCoroutine(PlaySubtitleSequence(lines));
    }

    // Clear subtitles and stop current sequence
    public void ClearSubtitles()
    {
        if (currentSequence != null)
        {
            StopCoroutine(currentSequence);
        }
        
        text = "";
        isPlaying = false;
    }

    private IEnumerator PlaySubtitleSequence(SubtitleLine[] lines)
    {
        isPlaying = true;

        foreach (SubtitleLine line in lines)
        {
            // Check if subtitles got disabled during playback
            if (!settingsManager.SubtitlesEnabled)
            {
                text = "";
                isPlaying = false;
                yield break;
            }

            text = line.text;
            yield return new WaitForSeconds(line.delay);
        }

        // Clear final subtitle and reset
        text = "";
        isPlaying = false;
    }

    // Public method to play the intro sequence
    public void PlayIntroSequence()
    {
        PlaySubtitles(introSequence);
        Debug.Log("Playing intro sequence");
    }

    // Public method to play the RAMP suit sequence
    public void PlayRampSuitSequence()
    {
        PlaySubtitles(rampSuitSequence);
        Debug.Log("Playing RAMP suit sequence");
    }
    
    // Public method to play the flashbang sequence
    public void PlayFlashbangSequence()
    {
        PlaySubtitles(flashbangSequence);
        Debug.Log("Playing flashbang sequence");
    }

    // Public method to play the power drain sequence
    public void PlayPowerDrainSequence()
    {
        PlaySubtitles(powerDrainSequence);
        Debug.Log("Playing power drain sequence");
    }
    
    // Public method to play the battery sequence
    public void PlayLandingSequence()
    {
        PlaySubtitles(landingSequence);
        Debug.Log("Playing landing sequence");
    }
} 