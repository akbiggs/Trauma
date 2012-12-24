using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Trauma.Helpers;
using Trauma.Interface;
using Trauma.Rooms;

namespace Trauma.Engine
{
    /// <summary>
    /// The main engine controlling all of the game's components.
    /// </summary>
    public class GameEngine : Microsoft.Xna.Framework.Game
    {
        const float FADE_FACTOR = 0.5f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// All of the rooms in the game, in order.
        /// </summary>
        List<String> roomNames = new List<String>
            {
                "Intro_1"
            };

        Intro intro;
        TitleScreen titleScreen;
        GameMenu gameMenu;
        Room curRoom;
        Song curSong;
        Credits credits;
        GameState state;

        /// <summary>
        /// The main component in charge of the game at the moment.
        /// </summary>
        IController controller;

        public GameEngine(int width, int height, bool stretch)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferMultiSampling = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            state = GameState.Intro;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load content from managers
            ResourceManager.LoadTextures(Content);
            ResourceManager.LoadSounds(Content);

            // Initialize anything depending on loaded content.
            SetupState();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            Input.Update();

            GameState oldState = state;
            state = GetNextState();

            // handle state transition
            if (state != oldState)
            {
                if (state == GameState.Exit)
                    Exit();
                else
                    SetupState();
            }

            controller.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Set up the state of the game.
        /// </summary>
        private void SetupState()
        {
            switch (state)
            {
                case GameState.Intro:
                    intro = new Intro();
                    intro.Play();
                    controller = intro;
                    break;
                case GameState.TitleScreen:
                    titleScreen = new TitleScreen();
                    controller = titleScreen;
                    break;
                case GameState.Room:
                    curRoom = NextRoom();
                    controller = curRoom;
                    break;
                case GameState.GameMenu:
                    controller = gameMenu;
                    break;
                case GameState.Credits:
                    controller = credits;
                    break;
                default:
                    throw new InvalidOperationException("Can't setup this state.");
            }
        }

        /// <summary>
        /// Get the next room.
        /// </summary>
        /// <returns>The next room(level) of the game.</returns>
        private Room NextRoom()
        {
            if (!MoreLevels())
                throw new InvalidOperationException();
            
            return new Room(Color.Black, ResourceManager.LoadMap(roomNames.Pop(), Content));
        }

        /// <summary>
        /// Get the next state the game should be in.
        /// </summary>
        /// <returns>The next state of the game.</returns>
        private GameState GetNextState()
        {
            switch (state)
            {
                case GameState.Intro:
                    if (intro.Finished)
                        return GameState.TitleScreen;
                    break;

                case GameState.TitleScreen:
                    if (titleScreen.Finished)
                        return titleScreen.ExitSelected ? GameState.Exit : GameState.Room;
                    break;

                case GameState.Room:
                    if (curRoom.MenuRequested)
                        return GameState.GameMenu;
                    if (curRoom.Finished && !MoreLevels())
                        return GameState.Credits;
                    break;

                case GameState.Credits:
                    if (credits.Finished)
                        return GameState.Exit;
                    break;

                case GameState.GameMenu:
                    if (gameMenu.Finished)
                        return GameState.Room;
                    break;

                case GameState.Exit:
                    return GameState.Exit;

                // if we've defaulted, the game is in an unknown/unimplemented state, so crash.
                default:
                    throw new InvalidOperationException();
            }

            // if one of the cases fell through without returning, return
            // the current state of the game.
            return state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            controller.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Fade out the given screen.
        /// </summary>
        /// <param name="color">The color to fade out with.</param>
        /// <param name="speed">The speed to fade out at.</param>
        public static void FadeOut(Color color, FadeSpeed speed)
        {
            // TODO: Fade out the screen to the given color.
        }

        public static void FadeIn(Color color, FadeSpeed speed)
        {
            // TODO: Fade in the screen to the given color.
        }

        /// <summary>
        /// Whether or not there are any more levels remaining in the game.
        /// </summary>
        /// <returns></returns>
        private bool MoreLevels()
        {
            return roomNames.Count > 0;
        }
    }

    enum GameState
    {
        Intro,
        TitleScreen,
        Room,
        GameMenu,
        Credits,
        Exit
    }

    public enum FadeSpeed
    {
        Slow = 1,
        Medium = 2,
        Fast = 3
    }
}
