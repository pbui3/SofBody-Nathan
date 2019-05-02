using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathPhysSoftBody
{
    public class Spring
    {
        // Properties
        public int pI { get; set; }//Point index 1 for soft body
        public int pJ { get; set; }//POint index 2 for soft body
        public double K { get; set; }
        public double LRest { get; set; }
        public double nx { get; set; } //normal vector (x)
        public double ny { get; set; } //normal vector (y)
        // Constructors
        public Spring(double LRest, double LStretched,double force)
        {
            this.LRest = LRest;
            K = Math.Abs(force / (LStretched-LRest));
        }//eom

        public Spring(double k, double lRest)
        {
            K = k;
            LRest = lRest;
        }//eom

        //1.a
        

        #region Class Methods
        // 2.a - Calculate restorative force
        public double RestitutionForce(double LStrechted)
        {
            return (K*-1)*(LRest-LStrechted);
        }

        // 2.b - Calculate stretched length
        public double StretchedLength(double force)
        {
            return LRest + (force / K);
        }

        // 2.c - Calculate frequency of oscillation
        public double Oscillation(double mass)
        {
            double os = Math.Sqrt(K / mass);
            return os / (2 * Math.PI);
        }

        // 2.d - Calculate Velocity at rest length - Does not work

        public double Velocity(double amplitude, double mass)
        {

            double freq = (1 / (2 * Math.PI)) * (Math.Sqrt(K / mass));
            double omega = 2 * Math.PI * freq;
            return amplitude * omega;
        }

        #endregion
    }//eoc
}//eon
