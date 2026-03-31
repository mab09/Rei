#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using Rei.Runtime.Duel;
using UnityEngine;

public class DuelRoundEngineTests
{
    [Test]
    public void PrematureStrikeDoesNotEndRoundUntilOpponentAttacks()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Assert.That(engine.SubmitCommand(PlayerSlot.Left, DuelCommandKind.InitialAttack, DuelMove.Taka), Is.True);

        Advance(engine, 5f);

        var snapshot = engine.Snapshot;
        Assert.That(snapshot.Phase, Is.EqualTo(DuelPhase.TakeYourStance));
        Assert.That(snapshot.LeftPlayer.PrematureCommitted, Is.True);
        Assert.That(snapshot.Winner, Is.Null);

        Assert.That(engine.SubmitCommand(PlayerSlot.Right, DuelCommandKind.InitialAttack, DuelMove.Hebi), Is.True);
        Assert.That(engine.ResolveAttackSequence(), Is.True);
        Assert.That(engine.AdvanceToEndPhase(), Is.True);

        snapshot = engine.Snapshot;
        Assert.That(snapshot.Phase, Is.EqualTo(DuelPhase.End));
        Assert.That(snapshot.Winner, Is.EqualTo(PlayerSlot.Right));
        Assert.That(snapshot.ResolutionReason, Is.EqualTo(DuelResolutionReason.Premature));
    }

    [Test]
    public void NoAttackKeepsRoundInTakeYourStance()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Advance(engine, 10f);

        Assert.That(engine.Snapshot.Phase, Is.EqualTo(DuelPhase.TakeYourStance));
    }

    [Test]
    public void FirstValidAttackStartsAttackPhase()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Advance(engine, 2.1f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Left, DuelCommandKind.InitialAttack, DuelMove.Taka), Is.True);

        var snapshot = engine.Snapshot;
        Assert.That(snapshot.Phase, Is.EqualTo(DuelPhase.Attack));
        Assert.That(snapshot.LeftPlayer.HasInitialMove, Is.True);
    }

    [Test]
    public void ResolveAttackSequenceUsesMoveRules()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Advance(engine, 2.1f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Left, DuelCommandKind.InitialAttack, DuelMove.Taka), Is.True);
        Advance(engine, 0.1f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Right, DuelCommandKind.InitialAttack, DuelMove.Hebi), Is.True);

        Assert.That(engine.ResolveAttackSequence(), Is.True);
        Assert.That(engine.AdvanceToEndPhase(), Is.True);

        var snapshot = engine.Snapshot;
        Assert.That(snapshot.Phase, Is.EqualTo(DuelPhase.End));
        Assert.That(snapshot.Winner, Is.EqualTo(PlayerSlot.Left));
        Assert.That(snapshot.ResolutionReason, Is.EqualTo(DuelResolutionReason.Standard));
    }

    [Test]
    public void FasterAttackWinsBySpeedAdvantage()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Advance(engine, 2.1f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Left, DuelCommandKind.InitialAttack, DuelMove.Taka), Is.True);
        Advance(engine, 0.2f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Right, DuelCommandKind.InitialAttack, DuelMove.Hebi), Is.True);

        Assert.That(engine.ResolveAttackSequence(), Is.True);
        Assert.That(engine.AdvanceToEndPhase(), Is.True);

        var snapshot = engine.Snapshot;
        Assert.That(snapshot.Winner, Is.EqualTo(PlayerSlot.Left));
        Assert.That(snapshot.ResolutionReason, Is.EqualTo(DuelResolutionReason.SpeedAdvantage));
    }

    [Test]
    public void CounterCanBeatSpeedAdvantage()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Advance(engine, 2.1f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Left, DuelCommandKind.InitialAttack, DuelMove.Taka), Is.True);
        Advance(engine, 0.12f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Right, DuelCommandKind.InitialAttack, DuelMove.Tora), Is.True);

        Assert.That(engine.ResolveAttackSequence(), Is.True);
        Assert.That(engine.AdvanceToEndPhase(), Is.True);

        var snapshot = engine.Snapshot;
        Assert.That(snapshot.Winner, Is.EqualTo(PlayerSlot.Right));
        Assert.That(snapshot.ResolutionReason, Is.EqualTo(DuelResolutionReason.Counter));
    }

    [Test]
    public void LateResponseDuringAttackStillRegistersAndLosesBySpeedAdvantage()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Advance(engine, 2.1f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Left, DuelCommandKind.InitialAttack, DuelMove.Taka), Is.True);
        Advance(engine, 0.35f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Right, DuelCommandKind.InitialAttack, DuelMove.Hebi), Is.True);

        Assert.That(engine.ResolveAttackSequence(), Is.True);
        Assert.That(engine.AdvanceToEndPhase(), Is.True);

        var snapshot = engine.Snapshot;
        Assert.That(snapshot.Winner, Is.EqualTo(PlayerSlot.Left));
        Assert.That(snapshot.ResolutionReason, Is.EqualTo(DuelResolutionReason.SpeedAdvantage));
    }

    [Test]
    public void CounterIsNotMisreadAsNoResponse()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Advance(engine, 2.1f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Right, DuelCommandKind.InitialAttack, DuelMove.Hebi), Is.True);
        Advance(engine, 0.12f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Left, DuelCommandKind.InitialAttack, DuelMove.Taka), Is.True);

        Assert.That(engine.ResolveAttackSequence(), Is.True);
        Assert.That(engine.AdvanceToEndPhase(), Is.True);

        var snapshot = engine.Snapshot;
        Assert.That(snapshot.ResolutionReason, Is.Not.EqualTo(DuelResolutionReason.NoResponse));
    }

    [Test]
    public void SingleAttackerWinsByNoResponseWhenResolved()
    {
        var engine = CreateEngine();
        engine.StartRound();

        Advance(engine, 2.1f);
        Assert.That(engine.SubmitCommand(PlayerSlot.Right, DuelCommandKind.InitialAttack, DuelMove.Hebi), Is.True);

        Assert.That(engine.ResolveAttackSequence(), Is.True);
        Assert.That(engine.AdvanceToEndPhase(), Is.True);

        var snapshot = engine.Snapshot;
        Assert.That(snapshot.Winner, Is.EqualTo(PlayerSlot.Right));
        Assert.That(snapshot.ResolutionReason, Is.EqualTo(DuelResolutionReason.NoResponse));
    }

    [Test]
    public void BasicAiPlanStaysWithinConfiguredRanges()
    {
        var config = CreateConfig();
        var plan = BasicAiPlanner.CreatePlan(config, new System.Random(123));

        Assert.That(plan.InitialMove, Is.Not.EqualTo(DuelMove.None));
        Assert.That(plan.InitialDelay, Is.InRange(config.aiInitialMinDelay, config.aiInitialMaxDelay));
    }

    private static DuelRoundEngine CreateEngine()
    {
        return new DuelRoundEngine(CreateConfig());
    }

    private static DuelRoundConfig CreateConfig()
    {
        var config = ScriptableObject.CreateInstance<DuelRoundConfig>();
        config.beatInterval = 1f;
        config.randomThirdBeatMinDelay = 1f;
        config.randomThirdBeatMaxDelay = 1f;
        config.speedAdvantageThreshold = 0.08f;
        config.counterWindowDuration = 0.2f;
        config.aiInitialMinDelay = 0.05f;
        config.aiInitialMaxDelay = 0.5f;
        config.aiCounterChance = 0.5f;
        config.aiCounterMinDelay = 0.09f;
        config.aiCounterMaxDelay = 0.18f;
        return config;
    }

    private static void Advance(DuelRoundEngine engine, float duration)
    {
        var elapsed = 0f;
        while (elapsed < duration)
        {
            var step = Mathf.Min(0.02f, duration - elapsed);
            engine.Tick(step);
            elapsed += step;
        }
    }
}
#endif
