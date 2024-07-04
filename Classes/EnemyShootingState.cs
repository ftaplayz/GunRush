using Godot;

namespace GunRush.Classes;

public partial class EnemyShootingState: EnemyState
{
    public EnemyShootingState(EnemyStateMachine esm) : base(esm)
    {
    }
    public override void Enter()
    {
        ESM.Owner.SetCondition("shoot", true);
        //ESM.Owner.LookAt(ESM.Owner.TargetPlayer.GlobalPosition)//
        ESM.Owner.WeaponController.Fire(true);
    }

    public override void Exit()
    {
        ESM.Owner.WeaponController.Fire(false);
        ESM.Owner.SetCondition("shoot", false);
    }

    
}