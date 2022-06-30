using System.Drawing;

namespace Moon.HabboHotel.Items.Wired.Util
{
    public enum MovementState
    {
        NONE = 0,
        RANDOM = 1,
        LEFT_RIGHT = 2,
        UP_DOWN = 3,
        UP = 4,
        RIGHT = 5,
        DOWN = 6,
        LEFT = 7
    }

    public enum MovementDirection
    {
        UP = 0,
        UP_RIGHT = 1,
        RIGHT = 2,
        DOWN_RIGHT = 3,
        DOWN = 4,
        DOWN_LEFT = 5,
        LEFT = 6,
        UP_LEFT = 7,
        RANDOM = 8,
        NONE = 9
    }

    public enum WhenMovementBlock
    {
        NONE = 0,
        RIGHT45 = 1,
        RIGHT90 = 2,
        LEFT45 = 3,
        LEFT90 = 4,
        TURN_BACK = 5,
        TURN_RANDOM = 6
    }

    public enum RotationState
    {
        NONE = 0,
        CLOCK_WISE = 1,
        COUNTER_CLOCK_WISE = 2,
        RANDOM = 3
    }

    public class Movement
    {
        private static void HandleMovement(ref Point coordinate, MovementState state)
        {
            switch (state)
            {
                case MovementState.DOWN:
                    {
                        coordinate.Y++;
                        break;
                    }

                case MovementState.UP:
                    {
                        coordinate.Y--;
                        break;
                    }

                case MovementState.LEFT:
                    {
                        coordinate.X--;
                        break;
                    }

                case MovementState.RIGHT:
                    {
                        coordinate.X++;
                        break;
                    }
            }
        }

        private static void HandleMovementDir(ref Point coordinate, MovementDirection state)
        {
            switch (state)
            {
                case MovementDirection.DOWN:
                    {
                        coordinate.Y++;
                        break;
                    }

                case MovementDirection.UP:
                    {
                        coordinate.Y--;
                        break;
                    }

                case MovementDirection.LEFT:
                    {
                        coordinate.X--;
                        break;
                    }

                case MovementDirection.RIGHT:
                    {
                        coordinate.X++;
                        break;
                    }

                case MovementDirection.DOWN_RIGHT:
                    {
                        coordinate.X++;
                        coordinate.Y++;
                        break;
                    }

                case MovementDirection.DOWN_LEFT:
                    {
                        coordinate.X--;
                        coordinate.Y++;
                        break;
                    }

                case MovementDirection.UP_RIGHT:
                    {
                        coordinate.X++;
                        coordinate.Y--;
                        break;
                    }

                case MovementDirection.UP_LEFT:
                    {
                        coordinate.X--;
                        coordinate.Y--;
                        break;
                    }
            }
        }

        public static Point HandleMovement(Point newCoordinate, MovementState state, int newRotation)
        {
            var newPoint = new Point(newCoordinate.X, newCoordinate.Y);

            switch (state)
            {
                case MovementState.UP:
                case MovementState.DOWN:
                case MovementState.LEFT:
                case MovementState.RIGHT:
                    {
                        HandleMovement(ref newPoint, state);
                        break;
                    }

                case MovementState.LEFT_RIGHT:
                    {
                        if (MoonEnvironment.GetRandomNumber(0, 2) == 1)
                            HandleMovement(ref newPoint, MovementState.LEFT);
                        else
                            HandleMovement(ref newPoint, MovementState.RIGHT);
                        break;
                    }

                case MovementState.UP_DOWN:
                    {
                        if (MoonEnvironment.GetRandomNumber(0, 2) == 1)
                            HandleMovement(ref newPoint, MovementState.UP);
                        else
                            HandleMovement(ref newPoint, MovementState.DOWN);
                        break;
                    }

                case MovementState.RANDOM:
                    {
                        switch (MoonEnvironment.GetRandomNumber(1, 5))
                        {
                            case 1:
                                {
                                    HandleMovement(ref newPoint, MovementState.UP);
                                    break;
                                }
                            case 2:
                                {
                                    HandleMovement(ref newPoint, MovementState.DOWN);
                                    break;
                                }

                            case 3:
                                {
                                    HandleMovement(ref newPoint, MovementState.LEFT);
                                    break;
                                }
                            case 4:
                                {
                                    HandleMovement(ref newPoint, MovementState.RIGHT);
                                    break;
                                }
                        }
                        break;
                    }
            }

            return newPoint;
        }

        public static Point HandleMovementDir(Point newCoordinate, MovementDirection state, int newRotation)
        {
            var newPoint = new Point(newCoordinate.X, newCoordinate.Y);

            switch (state)
            {
                case MovementDirection.UP:
                case MovementDirection.DOWN:
                case MovementDirection.LEFT:
                case MovementDirection.RIGHT:
                case MovementDirection.DOWN_RIGHT:
                case MovementDirection.DOWN_LEFT:
                case MovementDirection.UP_RIGHT:
                case MovementDirection.UP_LEFT:
                    {
                        HandleMovementDir(ref newPoint, state);
                        break;
                    }

                case MovementDirection.RANDOM:
                    {
                        switch (MoonEnvironment.GetRandomNumber(1, 5))
                        {
                            case 1:
                                {
                                    HandleMovementDir(ref newPoint, MovementDirection.UP);
                                    break;
                                }
                            case 2:
                                {
                                    HandleMovementDir(ref newPoint, MovementDirection.DOWN);
                                    break;
                                }

                            case 3:
                                {
                                    HandleMovementDir(ref newPoint, MovementDirection.LEFT);
                                    break;
                                }
                            case 4:
                                {
                                    HandleMovementDir(ref newPoint, MovementDirection.RIGHT);
                                    break;
                                }
                        }
                        break;
                    }
            }

            return newPoint;
        }

        public static int HandleRotation(int oldRotation, RotationState state)
        {
            var rotation = oldRotation;
            switch (state)
            {
                case RotationState.CLOCK_WISE:
                    {
                        HandleClockwiseRotation(ref rotation);
                        return rotation;
                    }

                case RotationState.COUNTER_CLOCK_WISE:
                    {
                        HandleCounterClockwiseRotation(ref rotation);
                        return rotation;
                    }

                case RotationState.RANDOM:
                    {
                        if (MoonEnvironment.GetRandomNumber(0, 3) == 1)
                            HandleClockwiseRotation(ref rotation);
                        else
                            HandleCounterClockwiseRotation(ref rotation);
                        return rotation;
                    }
            }

            return rotation;
        }

        private static void HandleClockwiseRotation(ref int rotation)
        {
            rotation = rotation + 2;
            if (rotation > 6)
                rotation = 0;
        }

        private static void HandleCounterClockwiseRotation(ref int rotation)
        {
            rotation = rotation - 2;
            if (rotation < 0)
                rotation = 6;
        }
    }
}