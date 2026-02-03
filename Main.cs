using EvolutionProject.Model;
using EvolutionProject.Patterns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EvolutionProject
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _texturaBase;
        private Boolean _firstIteration = true;
        private Texture2D _texturaFundo;
        private int _yearsCount = 0;
        private int _populationCount = DefaultValues.START_POPULATION;
        private Dictionary<int, List<Specie>> _population;
        private int _populationHashWidth, _populationHashHeight;
        private Microsoft.Xna.Framework.Color[] _backgroundPixelsColor;

        /* NEXT FEATURES
         * BACKGROUND COLOR SURVIVOR FACTOR BASE
         * IN-GAME OPTIONS AND PAUSE MENU
         * SPECIE DATA INTERFACE ONCLICK
         * SPECIE DATA MODIFICATION INTERFACE ONCLICK
         * POPULATION DATA MODIFICATION VIA GRID SELECTION
         * CENTRALIZE START POPULATION
         * PERLIN NOISE PROCEDURAL BACKGROUND GENERATOR
         * PERLIN NOISE BACKGROUND TRANSFORMATION TIME LOOP
         * NEW SPECIE TEXTURE
         * SPECIE ENERGY DATA
         * SPECIE RESISTANCE DATA
         * SPECIE HUNGRY DATA AND HUNGRY FEATURES
         * SPECIE ATTACK FEATURES
         */

        public Main()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _populationHashWidth = (int)(MathF.Floor(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / DefaultValues.HASH_GRID_SIZE));
            _populationHashHeight = (int)(MathF.Floor(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / DefaultValues.HASH_GRID_SIZE));


            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.IsFullScreen = false;
            _graphics.HardwareModeSwitch = false;


            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / DefaultValues.SIMULATOR_SPEED);

            _graphics.ApplyChanges();

        }

        private Texture2D CriarBolinhaComBorda(int raio, int espessuraBorda)
        {
            int diametro = raio * 2;

            Texture2D textura = new Texture2D(GraphicsDevice, diametro, diametro);
            Color[] dados = new Color[diametro * diametro];

            Vector2 centro = new Vector2(raio, raio);

            for (int i = 0; i < dados.Length; i++)
            {
                int x = i % diametro;
                int y = i / diametro;

                float distancia = Vector2.Distance(new Vector2(x, y), centro);

                if (distancia > raio)
                {
                    dados[i] = Color.Transparent;
                }

                else if (distancia > raio - espessuraBorda)
                {
                    dados[i] = new Color(20, 20, 20);
                }

                else
                {
                    dados[i] = Color.White;
                }
            }

            textura.SetData(dados);
            return textura;
        }

        protected override void Initialize()
        {

            _population = new Dictionary<int, List<Specie>>();

            PerlinNoise.SetCornerValues(DefaultValues.DISPLAY_WIDTH, DefaultValues.DISPLAY_HEIGHT);
            _texturaFundo = new Texture2D(GraphicsDevice, DefaultValues.DISPLAY_WIDTH, DefaultValues.DISPLAY_HEIGHT);
            for (int i = 0; i < DefaultValues.START_POPULATION; i++)
            {

                var specie = new Specie();
                var XHashIndex = (int)(MathF.Floor(specie.getPosition().X / _populationHashWidth));
                var YHashIndex = (int)(MathF.Floor(specie.getPosition().Y / _populationHashHeight));
                var HashIndex = XHashIndex * 1000 + YHashIndex;

                if(!_population.ContainsKey(HashIndex)){

                    _population.Add(HashIndex, new List<Specie>());

                }

                _population[HashIndex].Add(specie);

            }

            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _texturaFundo.SetData(PerlinNoise.SetPixelNoise(GraphicsDevice));

            _texturaBase = CriarBolinhaComBorda(100, 10);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Window.Title = "Evolution Simulator: " + _yearsCount + " Years";
            _yearsCount++;

            var x = new Dictionary<int, List<Specie>>();
            
            foreach(List<Specie> species in _population.Values)
            {

                for (int i = species.Count - 1; i >= 0; i--)
                {

                    var specie = species[i];

                    if (!_firstIteration)
                    {

                        specie.setRemainingLifeTime();

                        var y = Mutations.reproductionMethod(specie, _population, _populationCount);
                        if (y != null)
                        {

                            var XHashIndex = (int)(MathF.Floor(y.getPosition().X / _populationHashWidth));
                            var YHashIndex = (int)(MathF.Floor(y.getPosition().Y / _populationHashHeight));
                            var HashIndex = XHashIndex * 1000 + YHashIndex;

                            if (!x.ContainsKey(HashIndex))
                            {

                                x.Add(HashIndex, new List<Specie>());

                            }

                            x[HashIndex].Add(y);
                            _populationCount++;

                        }

                    }

                    if (specie.getRemainingLifeTime() <= 0)
                    {

                        var XHashIndex = (int)(MathF.Floor(specie.getPosition().X / _populationHashWidth));
                        var YHashIndex = (int)(MathF.Floor(specie.getPosition().Y / _populationHashHeight));
                        var HashIndex = XHashIndex * 1000 + YHashIndex;
                        
                        _population[HashIndex].RemoveAt(i);
                        _populationCount--;

                    }

                }


            }

            foreach(List<Specie> species in x.Values)
            {

                for (int i = 0; i < species.Count; i++)
                {
                    var XHashIndex = (int)(MathF.Floor(species[i].getPosition().X / _populationHashWidth));
                    var YHashIndex = (int)(MathF.Floor(species[i].getPosition().Y / _populationHashHeight));
                    var HashIndex = XHashIndex * 1000 + YHashIndex;


                    if (!_population.ContainsKey(HashIndex))
                    {

                        _population.Add(HashIndex, new List<Specie>());

                    }
                    _population[HashIndex].Add(species[i]);
                }

            }

            _population = Mutations.PopulationReDraw(_population);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_texturaFundo, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            foreach (List<Specie> species in _population.Values)
            {
                foreach (Specie specie in species)
                {
                    _spriteBatch.Draw(
                    _texturaBase,
                    specie.getPosition(),
                    null,
                    specie.getColor(),
                    0f,
                    new Vector2(100, 100),
                    0.07f,
                    SpriteEffects.None,
                    0f
                    );
                }
            }

            _firstIteration = false;

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
