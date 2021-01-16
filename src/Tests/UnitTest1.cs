using Microsoft.Xna.Framework;
using NUnit.Framework;
using PASS4;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class Tests
    {
        private static readonly TestCaseData[] LineIntersectWithVerticalLineInput = new[]
        {
            new TestCaseData(new Line(new Vector2(37, 56), new Vector2(37, 100)), new Line(new Vector2(37, 56), new Vector2(37, 100)))
            {
                ExpectedResult = (new Vector2(37, 56), true),
                TestName = "2VerticalsSameLine"
            },

            new TestCaseData(new Line(new Vector2(37, 70), new Vector2(37, 110)), new Line(new Vector2(37, 56), new Vector2(37, 100)))
            {
                ExpectedResult = (new Vector2(37, 70), true),
                TestName = "2VerticalsStartOnVerticalLine"
            },

            new TestCaseData(new Line(new Vector2(37, 10), new Vector2(37, 80)), new Line(new Vector2(37, 56), new Vector2(37, 100)))
            {
                ExpectedResult = (new Vector2(37, 56), true),
                TestName = "2VerticalsEndOnVerticalLine"
            },

            new TestCaseData(new Line(new Vector2(37, 70), new Vector2(37, 80)), new Line(new Vector2(37, 56), new Vector2(37, 100)))
            {
                ExpectedResult = (new Vector2(37, 70), true),
                TestName = "2VerticalsLineInsideVertical"
            },

            new TestCaseData(new Line(new Vector2(37, 10), new Vector2(37, 110)), new Line(new Vector2(37, 56), new Vector2(37, 100)))
            {
                ExpectedResult = (new Vector2(37, 56), true),
                TestName = "2VerticalsVerticalInsideLineStartFirst"
            },

            new TestCaseData(new Line(new Vector2(37, 10), new Vector2(37, 110)), new Line(new Vector2(37, 100), new Vector2(37, 56)))
            {
                ExpectedResult = (new Vector2(37, 56), true),
                TestName = "2VerticalsVerticalInsideLineEndFirst"
            },

            new TestCaseData(new Line(new Vector2(20, 10), new Vector2(20, 110)), new Line(new Vector2(37, 100), new Vector2(37, 56)))
            {
                ExpectedResult = (Vector2.Zero, false),
                TestName = "2VerticalsNotIntersect"
            },

            new TestCaseData(new Line(new Vector2(20, 20), new Vector2(40, 40)), new Line(new Vector2(30, 0), new Vector2(30, 100)))
            {
                ExpectedResult = (new Vector2(30, 30), true),
                TestName = "LinesAndIntervalIntersect"
            },

            new TestCaseData(new Line(new Vector2(40, 40), new Vector2(20, 20)), new Line(new Vector2(30, 0), new Vector2(30, 100)))
            {
                ExpectedResult = (new Vector2(30, 30), true),
                TestName = "LinesAndIntervalIntersectNegativeSlope"
            },

            new TestCaseData(new Line(new Vector2(20, 20), new Vector2(40, 40)), new Line(new Vector2(50, 0), new Vector2(50, 100)))
            {
                ExpectedResult = (Vector2.Zero, false),
                TestName = "LinesIntersectButNotInterval"
            }
        };

        [TestCaseSource(nameof(LineIntersectWithVerticalLineInput))]
        public (Vector2, bool) LineIntersectWithVerticalLineTest(Line line, Line verticalLine) => Helper.LineIntersectWithVerticalLine(line, verticalLine);

        private static readonly TestCaseData[] LineIntersectWithHorizontalLineInput = new[]
       {
            new TestCaseData(new Line(new Vector2(20, 10), new Vector2(20, 50)), new Line(new Vector2(0, 30), new Vector2(100, 30)))
            {
                ExpectedResult = (new Vector2(20, 30), true),
                TestName = "VerticalIntersectsHorizontal"
            }
        };

        [TestCaseSource(nameof(LineIntersectWithHorizontalLineInput))]
        public (Vector2, bool) LineIntersectWithHorizontalLineTest(Line line, Line verticalLine) => Helper.LineIntersectsWithHorizontal(line, verticalLine);

        private static readonly TestCaseData[] Rotate90DegreesCounterClockwiseInput = new[]
        {
            new TestCaseData(new Vector2(1, 0))
            {
                ExpectedResult = new Vector2(0, 1),
                TestName = "RightRotatesUp"
            }
        };

        [TestCaseSource(nameof(Rotate90DegreesCounterClockwiseInput))]
        public Vector2 Rotate90DegreesCounterClockwiseTests(Vector2 point) => Helper.Rotate90DegreesCounterClockwise(point);

        private static readonly TestCaseData[] Rotate90DegreesClockwiseInput = new[]
     {
            new TestCaseData(new Vector2(-1, 0))
            {
                ExpectedResult = new Vector2(0, -1),
                TestName = "LeftRotatesDown"
            }
        };

        [TestCaseSource(nameof(Rotate90DegreesClockwiseInput))]
        public Vector2 Rotate90DegreesClockwiseTests(Vector2 point) => Helper.Rotate90DegreesCounterClockwise(point);

        private static readonly TestCaseData[] RayBoxFirstCollisionInput = new[]
        {
            new TestCaseData(new Line(new Vector2(205, 201), new Vector2(207, 199)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(new Vector2(206, 200), new Vector2(1, 1), Side.Top, true),
                TestName = "RayCollidesTop"
            },

            new TestCaseData(new Line(new Vector2(205, 200), new Vector2(207, 199)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(false),
                TestName = "RayTouchesTop"
            },

            new TestCaseData(new Line(new Vector2(209, 204), new Vector2(211, 206)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(new Vector2(210, 205), Vector2.One, Side.Right, true),
                TestName = "RayCollidesRight"
            },

            new TestCaseData(new Line(new Vector2(210, 504), new Vector2(211, 206)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(false),
                TestName = "RayTouchesRight"
            },

            new TestCaseData(new Line(new Vector2(203, 204), new Vector2(198, 199)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(new Vector2(200, 201), 3 * Vector2.One, Side.Left, true),
                TestName = "RayCollidesLeft"
            },

            new TestCaseData(new Line(new Vector2(200, 201), new Vector2(198, 199)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(false),
                TestName = "RayTouchesLeft"
            },

            new TestCaseData(new Line(new Vector2(205, 205), new Vector2(205, 215)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(new Vector2(205, 210), new Vector2(0, 5), Side.Bottom, true),
                TestName = "RayCollidesBottom"
            },

            new TestCaseData(new Line(new Vector2(205, 210), new Vector2(205, 215)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(false),
                TestName = "RayTouchesBottom"
            },

            new TestCaseData(new Line(new Vector2(205, 195), new Vector2(205, 215)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(new Vector2(205, 200), new Vector2(0, 5), new List<Side>() { Side.Top}, true),
                TestName = "RayPassesThroughTopAndBottom"
            },

            new TestCaseData(new Line(new Vector2(195, 205), new Vector2(215, 205)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(new Vector2(200, 205), new Vector2(5, 0), new List<Side>() { Side.Left}, true),
                TestName = "RayPassesThroughLeftAndRight"
            },

            new TestCaseData(new Line(new Vector2(211, 205), new Vector2(201, 215)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(new Vector2(210, 206), new Vector2(1, 1), new List<Side>() { Side.Right}, true),
                TestName = "RayPassesThroughRightandBottom"
            },

            new TestCaseData(new Line(new Vector2(205, 195), new Vector2(215, 205)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(false),
                TestName = "RayTouchesTopRightCorner"
            },

            new TestCaseData(new Line(new Vector2(195, 215), new Vector2(205, 205)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(new Vector2(200, 210), new Vector2(5, 5), new List<Side>() { Side.Bottom, Side.Left}, true),
                TestName = "RayPassesThroughBottomLeftCorner"
            },

            new TestCaseData(new Line(new Vector2(201, 200), new Vector2(209, 200)), new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new RayCollisionInfo(false),
                TestName = "RayLiesOnTopOfBox"
            },

            new TestCaseData(new Line(new Vector2(180, 314), new Vector2(180, 315)), new Rectangle(135, 270, 45, 45))
            {
                ExpectedResult = new RayCollisionInfo(false),
                TestName = "RayOnTheSideAndHitsCorner"
            }
        };

        [TestCaseSource(nameof(RayBoxFirstCollisionInput))]
        public RayCollisionInfo RayBoxFirstCollisionTests(Line ray, Rectangle box)
        {
            RayCollisionInfo rci = Helper.RayBoxFirstCollision(ray, box);
            return new RayCollisionInfo(rci.Intersection, new Vector2(Math.Abs(rci.Distance.X), Math.Abs(rci.Distance.Y)), rci.Sides, rci.IsIntersecting);
        }

        private static readonly TestCaseData[] GetRayStatingPointsOnBoxInput = new[]
        {
            new TestCaseData(new Rectangle(200, 200, 10, 10))
            {
                ExpectedResult = new List<Vector2>() {new Vector2(200, 200), new Vector2(200, 210), new Vector2(210, 200), new Vector2(210, 210)}
            }
        };

        [TestCaseSource(nameof(GetRayStatingPointsOnBoxInput))]
        public List<Vector2> GetRayStatingPointsOnBoxTests(Rectangle box) => MainGame.GetRayStatingPointsOnBox(box).ToList();
    }
}