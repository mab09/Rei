using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Rei.Runtime.Duel
{
    public class DuelGameController : MonoBehaviour
    {
        private static AiSelection sessionAiSelection = AiSelection.Off;

        private enum AiSelection
        {
            Off = 0,
            Rei = 1,
            Shogun = 2
        }

        [Header("Config")]
        [SerializeField] private DuelRoundConfig roundConfig;

        [Header("Command Sources")]
        [SerializeField] private MonoBehaviour leftCommandSource;
        [SerializeField] private MonoBehaviour rightCommandSource;

        [Header("Presentation")]
        [SerializeField] private DuelCharacterPresenter leftPresenter;
        [SerializeField] private DuelCharacterPresenter rightPresenter;
        [SerializeField] private DuelHudPresenter hudPresenter;

        [Header("Controls")]
        [SerializeField] private Button readyButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button aiModeButton;
        [SerializeField] private Text aiModeLabel;
        [SerializeField] private bool showDebugHud = true;

        private DuelCommandSource leftSource;
        private DuelCommandSource rightSource;
        private DuelRoundEngine engine;
        private DuelRoundSnapshot lastSnapshot;
        private bool running;
        private AiSelection aiSelection;
        private bool endSequenceActive;
        private bool winnerVictoryTriggered;

        private void Awake()
        {
            ResolveCommandSources();
            aiSelection = sessionAiSelection;

            if (readyButton != null)
            {
                readyButton.onClick.AddListener(StartRound);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartScene);
            }

            if (aiModeButton != null)
            {
                aiModeButton.onClick.AddListener(CycleAiMode);
            }

            if (leftSource != null)
            {
                leftSource.Initialize(PlayerSlot.Left, roundConfig);
            }

            if (rightSource != null)
            {
                rightSource.Initialize(PlayerSlot.Right, roundConfig);
            }

            leftPresenter?.ApplyRoundConfig(roundConfig);
            rightPresenter?.ApplyRoundConfig(roundConfig);
            ApplyAiSelection();
        }

        private void Start()
        {
            leftPresenter?.ResetForRound();
            rightPresenter?.ResetForRound();
            UpdateButtonsForIdle();

            hudPresenter?.SetDebugVisibility(showDebugHud);
            Present(default);
        }

        private void Update()
        {
            if (!running || engine == null)
            {
                return;
            }

            var snapshotBeforeTick = engine.Snapshot;
            TickSources(snapshotBeforeTick);
            ConsumeCommands();

            engine.Tick(Time.deltaTime);
            var currentSnapshot = engine.Snapshot;

            leftSource?.OnSnapshotUpdated(currentSnapshot);
            rightSource?.OnSnapshotUpdated(currentSnapshot);

            Present(currentSnapshot);
            lastSnapshot = currentSnapshot;

            if (currentSnapshot.Phase == DuelPhase.Attack && AttackPresentationComplete(currentSnapshot))
            {
                engine.ResolveAttackSequence();
                currentSnapshot = engine.Snapshot;
                Present(currentSnapshot);
                lastSnapshot = currentSnapshot;
            }

            if (currentSnapshot.Phase == DuelPhase.Resolution)
            {
                TickResolutionPhase(currentSnapshot);
                currentSnapshot = engine.Snapshot;
                Present(currentSnapshot);
                lastSnapshot = currentSnapshot;
            }

            if (currentSnapshot.Phase == DuelPhase.End)
            {
                UpdateButtonsForResolved();
            }
        }

        public void StartRound()
        {
            ResolveCommandSources();

            if (roundConfig == null)
            {
                Debug.LogError("DuelGameController requires a DuelRoundConfig.");
                return;
            }

            engine = new DuelRoundEngine(roundConfig);
            engine.StartRound();
            running = true;
            endSequenceActive = false;
            winnerVictoryTriggered = false;

            lastSnapshot = engine.Snapshot;

            leftSource?.OnRoundStarted(lastSnapshot);
            rightSource?.OnRoundStarted(lastSnapshot);

            leftPresenter?.ApplyRoundConfig(roundConfig);
            rightPresenter?.ApplyRoundConfig(roundConfig);
            leftPresenter?.ResetForRound();
            rightPresenter?.ResetForRound();
            UpdateButtonsForRunning();

            Present(lastSnapshot);
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void CycleAiMode()
        {
            aiSelection = (AiSelection)(((int)aiSelection + 1) % 3);
            sessionAiSelection = aiSelection;
            ApplyAiSelection();
        }

        private void TickSources(DuelRoundSnapshot snapshot)
        {
            leftSource?.Tick(Time.deltaTime, snapshot);
            rightSource?.Tick(Time.deltaTime, snapshot);
        }

        private void ConsumeCommands()
        {
            ConsumeSource(leftSource);
            ConsumeSource(rightSource);
        }

        private void ConsumeSource(DuelCommandSource source)
        {
            if (source == null)
            {
                return;
            }

            while (source.TryConsumeCommand(out var command))
            {
                engine.SubmitCommand(command.Slot, command.Kind, command.Move);
            }
        }

        private void ResolveCommandSources()
        {
            leftSource = ResolveCommandSource(leftCommandSource, leftPresenter, "Left");
            rightSource = ResolveCommandSource(rightCommandSource, rightPresenter, "Right");
        }

        private DuelCommandSource ResolveCommandSource(MonoBehaviour reference, DuelCharacterPresenter presenter, string label)
        {
            DuelCommandSource source = null;

            if (reference != null)
            {
                source = reference as DuelCommandSource;
                if (source == null)
                {
                    source = reference.GetComponent<DuelCommandSource>();
                }
            }

            if (source == null && presenter != null)
            {
                source = presenter.GetComponent<DuelCommandSource>();
            }

            if (source == null)
            {
                Debug.LogError($"DuelGameController could not resolve the {label} command source. Assign a DuelCommandSource-derived component in the inspector.");
            }

            return source;
        }

        private void Present(DuelRoundSnapshot snapshot)
        {
            leftPresenter?.Present(snapshot);
            rightPresenter?.Present(snapshot);
            hudPresenter?.Present(snapshot, roundConfig);
        }

        private void TickResolutionPhase(DuelRoundSnapshot snapshot)
        {
            if (!endSequenceActive)
            {
                leftPresenter?.BeginEndSequence(snapshot);
                rightPresenter?.BeginEndSequence(snapshot);
                endSequenceActive = true;
                winnerVictoryTriggered = false;
                return;
            }

            if (!snapshot.Winner.HasValue)
            {
                engine.AdvanceToEndPhase();
                return;
            }

            if (!winnerVictoryTriggered)
            {
                var losingPresenter = GetLosingPresenter(snapshot);
                if (losingPresenter != null && !losingPresenter.IsDeathAnimationFinished())
                {
                    return;
                }

                leftPresenter?.TriggerVictoryIfWinner(snapshot);
                rightPresenter?.TriggerVictoryIfWinner(snapshot);
                winnerVictoryTriggered = true;
                return;
            }

            engine.AdvanceToEndPhase();
        }

        private void UpdateButtonsForIdle()
        {
            if (readyButton != null)
            {
                readyButton.gameObject.SetActive(true);
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
        }

        private void UpdateButtonsForRunning()
        {
            if (readyButton != null)
            {
                readyButton.gameObject.SetActive(false);
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
        }

        private void UpdateButtonsForResolved()
        {
            if (readyButton != null)
            {
                readyButton.gameObject.SetActive(false);
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
        }

        private void ApplyAiSelection()
        {
            SetAiMode(leftSource as TouchButtonDuelCommandSource, aiSelection == AiSelection.Shogun);
            SetAiMode(rightSource as TouchButtonDuelCommandSource, aiSelection == AiSelection.Rei);

            if (aiModeLabel != null)
            {
                aiModeLabel.text = GetAiLabel();
            }
        }

        private static void SetAiMode(TouchButtonDuelCommandSource source, bool useAi)
        {
            if (source == null)
            {
                return;
            }

            source.SetControlMode(useAi
                ? TouchButtonDuelCommandSource.ControlMode.BasicAi
                : TouchButtonDuelCommandSource.ControlMode.HumanButtons);
        }

        private string GetAiLabel()
        {
            switch (aiSelection)
            {
                case AiSelection.Rei:
                    return "AI: Rei";
                case AiSelection.Shogun:
                    return "AI: Shogun";
                default:
                    return "AI: Off";
            }
        }

        private bool AttackPresentationComplete(DuelRoundSnapshot snapshot)
        {
            return PresenterComplete(leftPresenter, snapshot) && PresenterComplete(rightPresenter, snapshot);
        }

        private DuelCharacterPresenter GetLosingPresenter(DuelRoundSnapshot snapshot)
        {
            if (!snapshot.Winner.HasValue)
            {
                return null;
            }

            return snapshot.Winner == PlayerSlot.Left ? rightPresenter : leftPresenter;
        }

        private static bool PresenterComplete(DuelCharacterPresenter presenter, DuelRoundSnapshot snapshot)
        {
            return presenter == null || presenter.HasCompletedAttackPresentation(snapshot);
        }
    }
}
