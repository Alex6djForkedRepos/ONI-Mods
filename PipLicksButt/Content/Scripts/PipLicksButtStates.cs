namespace PipLicksButt.Content.Scripts
{
    internal class PipLicksButtStates : GameStateMachine<PipLicksButtStates, PipLicksButtStates.Instance, IStateMachineTarget, PipLicksButtStates.Def>
    {
        const float MAX_COOLDOWN = 0.6f * 8.0f;

        public class Def : BaseDef
        {
            public HashedString[] animSequence;
            public HashedString animName;
        }

        private State lick;
        private State behaviourComplete;

        new public class Instance : GameInstance
        {
            public Instance(Chore<Instance> chore, Def def) : base(chore, def)
            {
                chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, ModTags.wantsToLickButt);
            }
        }

        public override void InitializeStates(out BaseState defaultState)
        {
            defaultState = lick;

            lick
                .ToggleAnims(smi => smi.def.animName)
                .PlayAnims(smi => smi.def.animSequence, KAnim.PlayMode.Once)
                .ScheduleGoTo(MAX_COOLDOWN, behaviourComplete)
                .OnAnimQueueComplete(behaviourComplete)
                ;

            behaviourComplete
                .BehaviourComplete(ModTags.wantsToLickButt)
                ;
        }
    }
}
