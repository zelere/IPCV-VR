using UnityEngine;

public class KeyboardInteractionHandler : IInteractionHandler
{
    public void EnterMode(BallBehaviour ball)
    {
        if (ball.rb != null)
        {
            ball.rb.velocity = Vector3.zero;
            ball.rb.angularVelocity = Vector3.zero;
        }
    }

    public void ExitMode(BallBehaviour ball) { }

    public void Update(BallBehaviour ball)
    {
        if (ball.rb == null) return;

        Vector3 inputVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) inputVector.z += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) inputVector.z -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) inputVector.x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) inputVector.x += 1f;

        if (inputVector.magnitude > 1f) inputVector.Normalize();

        Vector3 currentVelocity = ball.rb.velocity;
        float currentSpeed = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;

        if (inputVector != Vector3.zero && currentSpeed < ball.keyboardMaxSpeed)
        {
            Vector3 force = inputVector * ball.keyboardMoveForce;
            ball.rb.AddForce(force, ForceMode.Force);
        }
    }
}
