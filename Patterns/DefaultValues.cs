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
        public static int defaultLifeTime = 60;
        public static int START_POPULATION = 120;
        public static int MAX_POPULATION = 5000;
        public static int colorVariationValue = 16;
        public static int positionVariationValue = 40; 
        public static int GRID_BASE_MOVEMENT = 2;
        public static int maxReprodutionDistance = 25;
        public static int maxReproductionPosition = 50;
        public static int maxChildrenQuantity = 3;
        public static int HASH_GRID_SIZE = 40;
        public static float REPRODUCTION_FACTOR = 0.01f;
        public static double SIMULATOR_SPEED = 700d;

        public static Xna.Vector2 defaultPosition() {

            var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            return new Vector2(Random.Shared.Next(20, width - 20), Random.Shared.Next(20, height - 20));
        
        }


    }
}
