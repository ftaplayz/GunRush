using GunRush.Interfaces;

namespace GunRush.Classes;

public abstract class EnemyState: IState
{
    protected EnemyStateMachine ESM;

    protected EnemyState(EnemyStateMachine esm)
    {
        this.ESM = esm;
    }

    public abstract void Enter();
    public abstract void Exit();
}