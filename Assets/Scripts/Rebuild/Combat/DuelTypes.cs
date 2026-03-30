using System;

namespace Rei.Runtime.Duel
{
    public enum PlayerSlot
    {
        Left = 0,
        Right = 1
    }

    public enum DuelMove
    {
        None = 0,
        Taka = 1,
        Hebi = 2,
        Tora = 3
    }

    public enum DuelPhase
    {
        Idle = 0,
        TakeYourStance = 1,
        Attack = 2,
        Resolution = 3,
        End = 4
    }

    public enum DuelCommandKind
    {
        InitialAttack = 0
    }

    public enum DuelResolutionReason
    {
        None = 0,
        Standard = 1,
        SpeedAdvantage = 2,
        Counter = 3,
        CounterFailed = 4,
        CounterClash = 5,
        Premature = 6,
        DoublePremature = 7,
        Timeout = 8,
        NoResponse = 9,
        Clash = 10
    }

    [Serializable]
    public struct DuelPlayerSnapshot
    {
        public bool PrematureCommitted;
        public bool HasInitialMove;
        public DuelMove InitialMove;
        public float InitialMoveTime;
        public bool HasCounterMove;
        public DuelMove CounterMove;
        public float CounterMoveTime;
    }

    [Serializable]
    public struct DuelRoundSnapshot
    {
        public DuelPhase Phase;
        public float TotalTime;
        public float PhaseTime;
        public int VisibleBeatCount;
        public PlayerSlot? Aggressor;
        public PlayerSlot? Winner;
        public DuelResolutionReason ResolutionReason;
        public DuelPlayerSnapshot LeftPlayer;
        public DuelPlayerSnapshot RightPlayer;

        public DuelPlayerSnapshot GetPlayer(PlayerSlot slot)
        {
            return slot == PlayerSlot.Left ? LeftPlayer : RightPlayer;
        }
    }

    [Serializable]
    public struct BufferedDuelCommand
    {
        public PlayerSlot Slot;
        public DuelCommandKind Kind;
        public DuelMove Move;

        public BufferedDuelCommand(PlayerSlot slot, DuelCommandKind kind, DuelMove move)
        {
            Slot = slot;
            Kind = kind;
            Move = move;
        }
    }

    public static class DuelMoveRules
    {
        public static int Compare(DuelMove first, DuelMove second)
        {
            if (first == second)
            {
                return 0;
            }

            if (first == DuelMove.Taka && second == DuelMove.Hebi)
            {
                return 1;
            }

            if (first == DuelMove.Hebi && second == DuelMove.Tora)
            {
                return 1;
            }

            if (first == DuelMove.Tora && second == DuelMove.Taka)
            {
                return 1;
            }

            return -1;
        }

        public static PlayerSlot OpponentOf(PlayerSlot slot)
        {
            return slot == PlayerSlot.Left ? PlayerSlot.Right : PlayerSlot.Left;
        }
    }
}
