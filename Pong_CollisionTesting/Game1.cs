using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Pong_CollisionTesting
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Loading Screen
        private Texture2D start;
        private Texture2D exit;
        private Texture2D loading;

        private Vector2 startPos;
        private Vector2 exitPos;

        private Thread backgroundThread;
        private bool isLoading = false;

        MouseState mouseState;
        MouseState oldMouseState;

        enum GameState
        {
            startMenu,
            loading,
            playing,
            gameOver
        }

        // Game
        KeyboardState keyboardState;
        SpriteFont currentScore;

        Texture2D wallTex, playerTex, ballTex;

        GameObject upperWall;
        GameObject lowerWall;
        GameObject playerOne;
        GameObject playerTwo;
        GameObject ball;

        int playerOneScore, playerTwoScore;
        Vector2 sizeOfScore;

        Vector2 winnerSize;
        Vector2 winnerPos;

        Texture2D background;
        private GameState gameState;
        
        bool canPlay = false;
        string isWinner;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            oldMouseState = Mouse.GetState();

            startPos = new Vector2((Window.ClientBounds.Width - 300) / 2, 200);
            exitPos = new Vector2((Window.ClientBounds.Width - 300) / 2, 300);
            
            gameState = GameState.startMenu;
            
            base.Initialize();
        }

        protected void LoadGame()
        {

            // Loading Textures
            wallTex = Content.Load<Texture2D>("wall");
            playerTex = Content.Load<Texture2D>("paddle");
            ballTex = Content.Load<Texture2D>("ball");

            // Loading Wall Objects
            upperWall = new GameObject(wallTex, Vector2.Zero);
            lowerWall = new GameObject(wallTex, new Vector2(0, Window.ClientBounds.Height - wallTex.Height));

            // Loading Player Objects
            Vector2 position;
            position = new Vector2(0, (Window.ClientBounds.Height - playerTex.Height) / 2);
            playerOne = new GameObject(playerTex, position);

            position = new Vector2((Window.ClientBounds.Width - playerTex.Width), (Window.ClientBounds.Height - playerTex.Height) / 2);
            playerTwo = new GameObject(playerTex, position);

            // Loading Ball Object
            position = new Vector2(playerOne.BoundBox.Right + 1, (Window.ClientBounds.Height - ballTex.Height) / 2);
            ball = new GameObject(ballTex, position, new Vector2(8f, -8f));

            Thread.Sleep(1000);
            gameState = GameState.playing;
            isLoading = true;

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Font
            currentScore = Content.Load<SpriteFont>("Score");

            start = Content.Load<Texture2D>("start");
            exit = Content.Load<Texture2D>("exit");

            loading = Content.Load<Texture2D>("loading");

            background = Content.Load<Texture2D>("background");
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Load the game
            if (gameState == GameState.loading && !isLoading)
            {
                // Set Background Thread
                backgroundThread = new Thread(LoadGame);
                isLoading = true;

                // Start Background Thread
                backgroundThread.Start();
            }
            
            // Game in Progress
            if (gameState == GameState.playing)
            {
                keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Space))
                    canPlay = true;

                if (canPlay)
                {
                    ball.position += ball.velocity;

                    if (keyboardState.IsKeyDown(Keys.W))
                        playerOne.position.Y -= 10f;
                    if (keyboardState.IsKeyDown(Keys.S))
                        playerOne.position.Y += 10f;
                    if (keyboardState.IsKeyDown(Keys.Up))
                        playerTwo.position.Y -= 10f;
                    if (keyboardState.IsKeyDown(Keys.Down))
                        playerTwo.position.Y += 10f;

                    CheckPlayerWallCollision();
                    CheckBallCollision();
                }
                sizeOfScore = currentScore.MeasureString(playerOneScore + " | " + playerTwoScore);
                
                
            }

            // Wait for Mouse Click
            mouseState = Mouse.GetState();
            if (oldMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                MouseClicked(mouseState.X, mouseState.Y);
            oldMouseState = mouseState;

            if (gameState == GameState.playing && isLoading)
            {
                LoadGame();
                isLoading = false;
            }

            // Gameover
            if (gameState == GameState.gameOver)
            {
                playerOneScore = 0;
                playerTwoScore = 0;
                KeyboardState newState = Keyboard.GetState();
                if (newState.IsKeyDown(Keys.Space))
                    gameState = GameState.startMenu;

                winnerSize = currentScore.MeasureString(isWinner + " Won! Press Space to Continue");
                winnerPos = new Vector2((Window.ClientBounds.Width - winnerSize.X) / 2, (Window.ClientBounds.Height - winnerSize.Y) / 2);
               
            }

            base.Update(gameTime);
        }

        private void CheckPlayerWallCollision()
        {
            if (playerOne.BoundBox.Intersects(upperWall.BoundBox))
                playerOne.position.Y = upperWall.BoundBox.Bottom;
            if (playerOne.BoundBox.Intersects(lowerWall.BoundBox))
                playerOne.position.Y = lowerWall.BoundBox.Y - playerOne.BoundBox.Height;

            if (playerTwo.BoundBox.Intersects(upperWall.BoundBox))
                playerTwo.position.Y = upperWall.BoundBox.Bottom;
            if (playerTwo.BoundBox.Intersects(lowerWall.BoundBox))
                playerTwo.position.Y = lowerWall.BoundBox.Y - playerTwo.BoundBox.Height;
        }

        private void CheckBallCollision()
        {
            if (ball.BoundBox.Intersects(upperWall.BoundBox) || ball.BoundBox.Intersects(lowerWall.BoundBox))
            {
                ball.velocity.Y *= -1;
                ball.position += ball.velocity;
            }

            if (ball.BoundBox.Intersects(playerOne.BoundBox) || ball.BoundBox.Intersects(playerTwo.BoundBox))
            {
                ball.velocity.X *= -1;
                ball.position += ball.velocity;
            }

            if (ball.position.X < -ball.BoundBox.Width)
            {
                playerTwoScore++;
                if (playerTwoScore >= 11 && playerOneScore < playerTwoScore - 1)
                {
                    isWinner = "Player Two";
                    gameState = GameState.gameOver;
                }
                ResetPosition();
            }
                
            if (ball.position.X > Window.ClientBounds.Width)
            {
                playerOneScore++;
                if (playerOneScore >= 11 && playerTwoScore < playerOneScore - 1)
                {
                    isWinner = "Player One";
                    gameState = GameState.gameOver;
                }
                ResetPosition();
            }
        }

        private void ResetPosition()
        {
            playerOne.position.Y = (Window.ClientBounds.Height - playerTex.Height) / 2;

            playerTwo.position.Y = (Window.ClientBounds.Height - playerTex.Height) / 2;

            ball.position.Y = (Window.ClientBounds.Height - ballTex.Height) / 2;
            ball.position.X = playerOne.BoundBox.Right + 1;
            ball.velocity = new Vector2(8f, -8f);
            canPlay = false;
        }

        private void MouseClicked(int x, int y)
        {
            // Create a 10x10pixel rectangle around click
            Rectangle clickRect = new Rectangle(x, y, 10, 10);

            if (gameState == GameState.startMenu)
            {
                Rectangle startBtnRect = new Rectangle((int)startPos.X, (int)startPos.Y, start.Width, start.Height);
                Rectangle exitBtnRect = new Rectangle((int)exitPos.X, (int)exitPos.Y, exit.Width, exit.Height);
                if (clickRect.Intersects(startBtnRect))
                {
                    gameState = GameState.loading;
                    isLoading = false;
                }
                else if (clickRect.Intersects(exitBtnRect))
                    Exit();
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(); 
            spriteBatch.Draw(background, new Rectangle(0, 0, background.Width, background.Height), Color.White);

            // Start Menu
            if (gameState == GameState.startMenu)
            {
                spriteBatch.Draw(start, startPos, Color.White);
                spriteBatch.Draw(exit, exitPos, Color.White);
                spriteBatch.DrawString(currentScore, "Samuel's Pong", new Vector2((Window.ClientBounds.Width - currentScore.MeasureString("Samuel's Pong").X) / 2, 75), Color.White);
            }

            // Loading Screen
            if (gameState == GameState.loading) 
                spriteBatch.Draw(loading, new Vector2((Window.ClientBounds.Width - loading.Width) / 2, (Window.ClientBounds.Height - loading.Height) / 2), Color.White);

            // Playing
            if (gameState == GameState.playing)
            {

                upperWall.Draw(spriteBatch);
                lowerWall.Draw(spriteBatch);

                playerOne.Draw(spriteBatch);
                playerTwo.Draw(spriteBatch);

                ball.Draw(spriteBatch);

                spriteBatch.DrawString(currentScore, playerOneScore + " | " + playerTwoScore, new Vector2((Window.ClientBounds.Width - sizeOfScore.X) / 2, 100), Color.White);
                if (!canPlay)
                    spriteBatch.DrawString(currentScore, "Press Space to Begin", new Vector2((Window.ClientBounds.Width - currentScore.MeasureString("Press Space to Begin").X) / 2, (Window.ClientBounds.Height - currentScore.MeasureString("Press Space to Begin").Y) / 2), Color.White);
            }

            // Gameover
            if (gameState == GameState.gameOver)
            {
                spriteBatch.DrawString(currentScore, isWinner + " Won! Press Space to Continue", winnerPos, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
