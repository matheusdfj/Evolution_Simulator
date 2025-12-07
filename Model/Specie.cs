using System;
using System.Collections.Generic;
using Xna = Microsoft.Xna.Framework;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EvolutionProject;

namespace EvolutionProject.Model
{
    public class Specie
    {

        private Xna.Color color;
        private Xna.Vector2 position;
        private int lifeTime;
        private int remainingLifeTime;
        private Boolean hasChildrenThisYear;
        private int childrenQuantity;

        public Specie()
        {
            this.color = DefaultValues.defaultColor;
            this.position = DefaultValues.defaultPosition();
            this.lifeTime = DefaultValues.defaultLifeTime;
            this.remainingLifeTime = DefaultValues.defaultLifeTime;
            this.hasChildrenThisYear = false;
            this.childrenQuantity = 0;
        }
        public Specie(Xna.Color _color, Xna.Vector2 _position, int _defaultLifeTime)
        {

            this.color = _color;
            this.position = _position;
            this.lifeTime = _defaultLifeTime;
            this.lifeTime = _defaultLifeTime;
            this.childrenQuantity = 0;

        }

        public Specie(Xna.Color _color1, Xna.Color _color2, Xna.Vector2 _position, int _defaultLifeTime1, int _defaultLifeTime2)
        {

            var color = new Xna.Color(
                (int)((_color1.R + _color2.R) / 2),
                (int)((_color1.G + _color2.G) / 2),
                (int)((_color1.B + _color2.B) / 2)
                );

            var lifeTime = Random.Shared.Next(
                    Math.Min(_defaultLifeTime1, _defaultLifeTime2)
                   - 1,
                Math.Max(
                    _defaultLifeTime1, _defaultLifeTime2) + 1);


            this.color = Mutations.colorMutation(color);
            this.position = Mutations.positionMutation(_position);
            this.lifeTime = Mutations.lifeTimeMutation(lifeTime);
            this.remainingLifeTime = this.lifeTime;
            this.childrenQuantity = 0;

        }

        public Xna.Color getColor()
        {

            return this.color;

        }
        public Xna.Vector2 getPosition()
        {

            return this.position;

        }
        public int getLifeTime() 
        { 
        
            return this.lifeTime;

        }
        public int getRemainingLifeTime()
        {

            return this.remainingLifeTime;

        }
        public Boolean getHasChildThisYear()
        {

            return this.hasChildrenThisYear;

        }

        public int getChildrenQuantity()
        {

            return this.childrenQuantity;

        }

        public void setColor(Xna.Color _color)
        {

            this.color = _color;

        }
        public void setPosition(Xna.Vector2 _position)
        {

            this.position = _position;

        }
        public void setLifeTime(int _lifeTime)
        {

            this.lifeTime = _lifeTime;

        }

        public void setRemainingLifeTime(int _lifeTime = 1)
        {

            this.remainingLifeTime -= _lifeTime;

        }

        public void setHasChildThisYear(Boolean _hasChildThisYear)
        {

            this.hasChildrenThisYear = _hasChildThisYear;

        }

        public void setChildrenQuantity()
        {

            this.childrenQuantity++;

        }

    }
}
