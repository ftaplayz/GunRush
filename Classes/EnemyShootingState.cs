using Godot;

namespace GunRush.Classes;

public class EnemyShootingState: EnemyState
{
    public EnemyShootingState(EnemyStateMachine esm) : base(esm)
    {
    }
    public override void Enter()
    {
        ESM.Owner.SetCondition("shoot", true);
    }

    public override void Exit()
    {
        ESM.Owner.SetCondition("shoot", false);
    }
}