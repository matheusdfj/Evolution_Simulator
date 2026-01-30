using EvolutionProject.Model;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xna = Microsoft.Xna.Framework;

namespace EvolutionProject
{
    public static class Mutations
    {

        private static int _height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        private static int _width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

        public static bool ReproductionValidation(in Specie specie1, in Specie specie2)
        {

            if (specie1.getChildrenQuantity() >= DefaultValues.maxChildrenQuantity) return false;
            if (specie2.getChildrenQuantity() >= DefaultValues.maxChildrenQuantity) return false;
            if (!(Math.Abs(specie1.getColor().R - specie2.getColor().R) <= DefaultValues.maxReprodutionDistance)) return false;
            if (!(Math.Abs(specie1.getColor().G - specie2.getColor().G) <= DefaultValues.maxReprodutionDistance)) return false;
            if (!(Math.Abs(specie1.getColor().B - specie2.getColor().B) <= DefaultValues.maxReprodutionDistance)) return false;
            if (!(Math.Abs(specie1.getPosition().X - specie2.getPosition().X) <= DefaultValues.maxReproductionPosition)) return false;
            if (!(Math.Abs(specie1.getPosition().Y - specie2.getPosition().Y) <= DefaultValues.maxReproductionPosition)) return false;

            return true;

        }
        

        public static Xna.Color colorMutation(Xna.Color _color)
        {

            int r = Random.Shared.Next(0, 2) * 2 - 1;
            int g = Random.Shared.Next(0, 2) * 2 - 1;
            int b = Random.Shared.Next(0, 2) * 2 - 1;

            return new Xna.Color(
                Math.Max(0, Math.Min(255, (int)(_color.R + (Random.Shared.Next(0, DefaultValues.colorVariationValue) * r)))),
                Math.Max(0, Math.Min(255, (int)(_color.G + (Random.Shared.Next(0, DefaultValues.colorVariationValue) * g)))),
                Math.Max(0, Math.Min(255, (int)(_color.B + (Random.Shared.Next(0, DefaultValues.colorVariationValue) * b))))
                );

        }

        public static Xna.Vector2 positionMutation(Xna.Vector2 _position)
        {

            int x = Random.Shared.Next(0, 2) * 2 - 1;
            int y = Random.Shared.Next(0, 2) * 2 - 1;

            return new Xna.Vector2(
                Math.Clamp(_position.X + (Random.Shared.Next(0, DefaultValues.positionVariationValue) * x), 0, _width),
                Math.Clamp(_position.Y + (Random.Shared.Next(0, DefaultValues.positionVariationValue) * x), 0, _height)
                );

        }

        public static Xna.Vector2 MovementController(Xna.Vector2 _position)
        {

            int x = Random.Shared.Next(0, 2) * 2 - 1;
            int y = Random.Shared.Next(0, 2) * 2 - 1;

            return new Xna.Vector2(
                Math.Clamp(_position.X + (Random.Shared.Next(0, DefaultValues.GRID_BASE_MOVEMENT) * x), 0, _width),
                Math.Clamp(_position.Y + (Random.Shared.Next(0, DefaultValues.GRID_BASE_MOVEMENT) * x), 0, _height)
                );

        }
        public static int lifeTimeMutation(int _lifeTime)
        {

            return _lifeTime + 1;

        }

        public static float getColorDistance(Xna.Color color1, Xna.Color color2)
        {

            var r = color1.R - color2.R;
            var g = color1.G - color2.G;
            var b = color1.B - color2.B;

            var x = (r * r + g * g + b * b) / 195075;
            return x;

        }

        public static Specie reproductionMethod(Specie specie, Dictionary<int, List<Specie>> population, int populationCount)
        {

            if (populationCount >= DefaultValues.MAX_POPULATION) return null;

            var x = DefaultValues.REPRODUCTION_FACTOR;

            if (Random.Shared.Next(0, 100) <= x * 100)
            {

                var y = findSimilarSpecie(specie, GetPossibleMatches(specie, population));
                if (y != null)
                {

                    specie.setHasChildThisYear(true);
                    y.setHasChildThisYear(true);
                    specie.setChildrenQuantity();
                    return new Specie(specie.getColor(), y.getColor(), specie.getPosition(), specie.getLifeTime(), y.getLifeTime());

                }
            }

            return null;

        }

        public static Specie findSimilarSpecie(Specie specie, List<Specie> population)
        {

            for (int i = 0; i < population.Count; i++)
            {

                var x = population[i];

                if (x != specie)
                {

                    if (ReproductionValidation(specie, x))
                    {
                        return x; 
                    }

                }

            }

            return null;

        }

        public static List<Specie> GetPossibleMatches(Specie specie, Dictionary<int, List<Specie>> population)
        {

            var candidates = new List<Specie>();
            var _populationHashFactor = DefaultValues.HASH_GRID_SIZE;
            var _populationHashWidth = (int)(MathF.Floor(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / _populationHashFactor));
            var _populationHashHeight = (int)(MathF.Floor(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / _populationHashFactor));
            var XHashIndex = (int)(MathF.Floor(specie.getPosition().X / _populationHashWidth));
            var YHashIndex = (int)(MathF.Floor(specie.getPosition().Y / _populationHashHeight));
            var HashIndex = XHashIndex * 1000 + YHashIndex;

            for(int i = -1; i <= 1; i++)
            {

                for(int j = -1; j <= 1; j++)
                {

                    var currentKey = HashIndex + (i * 1000) + j;

                    if (population.ContainsKey(currentKey))
                    {

                        foreach(Specie _specie in population[currentKey])
                        {
                            if(_specie != specie) candidates.Add(_specie);
                        }


                    }

                }

            }

            Console.WriteLine("Total Enviado: " + candidates.Count);
            return candidates;

        }

        public static Dictionary<int, List<Specie>> PopulationReDraw(Dictionary<int, List<Specie>> _population)
        {


            var _populationHashWidth = (int)(MathF.Floor(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / DefaultValues.HASH_GRID_SIZE));
            var _populationHashHeight = (int)(MathF.Floor(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / DefaultValues.HASH_GRID_SIZE));

            Dictionary<int, List<Specie>> population = new Dictionary<int, List<Specie>>();

            foreach(List<Specie> species in _population.Values)
            {

                foreach(var specie in species)
                {

                    specie.setPosition(Mutations.MovementController(specie.getPosition()));

                    var XHashIndex = (int)(MathF.Floor(specie.getPosition().X / _populationHashWidth));
                    var YHashIndex = (int)(MathF.Floor(specie.getPosition().Y / _populationHashHeight));
                    var HashIndex = XHashIndex * 1000 + YHashIndex;

                    if (!population.ContainsKey(HashIndex))
                    {

                        population.Add(HashIndex, new List<Specie>());
                        
                    }

                    population[HashIndex].Add(specie);


                }


            }

            return population;

        }
    }
}