using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Globalization;
using System.ComponentModel;

namespace WritersBlox.Views
{
    class TimelinePanel : Panel
    {
        #region Declarations

        #endregion
    
        #region Constructor

        public TimelinePanel()
        {
        }

        #endregion

        #region Dependency Properties

        #region StartTime

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        private static FrameworkPropertyMetadata startTimeMetadata = new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(TimelinePanel), startTimeMetadata);

        #endregion

        #region EndTime

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        private static FrameworkPropertyMetadata endTimeMetadata = new FrameworkPropertyMetadata(DateTime.Now.AddYears(5), FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty = DependencyProperty.Register("EndTime", typeof(DateTime), typeof(TimelinePanel), endTimeMetadata);

        #endregion



        public TimeMode EventMode
        {
            get { return (TimeMode)GetValue(EventModeProperty); }
            set { SetValue(EventModeProperty, value); }
        }

        private static FrameworkPropertyMetadata eventModeMetadata = new FrameworkPropertyMetadata(TimeMode.eStartEndTime, FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for EventMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EventModeProperty =
            DependencyProperty.Register("EventMode", typeof(TimeMode), typeof(TimelinePanel), eventModeMetadata);

        

        #region CanvasStartTime

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime CanvasStartTime
        {
            get { return (DateTime)GetValue(CanvasStartTimeProperty); }
            set { SetValue(CanvasStartTimeProperty, value); }
        }

        private static FrameworkPropertyMetadata canvasStartTimeMetadata = new FrameworkPropertyMetadata(DateTime.Now.AddYears(5), FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for CanvasStartTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasStartTimeProperty =
            DependencyProperty.Register("CanvasStartTime", typeof(DateTime), typeof(TimelinePanel), canvasStartTimeMetadata);

        #endregion

        #region CanvasEndTime

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime CanvasEndTime
        {
            get { return (DateTime)GetValue(CanvasEndTimeProperty); }
            set { SetValue(CanvasEndTimeProperty, value); }
        }

        private static FrameworkPropertyMetadata canvasEndTimeMetadata = new FrameworkPropertyMetadata(DateTime.Now.AddYears(5), FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for CanvasEndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasEndTimeProperty =
            DependencyProperty.Register("CanvasEndTime", typeof(DateTime), typeof(TimelinePanel), canvasEndTimeMetadata);
        
        #endregion

        #region SecondsPerPixel

        [TypeConverter(typeof(DoubleConverter))]
        public double SecondsPerPixel
        {
            get { return (double)GetValue(SecondsPerPixelProperty); }
            set 
            { 
                SetValue(SecondsPerPixelProperty, value); 
            }
        }

        // Default to one year in 200 pixles
        private static FrameworkPropertyMetadata secPerPixMetadata = new FrameworkPropertyMetadata(TimelineConstants.c_SecondsPerYear / 200.0, FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for SecondsPerPixel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondsPerPixelProperty = DependencyProperty.Register("SecondsPerPixel", typeof(double), typeof(TimelinePanel), secPerPixMetadata);


        #endregion

        #region RulerPenColor

        [TypeConverter(typeof(ColorConverter))]
        public Color RulerPenColor
        {
            get { return (Color)GetValue(RulerPenColorProperty); }
            set { SetValue(RulerPenColorProperty, value); }
        }

        private static FrameworkPropertyMetadata rulerPenColorMetadata = new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender);

        // Using a DependencyProperty as the backing store for RulerPenColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RulerPenColorProperty =
            DependencyProperty.Register("RulerPenColor", typeof(Color), typeof(TimelinePanel), rulerPenColorMetadata);

        #endregion

        #region GridLines

        [TypeConverter(typeof(BooleanConverter))]
        public Boolean GridlinesVisible
        {
            get { return (Boolean)GetValue(GridlinesVisibleProperty); }
            set { SetValue(GridlinesVisibleProperty, value); }
        }

        private static FrameworkPropertyMetadata gridlinesVisibleMetadata = new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender);

        // Using a DependencyProperty as the backing store for GridlinesVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridlinesVisibleProperty =
            DependencyProperty.Register("GridlinesVisible", typeof(Boolean), typeof(TimelinePanel), gridlinesVisibleMetadata);

        #endregion

        #endregion

        #region Overrides

        protected override Size MeasureOverride(Size availableSource)
        {

            double maxHeight = 0;

            foreach (UIElement child in this.Children)
            {
                if (child != null)
                {
                    // Calculate the size adding the size of the arrows
                    // It is boogus to do this here but MeasureCore (apparently) does not handle
                    // if an element request larger size than constraint. Therefore 
                    // this canvas needs knowledge about it

                    // 

                    double width;

                    if (EventMode == TimeMode.eNoTime)
                    {
                        width = 0;
                    }
                    else if (EventMode == TimeMode.eSingleTime)
                    {
                        width = 2.0 * TimelineConstants.c_TimelineItemArrowDW;
                    }
                    else
                    {
                        width = WfTS(EndTime - StartTime) + 2.0 * TimelineConstants.c_TimelineItemArrowDW;
                    }
                    child.Measure(new Size(width, availableSource.Height));
                    maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
                }
            }

            TimeSpan timeSpan = CanvasEndTime - CanvasStartTime;
            double seconds = timeSpan.TotalSeconds;
            double newWidth = seconds / SecondsPerPixel;

            return new Size(newWidth, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                if (child != null)
                {
                    child.Arrange(new Rect(new Point(PXfT(StartTime) - TimelineConstants.c_TimelineItemArrowDW, finalSize.Height / 2 - child.DesiredSize.Height / 2), child.DesiredSize));
                }
            }
            return finalSize; // Returns the final Arranged size
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {

            drawingContext.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));

            if (GridlinesVisible)
            {
                /*
                // Determine start year (that will start before the panel
                DateTime startYear = new DateTime(StartTime.Year, 1, 1);

                // ...all to find end year
                DateTime endYear = new DateTime(EndTime.AddYears(1).Year, 1, 1);

                Brush rBr = new SolidColorBrush(RulerPenColor);
                Pen rPen = new Pen(rBr, 1);
                Pen mrPen = new Pen(rBr, 2);

                for (DateTime y = startYear; y < endYear; y = y.AddYears(1))
                {
                    int x1 = (int)PXfT(y);
                    drawingContext.DrawLine(mrPen, new Point(x1, 0), new Point(x1, ActualHeight));

                    // Now draw months under the current year
                    for (DateTime m = y; m < y.AddYears(1); m = m.AddMonths(1))
                    {
                        int m1 = (int)PXfT(m);
                        drawingContext.DrawLine(rPen, new Point(m1, 0), new Point(m1, ActualHeight));
                    }
                }
                 * */
            }
        }

        // PixelXFromTime (used often, hence the short name)
        protected double PXfT(DateTime time)
        {
            TimeSpan timeFromStart = time - CanvasStartTime;
            double secondsFromStart = timeFromStart.TotalSeconds;

            double x = secondsFromStart / SecondsPerPixel;

            return x;
        }

        // WidthFromTimeSpan (used often, hence the short name)
        protected double WfTS(TimeSpan timeSpan)
        {
            double seconds = timeSpan.TotalSeconds;

            double w = seconds / SecondsPerPixel;

            return w;
        }

        #endregion
    
    
    }
}
