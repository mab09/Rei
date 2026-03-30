using TMPro;
using UnityEngine;

namespace Rei.Runtime.Duel
{
    public class DuelHudPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text mainText;
        [SerializeField] private TMP_Text debugStateText;
        [SerializeField] private TMP_Text debugOutcomeText;
        [SerializeField] private bool showDebug = true;

        public void Present(DuelRoundSnapshot snapshot, DuelRoundConfig config)
        {
            if (mainText != null)
            {
                mainText.text = GetMainLabel(snapshot);
            }

            if (debugStateText != null)
            {
                debugStateText.text = showDebug ? $"Phase: {GetPhaseLabel(snapshot.Phase)}" : string.Empty;
            }

            if (debugOutcomeText != null)
            {
                debugOutcomeText.text = showDebug ? GetOutcomeLabel(snapshot) : string.Empty;
            }
        }

        public void SetDebugVisibility(bool visible)
        {
            showDebug = visible;

            if (debugStateText != null)
            {
                debugStateText.gameObject.SetActive(visible);
            }

            if (debugOutcomeText != null)
            {
                debugOutcomeText.gameObject.SetActive(visible);
            }
        }

        private static string GetMainLabel(DuelRoundSnapshot snapshot)
        {
            switch (snapshot.VisibleBeatCount)
            {
                case 1:
                    return "|";
                case 2:
                    return "| |";
                case 3:
                    return "| | |";
                default:
                    return string.Empty;
            }
        }

        private static string GetPhaseLabel(DuelPhase phase)
        {
            switch (phase)
            {
                case DuelPhase.TakeYourStance:
                    return "Take Your Stance";
                case DuelPhase.Attack:
                    return "Attack";
                case DuelPhase.Resolution:
                    return "Resolution";
                case DuelPhase.End:
                    return "End";
                default:
                    return "Idle";
            }
        }

        private static string GetOutcomeLabel(DuelRoundSnapshot snapshot)
        {
            if (snapshot.ResolutionReason == DuelResolutionReason.None)
            {
                return "Pending";
            }

            switch (snapshot.ResolutionReason)
            {
                case DuelResolutionReason.Premature:
                    return $"{GetWinnerName(snapshot.Winner)} Wins: Premature strike";
                case DuelResolutionReason.DoublePremature:
                    return "Draw: Double premature";
                case DuelResolutionReason.NoResponse:
                    return $"{GetWinnerName(snapshot.Winner)} Wins: No attack";
                case DuelResolutionReason.SpeedAdvantage:
                    return $"{GetWinnerName(snapshot.Winner)} Wins: Faster attack";
                case DuelResolutionReason.Counter:
                    return $"{GetWinnerName(snapshot.Winner)} Wins: Successful counter";
                case DuelResolutionReason.CounterFailed:
                    return $"{GetWinnerName(snapshot.Winner)} Wins: Counter punished";
                case DuelResolutionReason.CounterClash:
                    return "Draw: Counter clash";
                case DuelResolutionReason.Clash:
                    return "Draw: Clash";
                case DuelResolutionReason.Timeout:
                    return "No attack";
                case DuelResolutionReason.Standard:
                    return GetStandardOutcome(snapshot);
                default:
                    return snapshot.ResolutionReason.ToString();
            }
        }

        private static string GetStandardOutcome(DuelRoundSnapshot snapshot)
        {
            if (!snapshot.Winner.HasValue)
            {
                return "Draw";
            }

            var winnerPlayer = snapshot.GetPlayer(snapshot.Winner.Value);
            var loserSlot = snapshot.Winner == PlayerSlot.Left ? PlayerSlot.Right : PlayerSlot.Left;
            var loserPlayer = snapshot.GetPlayer(loserSlot);
            return $"{GetWinnerName(snapshot.Winner)} Wins: {MoveName(winnerPlayer.InitialMove)} beats {MoveName(loserPlayer.InitialMove)}";
        }

        private static string GetWinnerName(PlayerSlot? winner)
        {
            if (!winner.HasValue)
            {
                return "No one";
            }

            return winner == PlayerSlot.Left ? "Shogun" : "Rei";
        }

        private static string MoveName(DuelMove move)
        {
            switch (move)
            {
                case DuelMove.Taka:
                    return "Taka";
                case DuelMove.Hebi:
                    return "Hebi";
                case DuelMove.Tora:
                    return "Tora";
                default:
                    return "None";
            }
        }
    }
}
