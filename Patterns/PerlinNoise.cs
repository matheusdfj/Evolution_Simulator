using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionProject.Patterns
{
    public class PerlinNoise
    {

        public static List<Corner> CornerList = new List<Corner>();

        public class Corner
        {

            public Vector2 position;
            public int direction;

            public Corner(Vector2 position, int direction)
            {

                this.position = position;
                this.direction = direction;

            }

        }

        public static List<Corner> GetCornerValues(Vector2 currentPixel)
        {

            var cornerValues = new List<Corner>();

            for(int i = -1; i <= 1; i += 2)
            {

                for(int j = -1; j <= 1; j += 2)
                {

                    var xIndex = (int)MathF.Floor(currentPixel.X / DefaultValues.HASH_GRID_SIZE);
                    var yIndex = (int)MathF.Floor(currentPixel.Y / DefaultValues.HASH_GRID_SIZE);
                    var corner = CornerList.Where(c => c.position == new Vector2(xIndex, yIndex)).First();
                    
                    cornerValues.Add(corner);

                }

            }

            return cornerValues;

        }

        public static void SetCornerValues(int _width, int _height)
        {

            for(var i = 0; i <= (int)MathF.Floor(_width / DefaultValues.HASH_GRID_SIZE); i++)
            {

                for(var j = 0; j <= (int)MathF.Floor(_height / DefaultValues.HASH_GRID_SIZE); j++)
                {

                    // Sem adicionar Seed no momento
                    Corner corner = new Corner(new Vector2(i, j) ,Random.Shared.Next(0, 8));

                    CornerList.Add(corner);

                }

            }

        }

        public static double GetPixelNoise(Vector2 position)
        {

            var xIndex = (int)MathF.Floor(position.X / DefaultValues.HASH_GRID_SIZE);
            var yIndex = (int)MathF.Floor(position.Y / DefaultValues.HASH_GRID_SIZE);

            var Corners = CornerList.Where(c => 
            (c.position.X == xIndex || c.position.X == xIndex + 1) &&
            (c.position.Y == yIndex || c.position.Y == yIndex + 1));



        }

    }
}
