using System;
using System.Collections.Generic;
using Xna = Microsoft.Xna.Framework;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace EvolutionProject
{
    public static class DefaultValues
    {

        public static Xna.Color defaultColor = new Xna.Color(120, 120, 120);
        public static int defaultLifeTime = 500;
        public static int startPopulation = 20;
        public static int maxPopulation = 200;
        public static int colorVariationValue = 10;
        public static int positionVariationValue = 15;
        public static int maxReprodutionDistance = 10;
        public static int maxReproductionPosition = 6;
        public static int maxChildrenQuantity = 3;

        public static Xna.Vector2 defaultPosition() {

            var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            return new Vector2(Random.Shared.Next(1, width - 1), Random.Shared.Next(1, height - 1));
        
        }


    }
}
