using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FMOD.Studio;

namespace shootingGallery;

public static class MySounds
{
    public static Song bgMusic;
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FMOD.Studio.System fmodStudioSystem;

    Texture2D targetSprite;
    Texture2D crosshairsSprite;
    Texture2D backgroundSprite;
    SpriteFont gameFont;

    Vector2 targetPosition = new Vector2(300, 300);
    const int targetRadius = 32;

    MouseState mState;
    bool mReleased = true;
    int score = 0;

    double timer = 90;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        FMOD.Studio.System.create(out fmodStudioSystem);
        fmodStudioSystem.getCoreSystem(out var fmodSystem);
        fmodSystem.setDSPBufferSize(256, 4);
        fmodStudioSystem.initialize(
            128,
            FMOD.Studio.INITFLAGS.NORMAL,
            FMOD.INITFLAGS.NORMAL,
            (IntPtr)0
        );
        fmodStudioSystem.loadBankFile(
            // adjust this path to wherever you want to keep your .bank files
            "Master.bank",
            FMOD.Studio.LOAD_BANK_FLAGS.NORMAL,
            out Bank bank
        );
        fmodStudioSystem.loadBankFile(
            // adjust this path to wherever you want to keep your .bank files
            "Master.strings.bank",
            FMOD.Studio.LOAD_BANK_FLAGS.NORMAL,
            out Bank strings
        );
        fmodStudioSystem.loadBankFile(
            // adjust this path to wherever you want to keep your .bank files
            "Sounds.bank",
            FMOD.Studio.LOAD_BANK_FLAGS.NORMAL,
            out Bank sounds
        );

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        targetSprite = Content.Load<Texture2D>("green_ball_mod");
        crosshairsSprite = Content.Load<Texture2D>("pixel_crosshairs");
        backgroundSprite = Content.Load<Texture2D>("sky");
        gameFont = Content.Load<SpriteFont>("galleryFont");
        // MySounds.bgMusic = Content.Load<Song>("sounds/I Miss You");
        // MediaPlayer.Play(MySounds.bgMusic);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (timer > 0)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (timer < 0)
        {
            timer = 0;
        }

        // TODO: Add your update logic here
        mState = Mouse.GetState();

        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
        {
            float mouseTargetDist = Vector2.Distance(targetPosition, mState.Position.ToVector2());
            if (mouseTargetDist < targetRadius && timer > 0)
            {
                score++;

                var path = "event:/test";
                fmodStudioSystem.getEvent(path, out EventDescription evDesc);
                evDesc.createInstance(out EventInstance evInst);
                // evInst.start();
                // evInst.release();

                Random rand = new Random();

                targetPosition.X = rand.Next(0, _graphics.PreferredBackBufferWidth);
                targetPosition.Y = rand.Next(0, _graphics.PreferredBackBufferHeight);
            }
            mReleased = false;
        } 

        if (mState.LeftButton == ButtonState.Released)
        {
            mReleased = true;
        }

        fmodStudioSystem.update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.WhiteSmoke);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        // _spriteBatch.Draw(backgroundSprite, new Vector2(0,0), Color.White);
        _spriteBatch.DrawString(gameFont, "Hits: " + score.ToString(), new Vector2(3, 3), Color.DimGray);
        _spriteBatch.DrawString(gameFont, "Time: " + Math.Ceiling(timer).ToString(), new Vector2(3, 30), Color.DimGray);

        if (timer > 0)
        {
            _spriteBatch.Draw(targetSprite, new Vector2(targetPosition.X - targetRadius, targetPosition.Y - targetRadius), Color.White);
        }

        _spriteBatch.Draw(crosshairsSprite, new Vector2(mState.X - 16, mState.Y - 16), Color.White);
        
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}
