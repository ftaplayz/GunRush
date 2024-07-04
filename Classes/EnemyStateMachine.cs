using GunRush.Interfaces;

namespace GunRush.Classes;

public class EnemyStateMachine: IStateMachine
{
	public EnemyIdleState Idle;
	public EnemyShootingState Shooting;
	public EnemyDieState Die;
	public Enemy Owner;
	public EnemyState CurrentState => this._currentState;
	private EnemyState _currentState;
	private EnemyState _previousState;
	

	public EnemyStateMachine(Enemy owner)
	{
		this.Owner = owner;
		this.Idle = new EnemyIdleState(this);
		this.Shooting = new EnemyShootingState(this);
		this.Die = new EnemyDieState(this);
	}
	
	
	
	public void TransitionTo(IState state)
	{
		this._TransitionTo((EnemyState) state);
	}

	private void _TransitionTo(EnemyState state)
	{
		this._previousState = this._currentState;
		this._currentState?.Exit();
		this._currentState = state;
		this._currentState.Enter();
	}
}
