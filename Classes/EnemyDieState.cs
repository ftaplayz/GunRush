using Godot;
namespace GunRush.Classes;

public class EnemyDieState: EnemyState
{
    public EnemyDieState(EnemyStateMachine esm) : base(esm)
    {
    }
    public override void Enter()
    {
        ESM.Owner.SetCondition("die", true);
        foreach (var collision in this.ESM.Owner.Collisions)
            collision.Disabled = true;
        
        this.Exit();
    }

    public override void Exit()
    {
        GD.Print("I'm dead!");
    }


    
}