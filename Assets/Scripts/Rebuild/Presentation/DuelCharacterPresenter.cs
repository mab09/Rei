using UnityEngine;

namespace Rei.Runtime.Duel
{
    public class DuelCharacterPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerSlot slot;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform opponent;
        [SerializeField] private string readyTrigger = "TriggerReady";
        [SerializeField] private string runTrigger = "TriggerRun";
        [SerializeField] private string falseTrigger = "TriggerFalse";
        [SerializeField] private string winTrigger = "TriggerWin";
        [SerializeField] private string drawTrigger = "TriggerDraw";
        [SerializeField] private string falseDeathTrigger = "TriggerFalseDeath";
        [SerializeField] private string readyDeathTrigger = "TriggerReadyDeath";
        [SerializeField] private string upDeathTrigger = "TriggerUpDeath";
        [SerializeField] private string downDeathTrigger = "TriggerDownDeath";
        [SerializeField] private string frontDeathTrigger = "TriggerFrontDeath";
        [SerializeField] private string upTrigger = "TriggerUp";
        [SerializeField] private string downTrigger = "TriggerDown";
        [SerializeField] private string frontTrigger = "TriggerFront";
        [SerializeField] private GameObject katanaOn;
        [SerializeField] private GameObject katanaOff;

        private DuelRoundSnapshot lastSnapshot;
        private bool runTriggered;
        private bool attackTriggered;
        private bool shouldAdvance;
        private DuelMove queuedMove;
        private Vector3 startPosition;
        private bool pendingResolvedResult;
        private DuelRoundSnapshot pendingResolvedSnapshot;
        private bool endSequenceStarted;
        private bool victoryAnimationTriggered;
        private bool deathAnimationFinished;
        private float configuredMoveSpeed = 7f;
        private float configuredStrikeDistance = 0f;
        private float configuredPassThroughDistance = 5f;
        private const float PositionTolerance = 0.05f;

        private void Awake()
        {
            startPosition = transform.position;
            ApplyIdleVisuals();
        }

        private void Update()
        {
            if (!shouldAdvance || queuedMove == DuelMove.None || opponent == null)
            {
                if (pendingResolvedResult && !endSequenceStarted && !shouldAdvance)
                {
                    BeginEndSequence(pendingResolvedSnapshot);
                }

                return;
            }

            var currentX = transform.position.x;
            var liveTargetX = GetLiveTravelTargetX();
            var nextX = Mathf.MoveTowards(currentX, liveTargetX, configuredMoveSpeed * Time.deltaTime);
            var deltaX = nextX - currentX;

            if (!Mathf.Approximately(deltaX, 0f))
            {
                transform.position += new Vector3(deltaX, 0f, 0f);
            }

            if (!attackTriggered && HasReachedStrikePoint())
            {
                TriggerMove(queuedMove);
            }

            if (HasReachedTravelTarget())
            {
                shouldAdvance = false;

                if (pendingResolvedResult && !endSequenceStarted)
                {
                    BeginEndSequence(pendingResolvedSnapshot);
                }
            }
        }

        public void ApplyRoundConfig(DuelRoundConfig config)
        {
            if (config == null)
            {
                return;
            }

            configuredMoveSpeed = config.moveSpeed;
            configuredStrikeDistance = config.strikeDistance;
            configuredPassThroughDistance = config.passThroughDistance;
        }

        public void ResetForRound()
        {
            lastSnapshot = default;
            runTriggered = false;
            attackTriggered = false;
            shouldAdvance = false;
            queuedMove = DuelMove.None;
            pendingResolvedResult = false;
            pendingResolvedSnapshot = default;
            endSequenceStarted = false;
            victoryAnimationTriggered = false;
            deathAnimationFinished = false;

            transform.position = startPosition;

            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
            }

            ApplyIdleVisuals();
        }

        public void Present(DuelRoundSnapshot snapshot)
        {
            if (animator == null)
            {
                lastSnapshot = snapshot;
                return;
            }

            if (snapshot.Phase == DuelPhase.TakeYourStance && lastSnapshot.Phase != DuelPhase.TakeYourStance)
            {
                animator.SetTrigger(readyTrigger);
            }

            var previousPlayer = lastSnapshot.GetPlayer(slot);
            var currentPlayer = snapshot.GetPlayer(slot);

            if (!previousPlayer.HasInitialMove && currentPlayer.HasInitialMove)
            {
                BeginAdvance(currentPlayer.InitialMove);
            }

            if (!previousPlayer.PrematureCommitted && currentPlayer.PrematureCommitted)
            {
                animator.SetTrigger(falseTrigger);
            }

            if (!previousPlayer.HasCounterMove && currentPlayer.HasCounterMove)
            {
                BeginAdvance(currentPlayer.CounterMove);
            }

            if (snapshot.Phase == DuelPhase.End && lastSnapshot.Phase != DuelPhase.End)
            {
                pendingResolvedSnapshot = snapshot;
                pendingResolvedResult = true;

                if (!shouldAdvance)
                {
                    BeginEndSequence(snapshot);
                }
            }

            if (katanaOn != null && katanaOff != null)
            {
                var bladeDrawn = currentPlayer.HasInitialMove || currentPlayer.HasCounterMove || currentPlayer.PrematureCommitted;
                katanaOn.SetActive(bladeDrawn);
                katanaOff.SetActive(!bladeDrawn);
            }

            lastSnapshot = snapshot;
        }

        public bool HasCompletedAttackPresentation(DuelRoundSnapshot snapshot)
        {
            var player = snapshot.GetPlayer(slot);

            if (player.PrematureCommitted)
            {
                return true;
            }

            if (!player.HasInitialMove && !player.HasCounterMove)
            {
                return true;
            }

            return !shouldAdvance;
        }

        public void BeginEndSequence(DuelRoundSnapshot snapshot)
        {
            if (endSequenceStarted)
            {
                return;
            }

            var player = snapshot.GetPlayer(slot);
            endSequenceStarted = true;
            pendingResolvedResult = false;
            shouldAdvance = false;

            if (player.PrematureCommitted && snapshot.Winner != slot)
            {
                animator.SetTrigger(falseDeathTrigger);
                return;
            }

            if (!snapshot.Winner.HasValue)
            {
                var currentPlayer = snapshot.GetPlayer(slot);
                if (currentPlayer.HasInitialMove)
                {
                    queuedMove = currentPlayer.InitialMove;
                    TriggerRun();

                    if (!attackTriggered)
                    {
                        TriggerMove(currentPlayer.InitialMove);
                    }
                }

                animator.SetTrigger(drawTrigger);
                victoryAnimationTriggered = true;
            }
            else
            {
                if (snapshot.Winner == slot)
                {
                    return;
                }

                TriggerDeath(GetLosingMove(player));
            }
        }

        public void TriggerVictoryIfWinner(DuelRoundSnapshot snapshot)
        {
            if (victoryAnimationTriggered || snapshot.Winner != slot)
            {
                return;
            }

            animator.SetTrigger(winTrigger);
            victoryAnimationTriggered = true;
        }

        public bool IsDeathAnimationFinished()
        {
            return deathAnimationFinished;
        }

        public void OnDeathAnimationFinished()
        {
            deathAnimationFinished = true;
        }

        private void BeginAdvance(DuelMove move)
        {
            queuedMove = move;
            shouldAdvance = true;
            TriggerRun();
        }

        private void TriggerRun()
        {
            if (runTriggered)
            {
                return;
            }

            animator.SetTrigger(runTrigger);
            runTriggered = true;
        }

        private void TriggerMove(DuelMove move)
        {
            if (attackTriggered)
            {
                return;
            }

            switch (move)
            {
                case DuelMove.Taka:
                    animator.SetTrigger(upTrigger);
                    attackTriggered = true;
                    break;
                case DuelMove.Hebi:
                    animator.SetTrigger(frontTrigger);
                    attackTriggered = true;
                    break;
                case DuelMove.Tora:
                    animator.SetTrigger(downTrigger);
                    attackTriggered = true;
                    break;
            }
        }

        private void TriggerDeath(DuelMove losingMove)
        {
            switch (losingMove)
            {
                case DuelMove.Taka:
                    animator.SetTrigger(upDeathTrigger);
                    break;
                case DuelMove.Hebi:
                    animator.SetTrigger(frontDeathTrigger);
                    break;
                case DuelMove.Tora:
                    animator.SetTrigger(downDeathTrigger);
                    break;
                default:
                    animator.SetTrigger(readyDeathTrigger);
                    break;
            }
        }

        private DuelMove GetLosingMove(DuelPlayerSnapshot player)
        {
            if (player.HasCounterMove)
            {
                return player.CounterMove;
            }

            if (player.HasInitialMove)
            {
                return player.InitialMove;
            }

            return DuelMove.None;
        }

        private float GetLiveTravelTargetX()
        {
            if (opponent == null)
            {
                return transform.position.x;
            }

            return slot == PlayerSlot.Left
                ? opponent.position.x + configuredPassThroughDistance
                : opponent.position.x - configuredPassThroughDistance;
        }

        private bool HasReachedStrikePoint()
        {
            if (opponent == null)
            {
                return false;
            }

            var threshold = Mathf.Max(0f, configuredStrikeDistance);

            if (slot == PlayerSlot.Left)
            {
                return transform.position.x >= opponent.position.x - threshold;
            }

            return transform.position.x <= opponent.position.x + threshold;
        }

        private bool HasReachedTravelTarget()
        {
            if (opponent == null)
            {
                return true;
            }

            if (slot == PlayerSlot.Left)
            {
                return transform.position.x >= opponent.position.x + configuredPassThroughDistance - PositionTolerance;
            }

            return transform.position.x <= opponent.position.x - configuredPassThroughDistance + PositionTolerance;
        }

        private void ApplyIdleVisuals()
        {
            if (katanaOn != null)
            {
                katanaOn.SetActive(false);
            }

            if (katanaOff != null)
            {
                katanaOff.SetActive(true);
            }
        }

    }
}
