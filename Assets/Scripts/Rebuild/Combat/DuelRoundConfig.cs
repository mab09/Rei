using UnityEngine;

namespace Rei.Runtime.Duel
{
    [CreateAssetMenu(menuName = "Rei/Duel Round Config", fileName = "DuelRoundConfig")]
    public class DuelRoundConfig : ScriptableObject
    {
        [Header("Round Flow")]
        [Min(0.1f)] public float beatInterval = 1f;
        [Min(0.1f)] public float randomThirdBeatMinDelay = 0.75f;
        [Min(0.1f)] public float randomThirdBeatMaxDelay = 2f;
        [Range(0f, 0.5f)] public float speedAdvantageThreshold = 0.08f;
        [Min(0.01f)] public float counterWindowDuration = 0.2f;

        [Header("Presentation")]
        [Min(0.1f)] public float moveSpeed = 7f;
        [Min(0f)] public float strikeDistance = 0f;
        [Min(0f)] public float passThroughDistance = 5f;

        [Header("AI")]
        [Min(0f)] public float aiInitialMinDelay = 0.05f;
        [Min(0f)] public float aiInitialMaxDelay = 0.75f;
        [Range(0f, 1f)] public float aiCounterChance = 0.5f;
        [Min(0f)] public float aiCounterMinDelay = 0.09f;
        [Min(0f)] public float aiCounterMaxDelay = 0.2f;

        private void OnValidate()
        {
            beatInterval = Mathf.Max(0.1f, beatInterval);
            randomThirdBeatMinDelay = Mathf.Max(0.1f, randomThirdBeatMinDelay);
            speedAdvantageThreshold = Mathf.Clamp(speedAdvantageThreshold, 0f, 0.5f);
            counterWindowDuration = Mathf.Max(0.01f, counterWindowDuration);
            moveSpeed = Mathf.Max(0.1f, moveSpeed);
            strikeDistance = Mathf.Max(0f, strikeDistance);
            passThroughDistance = Mathf.Max(0f, passThroughDistance);

            if (randomThirdBeatMaxDelay < randomThirdBeatMinDelay)
            {
                randomThirdBeatMaxDelay = randomThirdBeatMinDelay;
            }

            if (aiInitialMaxDelay < aiInitialMinDelay)
            {
                aiInitialMaxDelay = aiInitialMinDelay;
            }

            if (aiCounterMaxDelay < aiCounterMinDelay)
            {
                aiCounterMaxDelay = aiCounterMinDelay;
            }

            if (aiCounterMinDelay < speedAdvantageThreshold)
            {
                aiCounterMinDelay = speedAdvantageThreshold;
            }

            if (aiCounterMaxDelay < speedAdvantageThreshold)
            {
                aiCounterMaxDelay = speedAdvantageThreshold;
            }
        }
    }
}
