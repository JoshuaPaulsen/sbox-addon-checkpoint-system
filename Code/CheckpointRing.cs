using Sandbox;
using System;
using System.Linq;
public sealed class CheckpointRing : Component
{
	[Property] public int CheckpointIndex { get; set; } = 0;
	[Property] public bool IsFinishLine { get; set; } = false;
	[Property] public bool IsLastCheckpoint { get; set; } = false;
	[Property] public float DetectionRadius { get; set; } = 150f;

	// Events — anyone can listen to these
	public static Action<int> OnCheckpointHit;
	public static Action OnFinishHit;

	bool _triggered = false;

	bool IsSequential => CheckpointManager.Instance?.Sequential ?? true;

	int TotalCheckpoints()
	{
		if ( CheckpointManager.Instance == null ) return 0;
		int count = 0;
		foreach ( var child in CheckpointManager.Instance.GameObject.Children )
		{
			var cp = child.Components.Get<CheckpointRing>();
			if ( cp is not null && !cp.IsFinishLine && !cp.IsLastCheckpoint )
				count++;
		}
		return count;
	}

	bool AllCheckpointsHit()
	{
		int total = TotalCheckpoints();
		int next = CheckpointManager.Instance?.NextCheckpoint ?? 0;
		return next >= total;
	}

	protected override void OnUpdate()
	{
		if ( _triggered ) return;

		var player = Scene.GetAllObjects( true )
			.FirstOrDefault( go => go.Tags.Has( "player" ) );

		if ( player == null ) return;

		float dist = Vector3.DistanceBetween( WorldPosition, player.WorldPosition );

		if ( dist >= DetectionRadius ) return;

		if ( IsFinishLine || IsLastCheckpoint )
		{
			if ( !AllCheckpointsHit() )
			{
				int remaining = TotalCheckpoints() - (CheckpointManager.Instance?.NextCheckpoint ?? 0);
				Log.Info( "[Checkpoint] Finish blocked — " + remaining + " remaining" );
				return;
			}
		}
		else if ( IsSequential )
		{
			int next = CheckpointManager.Instance?.NextCheckpoint ?? 0;
			if ( CheckpointIndex != next ) return;
		}

		_triggered = true;

		if ( IsFinishLine || IsLastCheckpoint )
		{
			Log.Info( "[Checkpoint] Finished!" );
			CheckpointManager.Instance?.OnFinishReached();
			OnFinishHit?.Invoke();

			var cube = CheckpointManager.Instance?.ColorCube;
			if ( cube is not null )
				cube.Tint = Color.Green;
		}
		else
		{
			Log.Info( "[Checkpoint " + CheckpointIndex + "] Hit" );
			CheckpointManager.Instance?.OnCheckpointReached( CheckpointIndex );
			OnCheckpointHit?.Invoke( CheckpointIndex );
		}
	}

	public void ResetCheckpoint()
	{
		_triggered = false;
	}
}
