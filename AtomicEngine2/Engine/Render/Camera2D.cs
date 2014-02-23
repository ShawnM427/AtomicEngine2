using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

/// <summary>
/// The base interface for an object that can be focused on
/// </summary>
public interface IFocusable
{
    Vector2 Position { get; }
}

public interface ICamera2D
{
    /// <summary>
    /// Gets or sets the position of the camera
    /// </summary>
    /// <value>The position.</value>
    Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the move speed of the camera.
    /// The camera will tween to its destination.
    /// </summary>
    /// <value>The move speed.</value>
    float MoveSpeed { get; set; }

    /// <summary>
    /// Gets or sets the rotation of the camera.
    /// </summary>
    /// <value>The rotation.</value>
    float Rotation { get; set; }

    /// <summary>
    /// Gets the origin of the viewport (accounts for Scale)
    /// </summary>        
    /// <value>The origin.</value>
    Vector2 Origin { get; }

    /// <summary>
    /// Gets or sets the scale of the Camera
    /// </summary>
    /// <value>The scale.</value>
    float Scale { get; set; }

    /// <summary>
    /// Gets the screen center (does not account for Scale)
    /// </summary>
    /// <value>The screen center.</value>
    Vector2 ScreenCenter { get; }

    /// <summary>
    /// Gets the transform that can be applied to 
    /// the SpriteBatch Class.
    /// </summary>
    /// <see cref="SpriteBatch"/>
    /// <value>The transform.</value>
    Matrix Transform { get; }

    /// <summary>
    /// Gets or sets the focus of the Camera.
    /// </summary>
    /// <seealso cref="IFocusable"/>
    /// <value>The focus.</value>
    IFocusable Focus { get; set; }

    /// <summary>
    /// Determines whether the target is in view given the specified position.
    /// This can be used to increase performance by not drawing objects
    /// directly in the viewport
    /// </summary>
    /// <param name="destination">The destination rectangle to check</param>
    /// <returns>
    ///     <c>true</c> if the target is in view at the specified position; otherwise, <c>false</c>.
    /// </returns>
    bool IsInView(Rectangle destination);
}

public class Camera2D : GameComponent, ICamera2D
{
    private Vector2 _position;
    protected float _viewportHeight;
    protected float _viewportWidth;
    protected bool _tweenTrack = false;

    public Camera2D(Game game)
        : base(game)
    { game.Components.Add(this); Initialize(); }

    #region Properties

    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; }
    }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public float Scale { get; set; }
    public Vector2 ScreenCenter { get; protected set; }
    public Matrix Transform { get; set; }
    public IFocusable Focus { get; set; }
    public float MoveSpeed { get; set; }
    public bool TweenTrack { get { return _tweenTrack; } set { _tweenTrack = value; } }

    public float Left
    {
        get { return _position.X - (Origin.X * Scale); }
    }
    public float Right
    {
        get { return _position.X + (Origin.X * Scale); }
    }
    public float Top
    {
        get { return _position.Y - (Origin.Y * Scale); }
    }
    public float Bottom
    {
        get { return _position.Y + (Origin.Y * Scale); }
    }

    #endregion

    /// <summary>
    /// Called when the GameComponent needs to be initialized. 
    /// </summary>
    public override void Initialize()
    {
        _viewportWidth = Game.GraphicsDevice.Viewport.Width;
        _viewportHeight = Game.GraphicsDevice.Viewport.Height;

        ScreenCenter = new Vector2(_viewportWidth / 2, _viewportHeight / 2);
        Scale = 1;
        MoveSpeed = 1.25f;

        base.Initialize();
    }

    /// <summary>
    /// Updates this camera
    /// </summary>
    /// <param name="gameTime">The current game time</param>
    public override void Update(GameTime gameTime)
    {
        Origin = ScreenCenter / Scale;

        // Create the Transform used by any
        // spritebatch process
        Transform = Matrix.Identity *
                    Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                    Matrix.CreateScale(new Vector3(Scale, Scale, Scale));

        if (_tweenTrack)
        {
            // Move the Camera to the position that it needs to go
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _position.X += (Focus.Position.X - Position.X) * MoveSpeed * delta;
            _position.Y += (Focus.Position.Y - Position.Y) * MoveSpeed * delta;
        }
        else
        {
            _position = Focus.Position;
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Determines whether the target is in view given the specified position.
    /// This can be used to increase performance by not drawing objects
    /// directly in the viewport
    /// </summary>
    /// <param name="destination">The destination rectangle</param>
    /// <returns>
    ///     <c>true</c> if [is in view] [the specified position]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsInView(Rectangle destination)
    {
        // If the object is not within the horizontal bounds of the screen

        if (destination.Right < (Position.X - Origin.X) || destination.Left > (Position.X + Origin.X))
            return false;

        // If the object is not within the vertical bounds of the screen
        if (destination.Bottom < (Position.Y - Origin.Y) || destination.Top > (Position.Y + Origin.Y))
            return false;

        // In View
        return true;
    }
}