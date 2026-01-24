using System;
using System.Collections.Generic;
using Xna = Microsoft.Xna.Framework;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using EvolutionProject.Model;

namespace EvolutionProject
{
    public static class Mutations
    {

        private static int _height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        private static int _width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
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

        public static Specie reproductionMethod(Specie specie, Dictionary<int, List<Specie>> population)
        {

            var populationCount = 0;

            foreach(List<Specie> species in population.Values)
            {

                foreach(Specie _specie in species){

                    populationCount++;

                }

            }


            if(populationCount >= DefaultValues.maxPopulation || specie.getChildrenQuantity() >= DefaultValues.maxChildrenQuantity)
            {

                return null;

            }

            var x = getColorDistance(specie.getColor(), specie.getColor());

            if(Random.Shared.Next(0, 100) <= x * 100)
            {

                var y = findSimilarSpecie(specie, GetPossibleMatches(specie, population));
                if(y != null)
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

            

            for(int i = 0; i < population.Count; i++)
            {

                var x = population[i];

                if(x != specie)
                {

                    if(Math.Abs(specie.getColor().R - x.getColor().R) < DefaultValues.maxReprodutionDistance &&
                        Math.Abs(specie.getColor().G - x.getColor().G) < DefaultValues.maxReprodutionDistance &&
                        Math.Abs(specie.getColor().B - x.getColor().B) < DefaultValues.maxReprodutionDistance)
                    {

                        if(Math.Abs(specie.getPosition().X - x.getPosition().X) < DefaultValues.maxReproductionPosition &&
                            Math.Abs(specie.getPosition().Y - x.getPosition().Y) < DefaultValues.maxReproductionPosition)
                        {

                            return x;

                        }


                    }

                }

            }

            return null;
            /*
            var z = population.FindAll(y =>
            Math.Abs(specie.getColor().R - y.getColor().R) < DefaultValues.maxReprodutionDistance &&
            Math.Abs(specie.getColor().G - y.getColor().G) < DefaultValues.maxReprodutionDistance &&
            Math.Abs(specie.getColor().B - y.getColor().B) < DefaultValues.maxReprodutionDistance &&
            Math.Abs(specie.getPosition().X - y.getPosition().X) < DefaultValues.maxReproductionPosition &&
            Math.Abs(specie.getPosition().Y - y.getPosition().Y) < DefaultValues.maxReproductionPosition &&
            y != specie
            );

            if(x.Count != 0)
            {

                return x[Random.Shared.Next(0, x.Count)];

            }

            return null;
            */

        }

        public static List<Specie> GetPossibleMatches(Specie specie, Dictionary<int, List<Specie>> population) {

            var candidates = new List<Specie>();
            var _populationHashFactor = DefaultValues.maxReprodutionDistance * 10;
            var XHashIndex = (int)(MathF.Floor(specie.getPosition().X / _populationHashFactor));
            var YHashIndex = (int)(MathF.Floor(specie.getPosition().Y / _populationHashFactor));
            var HashIndex = XHashIndex * 1000 + YHashIndex;

            for (int i = 0; i <= 1; i++)
            {

                for (int j = 0; j <= 1; j++)
                {

                    if (population.ContainsKey(HashIndex + (i * 1000) + j))
                    {

                        foreach (Specie _specie in population[HashIndex + (i * 1000) + j])
                        {

                            candidates.Add(_specie);

                        }

                    }
                }
            }

            return candidates;

        }
}
