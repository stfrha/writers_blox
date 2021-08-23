using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WritersBlox.Views
{
    class TimelineItemPanel : Panel
    {
       #region Constructor

        public TimelineItemPanel()
        {
        }

        #endregion

        #region Overrides

        protected override Size MeasureOverride(Size availableSource)
        {
            double maxHeight = 0;

            foreach (UIElement child in this.Children)
            {
                if (child != null)
                {
                    double clientWidth = availableSource.Width - 2.0 * TimelineConstants.c_TimelineItemArrowDW;

                    if (clientWidth <= 0)
                    {
                        child.Measure(new Size(0, availableSource.Height));
                    }
                    else
                    {
                        child.Measure(new Size(clientWidth, availableSource.Height));
                    }

                    maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
                }
            }

            maxHeight = Math.Max(maxHeight, TimelineConstants.c_TimelineItemArrowDH);

            return new Size(availableSource.Width, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                if (child != null)
                {
                    // Arrange the child (there should only be one) to offset for the arrow 

                    double y = ActualHeight / 2 - child.DesiredSize.Height / 2;
                    double clientWidth = finalSize.Width - 2.0 * TimelineConstants.c_TimelineItemArrowDW;

                    if (clientWidth <= 0)
                    {
                        child.Arrange(new Rect(new Point(TimelineConstants.c_TimelineItemArrowDW, y), new Size(0, child.DesiredSize.Height)));
                    }
                    else
                    {
                        child.Arrange(new Rect(new Point(TimelineConstants.c_TimelineItemArrowDW, y), new Size(clientWidth, child.DesiredSize.Height)));
                    }

                }
            }
            return finalSize; // Returns the final Arranged size
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Color darkFrame = Color.FromArgb(255, 112, 112, 112);
            Color lightFrame = Color.FromArgb(255, 254, 254, 254);
            Color lightFill = Color.FromArgb(255, 239, 239, 239);
            Color darkFill = Color.FromArgb(255, 212, 212, 212);
            Brush bDarkFrame = new SolidColorBrush(darkFrame);
            Brush bLightFrame = new SolidColorBrush(lightFrame);


            //                   P1
            //                  / |
            //                 /  |
            //                /   |
            //               /    |
            //              /     |        Right
            //             /      |        Arrow Tip
            //            P2     P4        (P2)
            //             \      |
            //              \     |
            //               \    |
            //                \   |
            //                 \  |
            //                  \ |
            //                   P3

            // Dont render if slot is too small
            if (ActualWidth >= 2 * TimelineConstants.c_TimelineItemArrowDW)
            {
                // Use multiple sections to reuse variable names
                // Fist left arrow
                {
                    Point p1 = new Point(TimelineConstants.c_TimelineItemArrowDW,
                        ActualHeight / 2 - TimelineConstants.c_TimelineItemArrowDH / 2);

                    Point p2 = new Point(0, ActualHeight / 2);

                    Point p3 = new Point(p1.X, ActualHeight / 2 + TimelineConstants.c_TimelineItemArrowDH / 2);

                    Point p4 = new Point(p1.X, p2.Y);

                    Triangle(drawingContext, lightFill, p1, p2, p3);
                    Triangle(drawingContext, darkFill, p2, p3, p4);
                    drawingContext.DrawLine(new Pen(bLightFrame, 1), new Point(p1.X - 1, p1.Y + 1), new Point(p2.X + 1, p2.Y));
                    drawingContext.DrawLine(new Pen(bLightFrame, 1), new Point(p2.X + 1, p2.Y), new Point(p3.X - 1, p3.Y - 1));
                    drawingContext.DrawLine(new Pen(bLightFrame, 1), new Point(p3.X - 1, p3.Y - 1), new Point(p1.X - 1, p1.Y + 1));
                    drawingContext.DrawLine(new Pen(bDarkFrame, 1), p1, p2);
                    drawingContext.DrawLine(new Pen(bDarkFrame, 1), p2, p3);
                    drawingContext.DrawLine(new Pen(bDarkFrame, 1), p3, p1);
                }

                // And then right arrow
                {
                    Point p1 = new Point(ActualWidth - TimelineConstants.c_TimelineItemArrowDW,
                        ActualHeight / 2 - TimelineConstants.c_TimelineItemArrowDH / 2);

                    Point p2 = new Point(ActualWidth, ActualHeight / 2);

                    Point p3 = new Point(p1.X, ActualHeight / 2 + TimelineConstants.c_TimelineItemArrowDH / 2);

                    Point p4 = new Point(p1.X, p2.Y);

                    Triangle(drawingContext, lightFill, p1, p2, p3);
                    Triangle(drawingContext, darkFill, p2, p3, p4);
                    drawingContext.DrawLine(new Pen(bLightFrame, 1), new Point(p1.X + 1, p1.Y + 1), new Point(p2.X - 1, p2.Y));
                    drawingContext.DrawLine(new Pen(bLightFrame, 1), new Point(p2.X - 1, p2.Y), new Point(p3.X + 1, p3.Y - 1));
                    drawingContext.DrawLine(new Pen(bLightFrame, 1), new Point(p3.X + 1, p3.Y - 1), new Point(p1.X + 1, p1.Y + 1));
                    drawingContext.DrawLine(new Pen(bDarkFrame, 1), p1, p2);
                    drawingContext.DrawLine(new Pen(bDarkFrame, 1), p2, p3);
                    drawingContext.DrawLine(new Pen(bDarkFrame, 1), p3, p1);
                }

            }
        }
        #endregion

        void Triangle(System.Windows.Media.DrawingContext drawingContext, Color col, Point p1, Point p2, Point p3)
        {
            Brush rBr = new SolidColorBrush(col);
            Pen rPen = new Pen(rBr, 1);
            Pen mrPen = new Pen(rBr, 2);

            PathFigure pf = new PathFigure();
            pf.StartPoint = p1; // new Point(ActualWidth, ActualHeight / 2);

            LineSegment top = new LineSegment();
            top.Point = p2; //  new Point(ActualWidth - TimelineConstants.c_TimelineItemArrowDW, ActualHeight / 2 - TimelineConstants.c_TimelineItemArrowDH / 2);

            LineSegment bot = new LineSegment();
            bot.Point = p3;  //  new Point(ActualWidth - TimelineConstants.c_TimelineItemArrowDW, ActualHeight / 2 + TimelineConstants.c_TimelineItemArrowDH / 2);

            PathSegmentCollection paths = new PathSegmentCollection();
            paths.Add(top);
            paths.Add(bot);

            pf.Segments = paths;

            PathFigureCollection figures = new PathFigureCollection();
            figures.Add(pf);

            PathGeometry myGeo = new PathGeometry();
            myGeo.Figures = figures;

            drawingContext.DrawGeometry(rBr, rPen, myGeo);
        }
    }
}
