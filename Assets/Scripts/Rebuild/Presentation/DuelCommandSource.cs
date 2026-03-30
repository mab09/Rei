using System.Collections.Generic;
using UnityEngine;

namespace Rei.Runtime.Duel
{
    public abstract class DuelCommandSource : MonoBehaviour
    {
        private readonly Queue<BufferedDuelCommand> bufferedCommands = new Queue<BufferedDuelCommand>();

        protected DuelRoundConfig Config { get; private set; }
        protected DuelRoundSnapshot Snapshot { get; private set; }
        protected PlayerSlot Slot { get; private set; }

        public virtual void Initialize(PlayerSlot slot, DuelRoundConfig config)
        {
            Slot = slot;
            Config = config;
        }

        public virtual void OnRoundStarted(DuelRoundSnapshot snapshot)
        {
            Snapshot = snapshot;
            bufferedCommands.Clear();
        }

        public virtual void OnSnapshotUpdated(DuelRoundSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public virtual void Tick(float deltaTime, DuelRoundSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public bool TryConsumeCommand(out BufferedDuelCommand command)
        {
            if (bufferedCommands.Count > 0)
            {
                command = bufferedCommands.Dequeue();
                return true;
            }

            command = default;
            return false;
        }

        protected void QueueCommand(DuelCommandKind kind, DuelMove move)
        {
            bufferedCommands.Enqueue(new BufferedDuelCommand(Slot, kind, move));
        }
    }
}
