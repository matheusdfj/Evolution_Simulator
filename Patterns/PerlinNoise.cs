using EvolutionProject.Model;
using Xna = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public static Dictionary<int, List<Corner>> CornerList = new Dictionary<int, List<Corner>>();

        public class Corner
        {

            public Vector2 position;
            public Vector2 direction;

            public Corner(Vector2 position, Vector2 direction)
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

                    var xIndex = (int)MathF.Floor(currentPixel.X / DefaultValues.MAP_GRID_SIZE);
                    var yIndex = (int)MathF.Floor(currentPixel.Y / DefaultValues.MAP_GRID_SIZE);
                    var corner = CornerList[i + xIndex][j + yIndex];
                    
                    cornerValues.Add(corner);

                }

            }

            return cornerValues;

        }

        public static void SetCornerValues(int _width, int _height)
        {

            var corners = new List<Vector2>();

            corners.Add(new Vector2(0, 1));
            corners.Add(new Vector2(1, 0));
            corners.Add(new Vector2(1, 1));
            corners.Add(new Vector2(0, -1));
            corners.Add(new Vector2(-1, 0));
            corners.Add(new Vector2(-1, -1));
            corners.Add(new Vector2(0, 1));
            corners.Add(new Vector2(1, 0));
            corners.Add(new Vector2(1, -1));
            corners.Add(new Vector2(-1, 1));


            for (var i = 0; i <= (int)MathF.Floor(_width / DefaultValues.MAP_GRID_SIZE); i++)
            {

                CornerList.Add(i, new List<Corner>());


                for(var j = 0; j <= (int)MathF.Floor(_height / DefaultValues.MAP_GRID_SIZE); j++)
                {

                    // Sem adicionar Seed no momento
                    Corner corner = new Corner(new Vector2(i, j) , corners[Random.Shared.Next(10)]);

                    CornerList[i].Add(corner);

                }

            }

        }

        public static Xna.Color[] SetPixelNoise(GraphicsDevice graphicsDevice)
        {

            Xna.Color[] data = new Xna.Color[DefaultValues.DISPLAY_WIDTH * DefaultValues.DISPLAY_HEIGHT];

            for (int i = 0; i < DefaultValues.DISPLAY_WIDTH; i++)
            {
                for(int j = 0; j < DefaultValues.DISPLAY_HEIGHT; j++)
                {

                    var x = PerlinNoiseFormula(new Vector2(i, j));

                    var cor = new Xna.Color();

                    if (x < -0.4f)
                    {
                        cor = new Xna.Color(100, 150, 215);
                    }
                    else if (x < 0.02f)
                    {
                        cor = new Xna.Color(125, 185, 230);
                    }
                    else if (x < 0.12f)
                    {
                        cor = new Xna.Color(235, 205, 95);
                    }
                    else if (x < 0.55f)
                    {
                        cor = new Xna.Color(90, 190, 70);
                    }
                    else if (x < 0.70f)
                    {
                        cor = new Xna.Color(45, 125, 45);
                    }
                    else if (x < 0.82f)
                    {
                        cor = new Xna.Color(185, 120, 50);
                    }
                    else if (x < 0.93f)
                    {
                        cor = new Xna.Color(115, 115, 115);
                    }
                    else
                    {
                        cor = new Xna.Color(245, 245, 245);
                    }


                    data[i + j * DefaultValues.DISPLAY_WIDTH] = cor;

                }

            }

            return data;

        }

        public static double PerlinNoiseFormula(Vector2 pixelPosition)
        {

            var x = (int)MathF.Floor(pixelPosition.X / DefaultValues.MAP_GRID_SIZE);
            var y = (int)MathF.Floor(pixelPosition.Y / DefaultValues.MAP_GRID_SIZE);

            var _cornerList = new List<Corner>();

            var xT = pixelPosition.X / DefaultValues.MAP_GRID_SIZE - x;
            var yT = pixelPosition.Y / DefaultValues.MAP_GRID_SIZE - y;

            _cornerList.Add(CornerList[x][y]);
            _cornerList.Add(CornerList[x + 1][y]);
            _cornerList.Add(CornerList[x][y + 1]);
            _cornerList.Add(CornerList[x + 1][y + 1]);

            var TL = new Vector2(xT, yT);
            var TR = new Vector2(xT - 1, yT);
            var BL = new Vector2(xT, yT - 1);
            var BR = new Vector2(xT - 1, yT - 1);

            var lerpTop = Single.Lerp(TL.X * _cornerList[0].direction.X + TL.Y * _cornerList[0].direction.Y, TR.X * _cornerList[1].direction.X + TR.Y * _cornerList[1].direction.Y, xT * xT * xT * (xT * (xT * 6 - 15) + 10));
            var lerpBottom = Single.Lerp(BL.X * _cornerList[2].direction.X + BL.Y * _cornerList[2].direction.Y, BR.X * _cornerList[3].direction.X + BR.Y * _cornerList[3].direction.Y, xT * xT * xT * (xT * (xT * 6 - 15) + 10));

            return Single.Lerp(lerpTop, lerpBottom, yT * yT * yT * (yT * (yT * 6 - 15) + 10));

        }

    }
}
