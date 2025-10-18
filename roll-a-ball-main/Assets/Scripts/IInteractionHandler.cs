public interface IInteractionHandler
{
    void EnterMode(BallBehaviour ball);
    void ExitMode(BallBehaviour ball);
    void Update(BallBehaviour ball);
 
}