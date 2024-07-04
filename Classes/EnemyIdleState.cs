using Godot;
namespace GunRush.Classes;

public class EnemyIdleState: EnemyState
{
    public EnemyIdleState(EnemyStateMachine esm) : base(esm)
    {
    }
    public override void Enter()
    {
        ESM.Owner.SetCondition("idle", true);
    }

    public override void Exit()
    {
        ESM.Owner.SetCondition("idle", false);
    }
}