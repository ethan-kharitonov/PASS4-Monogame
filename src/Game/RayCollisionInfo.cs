using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace PASS4
{
    public class RayCollisionInfo : IEquatable<RayCollisionInfo>
    {
        public Vector2 Intersection;
        public Vector2 Distance;
        public bool IsIntersecting;
        public List<Side> Sides;

        public RayCollisionInfo(Vector2 intersection, Vector2 distance, List<Side> sides, bool isIntersecting)
        {
            IsIntersecting = isIntersecting;
            Intersection = intersection;
            Distance = distance;
            Sides = sides;
        }

        public RayCollisionInfo(Vector2 intersection, Vector2 distance, Side side, bool isIntersecting)
        {
            IsIntersecting = isIntersecting;
            Intersection = intersection;
            Distance = distance;
            Sides = new List<Side> { side };
        }

        public RayCollisionInfo(bool isIntersecting)
        {
            if (isIntersecting)
            {
                throw new ArgumentException("This constructor is used to initalise collision info for a ray which did not collide");
            }

            Intersection = Vector2.Zero;
            Distance = Vector2.Zero;
            Sides = new List<Side>();
        }

        public RayCollisionInfo()
        {
            Sides = new List<Side>();
        }

        public override string ToString()
        {
            string sidesString = "";

            foreach(Side side in Sides)
            {
                sidesString += side;
                sidesString += " ";
            }
            return $"Intersection: {Intersection}, Distance: {Distance}, Sides: {sidesString}, IsIntersecting: {IsIntersecting}";
        }

        public override bool Equals(object obj) => Equals(obj as RayCollisionInfo);

        public bool Equals(RayCollisionInfo other) => other != null &&
            Intersection.Equals(other.Intersection) &&
            Distance.Equals(other.Distance) &&
            IsIntersecting == other.IsIntersecting &&
            Sides.SequenceEqual(other.Sides);

        public override int GetHashCode() => HashCode.Combine(Intersection, Distance, IsIntersecting, Sides);

        public static bool operator ==(RayCollisionInfo left, RayCollisionInfo right) => EqualityComparer<RayCollisionInfo>.Default.Equals(left, right);

        public static bool operator !=(RayCollisionInfo left, RayCollisionInfo right) => !(left == right);
    }
}
