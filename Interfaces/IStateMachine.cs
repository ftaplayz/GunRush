namespace GunRush.Interfaces;

public interface IStateMachine
{
    public void TransitionTo(IState state);
}