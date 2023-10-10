using Godot;
using System;

public partial class Main : Node
{
    [Export] public PackedScene MobScene { get; set; }

    private int _score;
    private Player _player;
    private Marker2D _startPosition;
    private Timer _startTimer, _mobTimer, _scoreTimer;
    private AudioStreamPlayer _music, _deathSound;
    private HUD _hud;

    public override void _Ready()
    {
        _player = GetNode<Player>("Player");
        _startPosition = GetNode<Marker2D>("StartPosition");
        _startTimer = GetNode<Timer>("StartTimer");
        _mobTimer = GetNode<Timer>("MobTimer");
        _scoreTimer = GetNode<Timer>("ScoreTimer");
        _music = GetNode<AudioStreamPlayer>("Music");
        _deathSound = GetNode<AudioStreamPlayer>("DeathSound");
        _hud = GetNode<HUD>("HUD");
    }

    public void NewGame()
    {
        _score = 0;

        _player.Start(_startPosition.Position);

        _hud.UpdateScore(_score);
        _hud.ShowMessage("Get Ready!");

        // Note that for calling Godot-provided methods with strings,
        // we have to use the original Godot snake_case name.
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

        _startTimer.Start();

        _music.Play();
    }

    public void GameOver()
    {
        _music.Stop();
        _deathSound.Play();
        _mobTimer.Stop();
        _scoreTimer.Stop();
        _hud.ShowGameOver();
    }

    private void OnScoreTimerTimeout()
    {
        _score++;
        _hud.UpdateScore(_score);
    }

    private void OnStartTimerTimeout()
    {
        _mobTimer.Start();
        _scoreTimer.Start();
    }

    private void OnMobTimerTimeout()
    {
        // Note: Normally it is best to use explicit types rather than the `var`
        // keyword. However, var is acceptable to use here because the types are
        // obviously Mob and PathFollow2D, since they appear later on the line.

        // Create a new instance of the Mob scene.
        Mob mob = MobScene.Instantiate<Mob>();

        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        // Set the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's position to a random location.
        mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Choose the velocity.
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }
}
