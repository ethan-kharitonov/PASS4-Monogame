//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: RayCollisionInfo.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Stores information about a collision between a ray (line) and a rectangle
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PASS4
{
    public class RayCollisionInfo : IEquatable<RayCollisionInfo>
    {
        //Stores the intersection point, distance between the collisions and the start of the ray,
        //a bool indicating if there was an intersection, and the sides of the rec that the ray hit
        public Vector2 Intersection;
        public Vector2 Distance;
        public bool IsIntersecting;
        public List<Side> Sides;

        /// <summary>
        /// Creates a new RayCollisionInfo object given all the variables above
        /// </summary>
        /// <param name="intersection">The intersection point</param>
        /// <param name="distance">The distance between the start of the ray and and intersection point</param>
        /// <param name="side">The sides of the square that the ray collided with</param>
        /// <param name="isIntersecting">Idicates whether there was a collision</param>
        public RayCollisionInfo(Vector2 intersection, Vector2 distance, Side side, bool isIntersecting)
        {
            IsIntersecting = isIntersecting;
            Intersection = intersection;
            Distance = distance;
            Sides = new List<Side> { side };
        }


        /// <summary>
        /// Used to create an RayCollisionInfo for a ray that did not collide
        /// </summary>
        /// <param name="isIntersecting">Should always be false</param>
        public RayCollisionInfo(bool isIntersecting)
        {
            //Throw an exception if this constructor is not used correclty
            if (isIntersecting)
            {
                throw new ArgumentException("This constructor is used to initalise collision info for a ray which did not collide");
            }

            //Set all the properties to their default values
            Intersection = Vector2.Zero;
            Distance = Vector2.Zero;
            Sides = new List<Side>();
        }

        /// <summary>
        /// Creates an empty RayCollisionInfo and initializes the sides list
        /// </summary>
        public RayCollisionInfo()
        {
            Sides = new List<Side>();
        }

        /// <summary>
        /// Creates a string representing this object
        /// </summary>
        /// <returns>A string representing this object</returns>
        public override string ToString()
        {
            string sidesString = "";

            //Converts the sides into a string
            foreach(Side side in Sides)
            {
                sidesString += side;
                sidesString += " ";
            }

            //Conects it with the rest and returns
            return $"Intersection: {Intersection}, Distance: {Distance}, Sides: {sidesString}, IsIntersecting: {IsIntersecting}";
        }

        /// <summary>
        /// Compares two RayCollisionInfo objects
        /// </summary>
        /// <param name="obj">The other object</param>
        /// <returns>True if all their values are the same, false otherwise</returns>
        /// <see cref="Equals(RayCollisionInfo)"/>
        public override bool Equals(object obj) => Equals(obj as RayCollisionInfo);

        /// <summary>
        /// Compares two RayCollisionInfo objects
        /// </summary>
        /// <param name="obj">The other object</param>
        /// <returns>True if all their values are the same, false otherwise</returns>
        public bool Equals(RayCollisionInfo other) => other != null &&
            Intersection.Equals(other.Intersection) &&
            Distance.Equals(other.Distance) &&
            IsIntersecting == other.IsIntersecting &&
            Sides.SequenceEqual(other.Sides);

        /// <summary>
        /// Gets the hascode of this RayCollisionInfo object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(Intersection, Distance, IsIntersecting, Sides);


        /// <summary>
        /// Defines the == operator for two RayCollisionInfo obejcts
        /// </summary>
        /// <param name="left">One RayCollisionInfo</param>
        /// <param name="right">Another RayCollisionInfo</param>
        /// <returns>True if they are equal to eachother, false otherwise</returns>
        public static bool operator ==(RayCollisionInfo left, RayCollisionInfo right) => EqualityComparer<RayCollisionInfo>.Default.Equals(left, right);

        /// <summary>
        /// Defines the != operator for two RayCollisionInfo objects
        /// </summary>
        /// <param name="left">One RayCollisionInfo</param>
        /// <param name="right">Another RayCollisionInfo</param>
        /// <returns>True if they are not equal to each other, false otherwise</returns>
        public static bool operator !=(RayCollisionInfo left, RayCollisionInfo right) => !(left == right);
    }
}
