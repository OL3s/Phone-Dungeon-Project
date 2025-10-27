using Godot;
using InputManager;
using System;

public partial class PlayerMovement : Node
{
	[Export] public bool EnableDebugLogs = false;
	[Export] public CharacterBody2D playerBody;
	[Export] public PlayerInput playerInput;
	[Export] public float MoveSpeed = 100f;

	public override void _Ready()
	{
		// Debug check for playerBody
		if (playerBody == null)
			throw new Exception("[PlayerMovement] playerBody is not assigned!");
		if (playerInput == null)
			throw new Exception("[PlayerMovement] playerInput is not assigned!");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		InputValues inputMovement = playerInput.GetInput(InputPositionType.Movement);
		playerBody.Velocity = inputMovement.Output * MoveSpeed;
		playerBody.MoveAndSlide();
		if (playerBody.Velocity != Vector2.Zero && EnableDebugLogs)
			GD.Print($"[PlayerMovement] Moving with velocity: {playerBody.Velocity} => pos: {playerBody.Position}");
	}
}
