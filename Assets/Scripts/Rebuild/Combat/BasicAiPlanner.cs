using System;
using UnityEngine;

namespace Rei.Runtime.Duel
{
    [Serializable]
    public struct BasicAiPlan
    {
        public DuelMove InitialMove;
        public float InitialDelay;
        public bool AttemptCounter;
        public DuelMove CounterMove;
        public float CounterDelay;
    }

    public static class BasicAiPlanner
    {
        public static BasicAiPlan CreatePlan(DuelRoundConfig config, System.Random random)
        {
            return new BasicAiPlan
            {
                InitialMove = RandomMove(random),
                InitialDelay = RandomRange(random, config.aiInitialMinDelay, config.aiInitialMaxDelay),
                AttemptCounter = random.NextDouble() <= config.aiCounterChance,
                CounterMove = RandomMove(random),
                CounterDelay = RandomRange(random, config.aiCounterMinDelay, config.aiCounterMaxDelay)
            };
        }

        private static DuelMove RandomMove(System.Random random)
        {
            return (DuelMove)random.Next(1, 4);
        }

        private static float RandomRange(System.Random random, float min, float max)
        {
            if (max <= min)
            {
                return min;
            }

            return min + ((float)random.NextDouble() * (max - min));
        }
    }
}
