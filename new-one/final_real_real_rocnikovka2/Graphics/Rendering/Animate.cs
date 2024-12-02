using final_real_real_rocnikovka2.Graphics.Objects;
using final_real_real_rocnikovka2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace final_real_real_rocnikovka2.Graphics.Rendering
{
    public static class Animate
    {
        private static TimeSpan AnimationTime => TimeSpan.FromMilliseconds(Globals.AnimationMs);
        private static readonly Storyboard AnimationStoryboard;
        private static bool isAnimationOngoing;
        public static bool IsAnimationOngoing => isAnimationOngoing;
        private static Window currentWindow;

        static Animate()
        {
            AnimationStoryboard = new Storyboard();
            AnimationStoryboard.CurrentStateInvalidated += (s, e) =>
            {
                if (s is Clock clock)
                {
                    isAnimationOngoing = clock.CurrentState == ClockState.Active;
                    UpdateResizeMode(isAnimationOngoing);
                }
            };
        }

        private static void UpdateResizeMode(bool disableResizing)
        {
            if (currentWindow == null) return;

            currentWindow.ResizeMode = disableResizing ? ResizeMode.NoResize : ResizeMode.CanResize;
        }
        public static void SetWindow(Window window)
        {
            currentWindow = window;
        }

        public static void AnimationRun()
        {
            AnimationStoryboard.Begin();
        }

        public static void AnimationClear()
        {
            AnimationStoryboard.Children.Clear();
        }
        public static async Task AnimationSkip()
        {
            AnimationStoryboard.SkipToFill();
            await Task.Delay(1);
        }

        public static void ScheduleForDeletion(GraphicElement gE)
        {
            AnimationStoryboard.Completed += (s, e) =>
            {
                gE.Delete();

            };
        }

        public static TimeSpan GetStoryboardDuration()
        {
            TimeSpan maxDuration = TimeSpan.Zero;

            foreach (var timeline in AnimationStoryboard.Children)
            {
                TimeSpan? duration = timeline.Duration.HasTimeSpan ? timeline.Duration.TimeSpan : (TimeSpan?)null;

                if (duration.HasValue)
                {
                    TimeSpan beginTime = timeline.BeginTime ?? TimeSpan.Zero;
                    TimeSpan endTime = beginTime + duration.Value;

                    if (endTime > maxDuration)
                    {
                        maxDuration = endTime;
                    }
                }
            }

            return maxDuration;
        }

        public static void BallStrokeColorChange(Ball ball, Color endColor, double duration, double beginTime)
        {
            ColorAnimation colorAnimation = CreateColorAnimation(
                gE:             ball,
                from:           ((SolidColorBrush)((Ellipse)ball.MainUIElement).Stroke).Color,
                to:             endColor,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath("Stroke.Color"));

            AnimationStoryboard.Children.Add(colorAnimation);
        }

      
        public static void BallFillColorChange(Ball ball, Color endColor, double duration, double beginTime)
        {
            ColorAnimation colorAnimation = CreateColorAnimation(
                gE:             ball,
                from:           ((SolidColorBrush)((Ellipse)ball.MainUIElement).Fill).Color,
                to:             endColor,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath("Fill.Color"));

            AnimationStoryboard.Children.Add(colorAnimation);
        }


        public static void TextColorChange(Text text, Color endColor, double duration, double beginTime)
        {
            ColorAnimation colorAnimation = CreateColorAnimation(
                gE:             text,
                from:           ((SolidColorBrush)((TextBlock)text.MainUIElement).Foreground).Color,
                to:             endColor,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath("Foreground.Color"));

            AnimationStoryboard.Children.Add(colorAnimation);
        }

        public static void OpacityChange(GraphicElement gE, double endOpacity, double duration, double beginTime)
        {
            DoubleAnimation doubleAnimation = CreateDoubleAnimation(
                gE:             gE, 
                from:           gE.MainUIElement.Opacity,
                to:             endOpacity, 
                duration:       duration, 
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(UIElement.OpacityProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            AnimationStoryboard.Children.Add(doubleAnimation);

        }

        public static void MoveBallWithText(Ball ball, double endX, double endY, double duration, double beginTime)
        {
            DoubleAnimation ballVerticalMovement = CreateDoubleAnimation(
                gE:             ball,
                from:           ball.Y,
                to:             endY,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            DoubleAnimation ballHorizontalMovement = CreateDoubleAnimation(
                gE:             ball,
                from:           ball.X,
                to:             endX,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.LeftProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            DoubleAnimation textVerticalMovement = CreateDoubleAnimation(
                gE:             ball.BallText,
                from:           ball.BallText.Y,
                to:             endY + Draw.BallRadius - ball.BallText.TextHeight/2,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            DoubleAnimation textHorizontalMovement = CreateDoubleAnimation(
                gE:             ball.BallText,
                from:           ball.BallText.X,
                to:             endX + Draw.BallRadius - ball.BallText.TextWidth/2,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.LeftProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            AnimationStoryboard.Children.Add(ballVerticalMovement);
            AnimationStoryboard.Children.Add(ballHorizontalMovement);
            AnimationStoryboard.Children.Add(textVerticalMovement);
            AnimationStoryboard.Children.Add(textHorizontalMovement);
        }


        public static void BallSwap(Ball ballA, Ball ballB, double duration, double beginTime, double arcHeight)
        {
            /////////////////// BALL ///////////////////

            DoubleAnimation ballAVerticalMovementUp = CreateDoubleAnimation(
                gE:             ballA,
                from:           ballA.Y,
                to:             ballA.Y - arcHeight * Draw.BallRadius,
                duration:       duration/2,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseOut });

            DoubleAnimation ballBVerticalMovementDown = CreateDoubleAnimation(
                gE:             ballB,
                from:           ballB.Y,
                to:             ballB.Y + arcHeight * Draw.BallRadius,
                duration:       duration / 2,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseOut });

            DoubleAnimation ballAHorizontalMovement = CreateDoubleAnimation(
                gE:             ballA,
                from:           ballA.X,
                to:             ballB.X,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.LeftProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            DoubleAnimation ballBHorizontalMovement = CreateDoubleAnimation(
                gE:             ballB,
                from:           ballB.X,
                to:             ballA.X,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.LeftProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            DoubleAnimation ballAVerticalMovementDown = CreateDoubleAnimation(
                gE:             ballA,
                from:           ballA.Y - arcHeight * Draw.BallRadius,
                to:             ballA.Y,
                duration:       duration / 2,
                beginTime:      beginTime + duration / 2,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseIn });

            DoubleAnimation ballBVerticalMovementUp = CreateDoubleAnimation(
                gE:             ballB,
                from:           ballB.Y + arcHeight * Draw.BallRadius,
                to:             ballB.Y,
                duration:       duration / 2,
                beginTime:      beginTime + duration / 2,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseIn });

            /////////////////// TEXT ///////////////////

            DoubleAnimation textAVerticalMovementUp = CreateDoubleAnimation(
                gE:             ballA.BallText,
                from:           ballA.BallText.Y,
                to:             ballA.BallText.Y - arcHeight * Draw.BallRadius,
                duration:       duration / 2,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseOut });

            DoubleAnimation textBVerticalMovementDown = CreateDoubleAnimation(
                gE:             ballB.BallText,
                from:           ballB.BallText.Y,
                to:             ballB.BallText.Y + arcHeight * Draw.BallRadius,
                duration:       duration / 2,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseOut });

            DoubleAnimation textAHorizontalMovement = CreateDoubleAnimation(
                gE:             ballA.BallText,
                from:           ballA.BallText.X,
                to:             ballB.X + Draw.BallRadius - ballA.BallText.TextWidth/2,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.LeftProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            DoubleAnimation textBHorizontalMovement = CreateDoubleAnimation(
                gE:             ballB.BallText,
                from:           ballB.BallText.X,
                to:             ballA.X + Draw.BallRadius - ballB.BallText.TextWidth / 2,
                duration:       duration,
                beginTime:      beginTime,
                propertyPath:   new PropertyPath(Canvas.LeftProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseInOut });

            DoubleAnimation textAVerticalMovementDown = CreateDoubleAnimation(
                gE:             ballA.BallText,
                from:           ballA.BallText.Y - arcHeight * Draw.BallRadius,
                to:             ballA.BallText.Y,
                duration:       duration / 2,
                beginTime:      beginTime + duration / 2,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseIn });

            DoubleAnimation textBVerticalMovementUp = CreateDoubleAnimation(
                gE:             ballB.BallText,
                from:           ballB.BallText.Y + arcHeight * Draw.BallRadius,
                to:             ballB.BallText.Y,
                duration:       duration / 2,
                beginTime:      beginTime + duration / 2,
                propertyPath:   new PropertyPath(Canvas.TopProperty),
                easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseIn });

            /////////////////// BALL ///////////////////
            AnimationStoryboard.Children.Add(ballAVerticalMovementUp);
            AnimationStoryboard.Children.Add(ballBVerticalMovementDown);
            AnimationStoryboard.Children.Add(ballAHorizontalMovement);
            AnimationStoryboard.Children.Add(ballBHorizontalMovement);
            AnimationStoryboard.Children.Add(ballAVerticalMovementDown);
            AnimationStoryboard.Children.Add(ballBVerticalMovementUp);

            /////////////////// TEXT ///////////////////
            AnimationStoryboard.Children.Add(textAVerticalMovementUp);
            AnimationStoryboard.Children.Add(textBVerticalMovementDown);
            AnimationStoryboard.Children.Add(textAHorizontalMovement);
            AnimationStoryboard.Children.Add(textBHorizontalMovement);
            AnimationStoryboard.Children.Add(textAVerticalMovementDown);
            AnimationStoryboard.Children.Add(textBVerticalMovementUp);

            AnimationStoryboard.Completed += (s, e) =>  // provizorni reseni protoze nevim jak tohle udelat pomoci iterace pro kazdy pri animationrun :(
            {
                ballA.MainUIElement.BeginAnimation(Canvas.TopProperty, null);
                ballB.MainUIElement.BeginAnimation(Canvas.TopProperty, null);
                ballA.MainUIElement.BeginAnimation(Canvas.LeftProperty, null);
                ballB.MainUIElement.BeginAnimation(Canvas.LeftProperty, null);
                ballA.MainUIElement.BeginAnimation(Canvas.TopProperty, null);
                ballB.MainUIElement.BeginAnimation(Canvas.TopProperty, null);

                ballA.BallText.MainUIElement.BeginAnimation(Canvas.TopProperty, null);
                ballB.BallText.MainUIElement.BeginAnimation(Canvas.TopProperty, null);
                ballA.BallText.MainUIElement.BeginAnimation(Canvas.LeftProperty, null);
                ballB.BallText.MainUIElement.BeginAnimation(Canvas.LeftProperty, null);
                ballA.BallText.MainUIElement.BeginAnimation(Canvas.TopProperty, null);
                ballB.BallText.MainUIElement.BeginAnimation(Canvas.TopProperty, null);

                AnimationStoryboard.Completed -= (s, e) => { };
            };
        }



        private static ColorAnimation CreateColorAnimation(GraphicElement gE, Color from, Color to, double duration, double beginTime, PropertyPath propertyPath)
        {
            ColorAnimation colorAnimation = new ColorAnimation();

            colorAnimation.From = from;
            colorAnimation.To = to;
            colorAnimation.Duration = AnimationTime * duration;
            colorAnimation.BeginTime = AnimationTime * beginTime;
            colorAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

            Storyboard.SetTarget(colorAnimation, gE.MainUIElement);
            Storyboard.SetTargetProperty(colorAnimation, propertyPath);

            return colorAnimation;
        }

     
        private static DoubleAnimation CreateDoubleAnimation(GraphicElement gE, double from, double to, double duration, double beginTime, PropertyPath propertyPath, IEasingFunction easingFunction)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = from;
            doubleAnimation.To = to;
            doubleAnimation.Duration = AnimationTime * duration;
            doubleAnimation.BeginTime = AnimationTime * beginTime;
            doubleAnimation.EasingFunction = easingFunction;

            Storyboard.SetTarget(doubleAnimation, gE.MainUIElement);
            Storyboard.SetTargetProperty(doubleAnimation, propertyPath);

            return doubleAnimation;
        }

    }
}
