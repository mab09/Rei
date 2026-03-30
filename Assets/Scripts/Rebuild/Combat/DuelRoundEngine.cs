using System;

namespace Rei.Runtime.Duel
{
    public sealed class DuelRoundEngine
    {
        private struct PlayerState
        {
            public bool PrematureCommitted;
            public bool HasInitialMove;
            public DuelMove InitialMove;
            public float InitialMoveTime;
            public bool HasCounterMove;
            public DuelMove CounterMove;
            public float CounterMoveTime;

            public DuelPlayerSnapshot ToSnapshot()
            {
                return new DuelPlayerSnapshot
                {
                    PrematureCommitted = PrematureCommitted,
                    HasInitialMove = HasInitialMove,
                    InitialMove = InitialMove,
                    InitialMoveTime = InitialMoveTime,
                    HasCounterMove = HasCounterMove,
                    CounterMove = CounterMove,
                    CounterMoveTime = CounterMoveTime
                };
            }
        }

        private readonly DuelRoundConfig config;
        private readonly PlayerState[] players = new PlayerState[2];
        private readonly System.Random random = new System.Random();

        private DuelPhase phase = DuelPhase.Idle;
        private float totalTime;
        private float phaseStartTime;
        private float thirdBeatTime;
        private PlayerSlot? aggressor;
        private PlayerSlot? winner;
        private DuelResolutionReason resolutionReason = DuelResolutionReason.None;

        public DuelRoundEngine(DuelRoundConfig config)
        {
            this.config = config;
        }

        public DuelRoundSnapshot Snapshot => BuildSnapshot();

        public void StartRound()
        {
            totalTime = 0f;
            phaseStartTime = 0f;
            phase = DuelPhase.TakeYourStance;
            thirdBeatTime = config.beatInterval + RandomThirdBeatDelay();
            aggressor = null;
            winner = null;
            resolutionReason = DuelResolutionReason.None;

            for (var i = 0; i < players.Length; i++)
            {
                players[i] = default;
            }
        }

        public void Tick(float deltaTime)
        {
            if (phase == DuelPhase.Idle || phase == DuelPhase.End)
            {
                return;
            }

            totalTime += Math.Max(0f, deltaTime);
        }

        public bool SubmitCommand(PlayerSlot slot, DuelCommandKind kind, DuelMove move)
        {
            if (move == DuelMove.None || phase == DuelPhase.Idle || phase == DuelPhase.End || phase == DuelPhase.Resolution)
            {
                return false;
            }

            if (kind != DuelCommandKind.InitialAttack)
            {
                return false;
            }

            var index = (int)slot;
            var player = players[index];

            if (player.PrematureCommitted)
            {
                return false;
            }

            if (phase == DuelPhase.TakeYourStance && !AttacksAllowed())
            {
                player.PrematureCommitted = true;
                player.InitialMove = move;
                player.InitialMoveTime = totalTime;
                players[index] = player;
                return true;
            }

            if (phase == DuelPhase.Attack)
            {
                return SubmitAttackPhaseCommand(slot, move, ref player);
            }

            if (phase != DuelPhase.TakeYourStance && phase != DuelPhase.Attack)
            {
                return false;
            }

            player.HasInitialMove = true;
            player.InitialMove = move;
            player.InitialMoveTime = totalTime;
            players[index] = player;

            if (phase == DuelPhase.TakeYourStance)
            {
                aggressor = slot;
                TransitionTo(DuelPhase.Attack);
            }

            return true;
        }

        private bool SubmitAttackPhaseCommand(PlayerSlot slot, DuelMove move, ref PlayerState player)
        {
            if (!aggressor.HasValue || aggressor.Value == slot)
            {
                return false;
            }

            if (player.HasInitialMove || player.HasCounterMove)
            {
                return false;
            }

            var attackDelta = totalTime - players[(int)aggressor.Value].InitialMoveTime;
            if (attackDelta <= config.speedAdvantageThreshold)
            {
                player.HasInitialMove = true;
                player.InitialMove = move;
                player.InitialMoveTime = totalTime;
                players[(int)slot] = player;
                return true;
            }

            // Keep the defender responsive for the full attack presentation.
            // Resolution decides whether this later response was a valid counter
            // or simply too slow to matter.
            player.HasCounterMove = true;
            player.CounterMove = move;
            player.CounterMoveTime = totalTime;
            players[(int)slot] = player;
            return true;
        }

        public bool ResolveAttackSequence()
        {
            if (phase != DuelPhase.Attack)
            {
                return false;
            }

            TransitionTo(DuelPhase.Resolution);
            ResolveAttackOutcome();
            return true;
        }

        public bool AdvanceToEndPhase()
        {
            if (phase != DuelPhase.Resolution)
            {
                return false;
            }

            TransitionTo(DuelPhase.End);
            return true;
        }

        private bool AttacksAllowed()
        {
            return totalTime >= thirdBeatTime;
        }

        private void ResolveAttackOutcome()
        {
            var left = players[(int)PlayerSlot.Left];
            var right = players[(int)PlayerSlot.Right];

            if (left.PrematureCommitted || right.PrematureCommitted)
            {
                if (left.PrematureCommitted && right.PrematureCommitted)
                {
                    winner = null;
                    resolutionReason = DuelResolutionReason.DoublePremature;
                    return;
                }

                winner = left.PrematureCommitted ? PlayerSlot.Right : PlayerSlot.Left;
                resolutionReason = DuelResolutionReason.Premature;
                aggressor = winner;
                return;
            }

            if (!left.HasInitialMove && !right.HasInitialMove)
            {
                winner = null;
                resolutionReason = DuelResolutionReason.Timeout;
                return;
            }

            if (left.HasInitialMove && right.HasCounterMove && !right.HasInitialMove)
            {
                aggressor = PlayerSlot.Left;
                ResolveResponseOutcome(PlayerSlot.Left, PlayerSlot.Right);
                return;
            }

            if (right.HasInitialMove && left.HasCounterMove && !left.HasInitialMove)
            {
                aggressor = PlayerSlot.Right;
                ResolveResponseOutcome(PlayerSlot.Right, PlayerSlot.Left);
                return;
            }

            if (left.HasInitialMove && !right.HasInitialMove)
            {
                winner = PlayerSlot.Left;
                resolutionReason = DuelResolutionReason.NoResponse;
                aggressor = PlayerSlot.Left;
                return;
            }

            if (right.HasInitialMove && !left.HasInitialMove)
            {
                winner = PlayerSlot.Right;
                resolutionReason = DuelResolutionReason.NoResponse;
                aggressor = PlayerSlot.Right;
                return;
            }

            var delta = Math.Abs(left.InitialMoveTime - right.InitialMoveTime);
            if (delta > config.speedAdvantageThreshold)
            {
                aggressor = left.InitialMoveTime < right.InitialMoveTime ? PlayerSlot.Left : PlayerSlot.Right;

                var defenderSlot = DuelMoveRules.OpponentOf(aggressor.Value);
                var defender = players[(int)defenderSlot];
                if (defender.HasCounterMove)
                {
                    ResolveCounterOutcome(aggressor.Value, defenderSlot);
                    return;
                }

                winner = aggressor.Value;
                resolutionReason = DuelResolutionReason.SpeedAdvantage;
                return;
            }

            aggressor = null;
            var result = DuelMoveRules.Compare(left.InitialMove, right.InitialMove);
            if (result > 0)
            {
                winner = PlayerSlot.Left;
                resolutionReason = DuelResolutionReason.Standard;
            }
            else if (result < 0)
            {
                winner = PlayerSlot.Right;
                resolutionReason = DuelResolutionReason.Standard;
            }
            else
            {
                winner = null;
                resolutionReason = DuelResolutionReason.Clash;
            }
        }

        private void TransitionTo(DuelPhase nextPhase)
        {
            phase = nextPhase;
            phaseStartTime = totalTime;
        }

        private float GetPhaseTime()
        {
            return totalTime - phaseStartTime;
        }

        private DuelRoundSnapshot BuildSnapshot()
        {
            return new DuelRoundSnapshot
            {
                Phase = phase,
                TotalTime = totalTime,
                PhaseTime = GetPhaseTime(),
                VisibleBeatCount = GetVisibleBeatCount(),
                Aggressor = aggressor,
                Winner = winner,
                ResolutionReason = resolutionReason,
                LeftPlayer = players[(int)PlayerSlot.Left].ToSnapshot(),
                RightPlayer = players[(int)PlayerSlot.Right].ToSnapshot()
            };
        }

        private int GetVisibleBeatCount()
        {
            if (phase == DuelPhase.Idle)
            {
                return 0;
            }

            if (phase == DuelPhase.TakeYourStance)
            {
                if (totalTime >= thirdBeatTime)
                {
                    return 3;
                }

                return totalTime >= config.beatInterval ? 2 : 1;
            }

            return 3;
        }

        private float RandomThirdBeatDelay()
        {
            if (config.randomThirdBeatMaxDelay <= config.randomThirdBeatMinDelay)
            {
                return config.randomThirdBeatMinDelay;
            }

            var sample = (float)random.NextDouble();
            return config.randomThirdBeatMinDelay + (sample * (config.randomThirdBeatMaxDelay - config.randomThirdBeatMinDelay));
        }

        private void ResolveCounterOutcome(PlayerSlot attackerSlot, PlayerSlot defenderSlot)
        {
            var attacker = players[(int)attackerSlot];
            var defender = players[(int)defenderSlot];
            var counterResult = DuelMoveRules.Compare(defender.CounterMove, attacker.InitialMove);

            if (counterResult > 0)
            {
                winner = defenderSlot;
                resolutionReason = DuelResolutionReason.Counter;
            }
            else if (counterResult < 0)
            {
                winner = attackerSlot;
                resolutionReason = DuelResolutionReason.CounterFailed;
            }
            else
            {
                winner = null;
                resolutionReason = DuelResolutionReason.CounterClash;
            }
        }

        private void ResolveResponseOutcome(PlayerSlot attackerSlot, PlayerSlot defenderSlot)
        {
            var attacker = players[(int)attackerSlot];
            var defender = players[(int)defenderSlot];
            var responseDelta = defender.CounterMoveTime - attacker.InitialMoveTime;

            if (responseDelta <= config.counterWindowDuration)
            {
                ResolveCounterOutcome(attackerSlot, defenderSlot);
                return;
            }

            winner = attackerSlot;
            resolutionReason = DuelResolutionReason.SpeedAdvantage;
        }
    }
}
