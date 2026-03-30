using UnityEngine;
using UnityEngine.UI;

namespace Rei.Runtime.Duel
{
    public class TouchButtonDuelCommandSource : DuelCommandSource
    {
        public enum ControlMode
        {
            HumanButtons = 0,
            BasicAi = 1
        }

        [SerializeField] private ControlMode controlMode = ControlMode.HumanButtons;
        [SerializeField] private Button takaButton;
        [SerializeField] private Button hebiButton;
        [SerializeField] private Button toraButton;

        private BasicAiPlan currentPlan;
        private bool queuedInitialMove;
        private bool queuedCounterMove;
        private float allowedAttackElapsed;
        private float attackPhaseElapsed;

        public ControlMode Mode => controlMode;

        private void Awake()
        {
            if (takaButton != null)
            {
                takaButton.onClick.AddListener(() => QueueFromButton(DuelMove.Taka));
            }

            if (hebiButton != null)
            {
                hebiButton.onClick.AddListener(() => QueueFromButton(DuelMove.Hebi));
            }

            if (toraButton != null)
            {
                toraButton.onClick.AddListener(() => QueueFromButton(DuelMove.Tora));
            }

            ApplyButtonVisibility();
        }

        private void OnValidate()
        {
            ApplyButtonVisibility();
        }

        public override void OnRoundStarted(DuelRoundSnapshot snapshot)
        {
            base.OnRoundStarted(snapshot);

            ResetPlan();

            ApplyButtonVisibility();
        }

        public override void Tick(float deltaTime, DuelRoundSnapshot snapshot)
        {
            base.Tick(deltaTime, snapshot);

            if (controlMode != ControlMode.BasicAi)
            {
                return;
            }

            if (snapshot.Phase == DuelPhase.TakeYourStance && snapshot.VisibleBeatCount >= 3)
            {
                allowedAttackElapsed += deltaTime;
            }
            else
            {
                allowedAttackElapsed = 0f;
            }

            if (!queuedInitialMove &&
                snapshot.Phase == DuelPhase.TakeYourStance &&
                snapshot.VisibleBeatCount >= 3 &&
                allowedAttackElapsed >= currentPlan.InitialDelay)
            {
                QueueCommand(DuelCommandKind.InitialAttack, currentPlan.InitialMove);
                queuedInitialMove = true;
            }

            if (snapshot.Phase == DuelPhase.Attack)
            {
                attackPhaseElapsed += deltaTime;
            }
            else
            {
                attackPhaseElapsed = 0f;
            }

            if (queuedInitialMove || queuedCounterMove || !currentPlan.AttemptCounter || snapshot.Phase != DuelPhase.Attack)
            {
                return;
            }

            if (attackPhaseElapsed >= currentPlan.CounterDelay)
            {
                QueueCommand(DuelCommandKind.InitialAttack, currentPlan.CounterMove);
                queuedCounterMove = true;
            }
        }

        public void QueueTaka()
        {
            QueueFromButton(DuelMove.Taka);
        }

        public void QueueHebi()
        {
            QueueFromButton(DuelMove.Hebi);
        }

        public void QueueTora()
        {
            QueueFromButton(DuelMove.Tora);
        }

        public void SetControlMode(ControlMode mode)
        {
            controlMode = mode;
            ResetPlan();
            ApplyButtonVisibility();
        }

        private void QueueFromButton(DuelMove move)
        {
            if (controlMode != ControlMode.HumanButtons)
            {
                return;
            }

            var kind = GetExpectedCommandKind();
            if (!kind.HasValue)
            {
                return;
            }

            QueueCommand(kind.Value, move);
        }

        private DuelCommandKind? GetExpectedCommandKind()
        {
            switch (Snapshot.Phase)
            {
                case DuelPhase.TakeYourStance:
                case DuelPhase.Attack:
                    return DuelCommandKind.InitialAttack;

                default:
                    return null;
            }
        }

        private void ApplyButtonVisibility()
        {
            var showButtons = controlMode == ControlMode.HumanButtons;

            if (takaButton != null)
            {
                takaButton.gameObject.SetActive(showButtons);
            }

            if (hebiButton != null)
            {
                hebiButton.gameObject.SetActive(showButtons);
            }

            if (toraButton != null)
            {
                toraButton.gameObject.SetActive(showButtons);
            }
        }

        private void ResetPlan()
        {
            queuedInitialMove = false;
            queuedCounterMove = false;
            allowedAttackElapsed = 0f;
            attackPhaseElapsed = 0f;

            if (controlMode == ControlMode.BasicAi && Config != null)
            {
                currentPlan = BasicAiPlanner.CreatePlan(Config, new System.Random(System.Environment.TickCount ^ (int)Slot));
            }
        }
    }
}
