using Sandbox;
using System;

public sealed class CheckpointManager : Component
{
	public static CheckpointManager Instance { get; private set; }

	[Property] public bool Sequential { get; set; } = true;
	[Property] public ModelRenderer ColorCube { get; set; }
	[Property] public CheckpointRing FinishLine { get; set; }
	[Property] public CheckpointRing LastCheckpoint { get; set; }

	// Events
	public Action<int> OnCheckpointReachedEvent;
	public Action OnFinishReachedEvent;

	public int NextCheckpoint { get; private set; } = 0;

	protected override void OnAwake()
	{
		Instance = this;

		if ( FinishLine is not null )
		{
			FinishLine.IsFinishLine = true;
			FinishLine.IsLastCheckpoint = false;
		}

		if ( LastCheckpoint is not null )
		{
			LastCheckpoint.IsLastCheckpoint = true;
			LastCheckpoint.IsFinishLine = false;
		}
	}

	protected override void OnStart()
	{
		NextCheckpoint = 0;
	}

	public void OnCheckpointReached( int index )
	{
		NextCheckpoint = index + 1;
		OnCheckpointReachedEvent?.Invoke( index );
		Log.Info( "[CheckpointManager] Checkpoint " + index + " reached. Next: " + NextCheckpoint );
	}

	public void OnFinishReached()
	{
		OnFinishReachedEvent?.Invoke();
		Log.Info( "[CheckpointManager] Finish reached!" );
	}

	public void ResetAll()
	{
		NextCheckpoint = 0;
		foreach ( var child in GameObject.Children )
		{
			var cp = child.Components.Get<CheckpointRing>();
			cp?.ResetCheckpoint();
		}
		Log.Info( "[CheckpointManager] Reset all checkpoints" );
	}
}
