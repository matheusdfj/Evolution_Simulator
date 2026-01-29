using EvolutionProject.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EvolutionProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _texturaBase;
        private Boolean _firstIteration = true;
        private Texture2D _texturaFundo;
        private int _yearsCount = 0;
        private int _populationCount = DefaultValues.startPopulation;
        private Dictionary<int, List<Specie>> _population;
        private int _populationHashWidth, _populationHashHeight, _populationHashFactor;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Quanto menor, mais grids
            _populationHashFactor = DefaultValues.maxReprodutionDistance * 1;

            // Quanto maior, menos grids
            // 192
            _populationHashWidth = (int)(MathF.Floor(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / _populationHashFactor));
            // 108
            _populationHashHeight = (int)(MathF.Floor(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / _populationHashFactor));


            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.IsFullScreen = false;
            _graphics.HardwareModeSwitch = false;


            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 20d);


            _graphics.ApplyChanges();
        }

        private Texture2D CriarBolinhaComBorda(int raio, int espessuraBorda)
        {
            // O diâmetro é o dobro do raio (largura total)
            int diametro = raio * 2;

            Texture2D textura = new Texture2D(GraphicsDevice, diametro, diametro);
            Color[] dados = new Color[diametro * diametro];

            Vector2 centro = new Vector2(raio, raio);

            for (int i = 0; i < dados.Length; i++)
            {
                int x = i % diametro;
                int y = i / diametro;

                // Calcula a distância desse pixel até o centro
                float distancia = Vector2.Distance(new Vector2(x, y), centro);

                // 1. FORA DO CÍRCULO: Transparente (recorta os cantos do quadrado)
                if (distancia > raio)
                {
                    dados[i] = Color.Transparent;
                }
                // 2. NA BORDA: Pinta de Preto (ou Cinza Escuro para ficar suave)
                else if (distancia > raio - espessuraBorda)
                {
                    dados[i] = new Color(20, 20, 20); // Quase preto
                }
                // 3. NO MIOLO: Branco (Isso é importante! O branco absorve a cor que você passar no Draw)
                else
                {
                    dados[i] = Color.White;
                }
            }

            textura.SetData(dados);
            return textura;
        }

        private Texture2D CriarFundoTermico(int largura, int altura)
        {
            Texture2D textura = new Texture2D(GraphicsDevice, largura, altura);
            Color[] dados = new Color[largura * altura];

            // Otimização: Vamos preencher coluna por coluna
            for (int x = 0; x < largura; x++)
            {
                // Calcula a temperatura deste X (de 0.0 a 1.0)
                float temperatura = (float)x / (float)largura;

                // Mistura as cores: Começa em Azul Escuro e vai virando Vermelho
                // Você pode mudar Color.DarkBlue e Color.OrangeRed para o que preferir
                Color corDaColuna = Color.Lerp(Color.DarkBlue, Color.OrangeRed, temperatura);

                // Preenche todos os pixels verticais dessa coluna com a mesma temperatura
                for (int y = 0; y < altura; y++)
                {
                    dados[x + y * largura] = corDaColuna;
                }
            }

            textura.SetData(dados);
            return textura;
        }

        public Color GetCorDoAmbiente(Vector2 posicao)
        {
            float larguraTela = _graphics.PreferredBackBufferWidth;

            // 1. Descobre a porcentagem da tela (0.0 esquerda -> 1.0 direita)
            // O Math.Clamp garante que não dê erro se o bicho sair um pouquinho da tela
            float porcentagemX = Math.Clamp(posicao.X / larguraTela, 0f, 1f);

            // 2. Calcula a cor exata usando a mesma mistura do fundo
            // IMPORTANTE: Use as MESMAS cores que você usou no CriarFundoTermico
            return Color.Lerp(Color.DarkBlue, Color.OrangeRed, porcentagemX);
        }

        protected override void Initialize()
        {

            _population = new Dictionary<int, List<Specie>>();

            for (int i = 0; i < DefaultValues.startPopulation; i++)
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

            _texturaFundo = CriarFundoTermico(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight
            );

            _texturaBase = CriarBolinhaComBorda(100, 10);
            //_texturaBase.SetData(new[] { Color.White });
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

                        //specie.setPosition(Mutations.positionMutation(specie.getPosition()));
                        specie.setRemainingLifeTime();
                        // (Especie Atual Gerada na IT 1, Fact Population, Fact Population Count + Current new Population Cached)
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
                        
                        // Verificar Remove At
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
                    0.1f,
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
