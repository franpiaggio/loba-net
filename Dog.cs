using Godot;

public partial class Dog : AnimatedSprite2D
{
	[Export] public float Speed = 80f;

	private int direction = 1;
	private float halfWidth = 16f;

	private Marker2D _marker;

	public override async void _Ready()
	{
		// Visibilidad / orden
		Visible = true;
		Modulate = new Color(1, 1, 1, 1);
		SelfModulate = new Color(1, 1, 1, 1);
		ZIndex = 9999;
		ZAsRelative = false;

		// Debug (opcional)
		_marker = new Marker2D();
		_marker.Name = "DogMarker";
		AddChild(_marker);

		if (SpriteFrames != null && SpriteFrames.HasAnimation(Animation))
			Play();

		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		// Cachear ancho real del sprite
		Texture2D tex0 = SpriteFrames.GetFrameTexture(Animation, Frame);
		if (tex0 != null)
			halfWidth = tex0.GetWidth() * Scale.X / 2f;

		PlaceOnSafeFloor();

		GD.Print("DOG READY OK");
	}

	public override void _Process(double delta)
	{
		// 👉 USAR GLOBALPOSITION (coherente con PlaceOnSafeFloor)
		Vector2 pos = GlobalPosition;
		Rect2 usable = DisplayServer.ScreenGetUsableRect();

		pos.X += direction * Speed * (float)delta;

		float leftLimit  = usable.Position.X + halfWidth;
		float rightLimit = usable.End.X - halfWidth;

		if (pos.X <= leftLimit)
		{
			pos.X = leftLimit;
			direction = 1;
			FlipH = false;
		}
		else if (pos.X >= rightLimit)
		{
			pos.X = rightLimit;
			direction = -1;
			FlipH = true;
		}

		GlobalPosition = pos;
	}

	private void PlaceOnSafeFloor()
	{
		Rect2 usable = DisplayServer.ScreenGetUsableRect();

		float dogHeight = 57f;
		Texture2D tex = null;

		if (SpriteFrames != null && SpriteFrames.HasAnimation(Animation))
			tex = SpriteFrames.GetFrameTexture(Animation, Frame);

		if (tex != null)
			dogHeight = tex.GetHeight() * Scale.Y;

		float x = 200;
		float y = usable.Size.Y - (dogHeight / 2f); // AnimatedSprite2D centrado

		GlobalPosition = new Vector2(x, y);

		GD.Print("──────── DOG DEBUG ────────");
		GD.Print($"UsableRect: {usable}");
		GD.Print($"HalfWidth:  {halfWidth}");
		GD.Print($"GlobalPos:  {GlobalPosition}");
		GD.Print("──────────────────────────");
	}
}
