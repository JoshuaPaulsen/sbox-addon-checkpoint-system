using Editor;
using Sandbox;

[EditorTool]
[Title( "Checkpoint Placer" )]
[Icon( "add_location" )]
public class CheckpointPlacer : EditorTool
{
	int _nextIndex = 0;
	GameObject _manager;

	public override void OnEnabled()
	{
		EnsureManager();
		RecalculateIndex();
		Log.Info( "[CheckpointPlacer] Active. Next index: " + _nextIndex );
	}

	void EnsureManager()
	{
		foreach ( var go in Scene.GetAllObjects( true ) )
		{
			if ( go.Name == "CheckpointManager" )
			{
				_manager = go;
				return;
			}
		}

		_manager = new GameObject();
		_manager.Name = "CheckpointManager";
		_manager.Components.Create<CheckpointManager>();
		Log.Info( "[CheckpointPlacer] Created CheckpointManager" );
	}

	void RecalculateIndex()
	{
		if ( _manager == null ) return;

		int count = 0;
		foreach ( var child in _manager.Children )
		{
			var cp = child.Components.Get<CheckpointRing>();
			if ( cp is not null )
				count++;
		}
		_nextIndex = count;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();

		var ray = Gizmo.CurrentRay;
		var tr = Scene.Trace.Ray( ray, 10000f ).WithoutTags( "trigger" ).Run();

		if ( !tr.Hit ) return;

		var pos = tr.HitPosition + Vector3.Up * 10f;

		Gizmo.Draw.Color = Color.Cyan.WithAlpha( 0.5f );
		Gizmo.Draw.SolidSphere( pos, 80f );

		Gizmo.Draw.Color = Color.White;
		Gizmo.Draw.ScreenText( "Left click to place checkpoint " + _nextIndex, new Vector2( 10, 10 ) );

		if ( Gizmo.WasLeftMousePressed )
		{
			EnsureManager();
			RecalculateIndex();
			PlaceCheckpoint( pos );
		}
	}

	void PlaceCheckpoint( Vector3 position )
	{
		var go = new GameObject();
		go.Parent = _manager;
		go.WorldPosition = position;
		go.Name = "Checkpoint_" + _nextIndex;

		var col = go.Components.Create<SphereCollider>();
		col.Radius = 150f;
		col.IsTrigger = true;
		go.Tags.Add( "trigger" );

		var cp = go.Components.Create<CheckpointRing>();
		cp.CheckpointIndex = _nextIndex;
		cp.IsFinishLine = false;
		cp.IsLastCheckpoint = false;

		var model = go.Components.Create<ModelRenderer>();
		model.Model = Model.Load( "models/primitives/sphere.vmdl" );
		model.Tint = Color.Cyan;
		go.WorldScale = new Vector3( 0.8f, 0.8f, 3f );

		_nextIndex++;
		Log.Info( "[CheckpointPlacer] Placed Checkpoint_" + (_nextIndex - 1) );
	}
}
