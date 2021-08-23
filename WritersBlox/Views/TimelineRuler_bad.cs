using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Globalization;
using System.ComponentModel;

namespace WritersBlox.Views
{
    class TimelineRuler : Adorner
    {
        #region Constants

        const double c_SecondsPerYear = 31536000.0;

        #endregion

        #region Declarations

        private double _secondsPerPixel = c_SecondsPerYear / 400.0; // One year in 200 pixles

        #endregion

        #region Constructor

        public TimelineRuler( UIElement adornerElement) : base( adornerElement )
        {
            SetBinding( StartTimeProperty, "StartTime");
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

        private static FrameworkPropertyMetadata startTimedMetadata = new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("TimelineRuler", typeof(DateTime), typeof(TimelineRuler), startTimedMetadata);

        #endregion

        #region EndTime

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        private static FrameworkPropertyMetadata endTimedMetadata = new FrameworkPropertyMetadata(DateTime.Now.AddYears(1), FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(TimelineRuler), endTimedMetadata);

        #endregion

        #region SecondsPerPixel

        [TypeConverter(typeof(DoubleConverter))]
        public double SecondsPerPixel
        {
            get { return (double)GetValue(SecondsPerPixelProperty); }
            set { SetValue(SecondsPerPixelProperty, value); }
        }

        // Default to one year in 200 pixles
        private static FrameworkPropertyMetadata secPerPixMetadata = new FrameworkPropertyMetadata(c_SecondsPerYear / 200.0, FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for SecondsPerPixel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondsPerPixelProperty = DependencyProperty.Register("SecondsPerPixel", typeof(double), typeof(TimelineRuler), secPerPixMetadata);

        #endregion

        #endregion



        #region Overrides

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            // Determine the number of years in the panel
            double yearsInPanel = (double) ActualWidth * _secondsPerPixel / c_SecondsPerYear;

            // Add one year to draw beyond the panel
            yearsInPanel += 2.0;

            // Determine start year (that will start before the panel
            DateTime startYear = new DateTime(StartTime.Year, 1, 1);

            // ...all to find end year
            DateTime endYear = startYear.AddYears((int)yearsInPanel);

            for (DateTime y = startYear ; y < endYear; y = y.AddYears(1))
            {
                int x1 = (int)PXfT(y);
                int x2 = (int)PXfT(y.AddYears(1));
                drawingContext.DrawLine(new Pen(Brushes.Blue, 1), new Point(x1, 0), new Point(x1, ActualHeight / 2));
                drawingContext.DrawLine(new Pen(Brushes.Blue, 1), new Point(x1, ActualHeight / 2), new Point(x2, ActualHeight / 2));
                FormattedText ty = new FormattedText(y.Year.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 14.0 , Brushes.Blue);
                ty.TextAlignment = TextAlignment.Center;
                ty.MaxTextWidth = x2 - x1;
                drawingContext.DrawText(ty, new Point(x1, 0));

                // Now draw months under the current year
                for (DateTime m = y; m < y.AddYears(1); m = m.AddMonths(1))
                {
                    int m1 = (int)PXfT(m);
                    int m2 = (int)PXfT(m.AddMonths(1));
                    drawingContext.DrawLine(new Pen(Brushes.Blue, 1), new Point(m1, ActualHeight / 2), new Point(m1, ActualHeight));
                    FormattedText tm = new FormattedText(m.Month.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 14.0, Brushes.Blue);
                    tm.TextAlignment = TextAlignment.Center;
                    tm.MaxTextWidth = m2 - m1;
                    drawingContext.DrawText(tm, new Point(m1, ActualHeight / 2));
                }

            }
        }

        // PixelXFromTime (used often, hence the short name)
        protected double PXfT( DateTime time )
        {
            TimeSpan timeFromStart = time - StartTime;
            double secondsFromStart = timeFromStart.TotalSeconds;

            double x = secondsFromStart / _secondsPerPixel;

            return x;
        }

        #endregion
    }
}
