using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#region Additional Namespaces
#endregion
namespace MathPhysSoftBody
{
    //Code adapted from http://panoramx.ift.uni.wroc.pl/~maq/eng/index.php - How to Implement Pressure Soft Body Model
    //On creation, give the softbody a number of points to create a circle out of (or set each position) - need to create method to do that.
    //Create Ball() - Sets positions of all points based off 0,0
    //Accumulateforces() - calculates all forces for all points and springs - gravity, spring, and pressure forces.
    //IntegrateEuler() - applies sum force to each point - sets the points velocity then changes it's position based on velocity.


    class SoftBody
    {
        int windowWidth, windowHeight;
        public float mass, radius, springConstant, springDamp, gravity, finalPressure,pressure;
        public int numPoints, numSprings;
        Point[] myPoints;
        Spring[] mySprings;
        public float timeSlice;

        public SoftBody(int numPoints, float mass, float radius, float springConstant, float springDamp, float gravity, float finalPressure,int width,int height)
        {
            this.numPoints = numPoints;
            numSprings = numPoints;
            this.mass = mass;
            this.springConstant = springConstant;
            this.springDamp = springDamp;
            this.radius = radius;
            this.gravity = gravity;
            this.finalPressure = finalPressure;
            windowHeight = height;
            windowWidth = width;
            pressure = finalPressure;
            myPoints = new Point[numPoints];
            mySprings = new Spring[numSprings];
        }
        
        public void Update(GameTime gameTime)
        {
            timeSlice = .1f;
            AccumulateForces();
            IntegrateEuler();
            Debug.WriteLine(myPoints[0].Position);

            //if (pressure < finalPressure)
            //{
            //    pressure += finalPressure * .01f;
            //}
            //else
            //{
            //    pressure =0f;
            //}
        }

        public void CreateBox()
        {
            numPoints = 8;
            numSprings = 8;
            myPoints = new Point[8];
            mySprings = new Spring[8];

            //set points
            myPoints[0] = new Point();
            myPoints[1] = new Point();
            myPoints[2] = new Point();
            myPoints[3] = new Point();
            myPoints[4] = new Point();
            myPoints[5] = new Point();
            myPoints[6] = new Point();
            myPoints[7] = new Point();

            myPoints[7].Position = new Vector2(301, 300);
            myPoints[6].Position = new Vector2(350, 302);
            myPoints[5].Position = new Vector2(400, 301);
            myPoints[4].Position = new Vector2(402, 350);
            myPoints[3].Position = new Vector2(400, 405);
            myPoints[2].Position = new Vector2(351, 401);
            myPoints[1].Position = new Vector2(301, 400);
            myPoints[0].Position = new Vector2(303, 351);

            //set springs
            for (int i = 0; i < numPoints - 1; ++i)
            {
                AddSpring(i, i, i + 1);
            }
            AddSpring(numSprings - 1, numPoints - 1, 0);
        }

        public void CreateBall() // Sets points around 0,0 in a circle.
        {
           
            for (int i = 0; i < numPoints; ++i)     // create points
            {
                myPoints[i] = new Point();
                myPoints[i].Position = new Vector2
                    (radius * (float)Math.Sin(i * (2* Math.PI) / numPoints) + windowWidth/2, 
                    radius * (float)Math.Cos(i * (2 * Math.PI) / numPoints) + windowHeight/2);
            }

            // create springs
            for (int i = 0; i < numPoints - 1; ++i)
            {
                AddSpring(i, i, i + 1);
            }
            AddSpring(numSprings-1, numPoints-1, 0); //connect last point to first point
        }

        public void AddSpring(int index, int i, int j)
        {
            mySprings[index] = new Spring
                (springConstant, Math.Sqrt(Math.Pow(myPoints[i].Position.X - 
                myPoints[j].Position.X, 2) + Math.Pow(myPoints[i].Position.Y -
                myPoints[j].Position.Y, 2)));

            mySprings[index].pI = i;
            mySprings[index].pJ = j;
            //pythagorean to find distance betwen connected points
        }

        public void AccumulateForces()
        {
            int i;
            float x1, x2, y1, y2;       // positions of spring points p1, p2
            float length;         // length of p1 - p2 vector
            float xVector;         // vx1 - vx2
            float yVector;         // vy1 - vy2
            float f;                // hooke force value
            float Fx, Fy;           // force vector
            float volume = 0;           // volume of the body
            float pressureForce;        // pressure force value
            
            /* gravity */
            for (i = 0; i < numPoints; ++i)
            {

                myPoints[i].Force = new Vector2(0, mass * gravity);
            }
            
            /* spring force */
            for (i = 0; i < numSprings; ++i)
            {
                x1 = myPoints[mySprings[i].pI].Position.X;
                y1 = myPoints[mySprings[i].pI].Position.Y;
                x2 = myPoints[mySprings[i].pJ].Position.X;
                y2 = myPoints[mySprings[i].pJ].Position.Y;

                length = (float)Math.Sqrt(Math.Pow(x1 - x2,2) + Math.Pow(y1 - y2,2)); // distance between both points connected to spring
                if (length != 0.01f) //if points are at same place, force calculation is skipped.
                {
                    //get velocities of ends of spring
                    xVector = (float)(myPoints[mySprings[i].pI].Velocity.X - myPoints[mySprings[i].pJ].Velocity.X);
                    yVector = (float)(myPoints[mySprings[i].pI].Velocity.Y - myPoints[mySprings[i].pJ].Velocity.Y);

                    //calculate the force the spring applies based on velocities of ends. Resitituion force formula (
                    f = (float)(springConstant * (length - mySprings[i].LRest) + (xVector * (x1 - x2) + yVector * (y1 - y2)) * springDamp / length);

                    //Vector of the force
                    Fx = ((x1 - x2) / length) * f;
                    Fy = ((y1 - y2) / length) * f;

                    myPoints[mySprings[i].pI].Force -= new Vector2((float)Math.Round(Fx,5),(float)Math.Round(Fy,5));

                    myPoints[mySprings[i].pJ].Force += new Vector2((float)Math.Round(Fx,5), (float)Math.Round(Fy,5));
                }
               
                /* Calculate normal vectors to springs */
                mySprings[i].nx = (y1 - y2) / length;
                mySprings[i].ny = -(x1 - x2) / length;
            }

            /* pressure force */
            /* Calculate Volume of the Ball (Gauss Theorem) */
            
            for (i = 0; i < numSprings ; i++)
            {
                x1 = myPoints[mySprings[i].pI].Position.X;
                y1 = myPoints[mySprings[i].pI].Position.Y;
                x2 = myPoints[mySprings[i].pJ].Position.X;
                y2 = myPoints[mySprings[i].pJ].Position.Y;
                length = (float)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)); // square

                // Volume Formula:
                volume += 0.5f * Math.Abs(x1 - x2) * (float)Math.Abs(mySprings[i].nx) * (length);
               
            }
            
            //Apply Pressure force to all points based on volume.
            for (i = 0; i < numSprings - 1; i++)
            {
                x1 = myPoints[mySprings[i].pI].Position.X;
                y1 = myPoints[mySprings[i].pI].Position.Y;
                x2 = myPoints[mySprings[i].pJ].Position.X;
                y2 = myPoints[mySprings[i].pJ].Position.Y;
                length = (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)); // square
                                                                                        // root  of the distance
                pressureForce = length * pressure * (1 / volume);

                myPoints[mySprings[i].pI].Force += new Vector2((float)mySprings[i].nx * pressureForce, (float)mySprings[i].ny * pressureForce);
                myPoints[mySprings[i].pJ].Force += new Vector2((float)mySprings[i].nx * pressureForce, (float)mySprings[i].ny * pressureForce);
            }
        }

        public void IntegrateEuler()
        {
            int i;
            float dry, drx;  // dr for Euler integration

            /* Euler Integrator (second Newton's law) */

            for (i = 0; i < numPoints; ++i)
            {
                /* x */
                myPoints[i].Velocity += new Vector2((myPoints[i].Force.X / mass) * timeSlice, myPoints[i].Force.Y * timeSlice);
               
                drx = myPoints[i].Velocity.X * timeSlice;
                dry = myPoints[i].Velocity.Y * timeSlice;

                ///* Boundaries  X */
                if (myPoints[i].Position.X + drx < 0)
                {
                    drx = 0 - myPoints[i].Position.X;
                    myPoints[i].Velocity = new Vector2(-0.1f * myPoints[i].Velocity.X,0.95f * myPoints[i].Velocity.Y);
                }
                else
                /* Boundaries  X */
                if (myPoints[i].Position.X + drx > windowWidth)
                {
                    drx = windowWidth - myPoints[i].Position.X;
                    myPoints[i].Velocity = new Vector2(-0.1f * myPoints[i].Velocity.X, 0.95f * myPoints[i].Velocity.Y);
                }

                
                /* y */

               
                if (myPoints[i].Position.Y + dry > windowHeight-10)
                {
                    dry = windowHeight-10 - myPoints[i].Position.Y;
                    myPoints[i].Velocity = new Vector2(myPoints[i].Velocity.X, -0.1f * myPoints[i].Velocity.Y);
                }
                myPoints[i].Position += new Vector2(drx, dry);


                ///* fast chek if outside */
                if (myPoints[i].Position.X > windowWidth)
                    myPoints[i].Position = new Vector2(windowWidth, myPoints[i].Position.Y);
                if (myPoints[i].Position.Y > windowHeight)
                    myPoints[i].Position = new Vector2(myPoints[i].Position.X, windowHeight-10);
                if (myPoints[i].Position.X < 0)
                    myPoints[i].Position = new Vector2(0, myPoints[i].Position.Y);
                if (myPoints[i].Position.Y < 0)
                    myPoints[i].Position = new Vector2(myPoints[i].Position.X, 0);

            }
        }

        public void Draw(SpriteBatch spritebatch, Texture2D rect)
        {
            foreach(Point p in myPoints)
            {
                spritebatch.Draw(rect, p.Position, Color.Black);
            }
        }

    }
    


    class Point
    {
        
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Force { get; set; }

        public Point()
        {
            
        }

        public Point(Vector2 position, Vector2 velocity, Vector2 force)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Force = force;
        }
    }

}



