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
    class TimelineRuler : Canvas
    {

        #region Constants

        const double c_SecondsPerYear = 31536000.0;

        #endregion

        #region Declarations

        #endregion
    
        #region Constructor

        public TimelineRuler()
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
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(TimelineRuler), startTimeMetadata);

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
        public static readonly DependencyProperty EndTimeProperty = DependencyProperty.Register("EndTime", typeof(DateTime), typeof(TimelineRuler), endTimeMetadata);

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
        private static FrameworkPropertyMetadata secPerPixMetadata = new FrameworkPropertyMetadata(c_SecondsPerYear / 200.0, FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for SecondsPerPixel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondsPerPixelProperty = DependencyProperty.Register("SecondsPerPixel", typeof(double), typeof(TimelineRuler), secPerPixMetadata);
        

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
            DependencyProperty.Register("RulerPenColor", typeof(Color), typeof(TimelineRuler), rulerPenColorMetadata);

        #endregion

        #endregion

        #region Overrides

        protected override Size MeasureOverride(Size availableSource)
        {
            foreach (UIElement child in this.Children)
            {
                if (child != null)
                {
                    child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                }
            }

            TimeSpan timeSpan = EndTime - StartTime;
            double seconds = timeSpan.TotalSeconds;
            double newWidth = seconds / SecondsPerPixel;

            return new Size(newWidth, 0);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            double leastElementWidth = 10;
            double secondsPerMinute = 60;
            double secondsPerHour = 60 * 60;
            double secondsPerDay = 24 * secondsPerHour;
            double secondsPerMonth = 30 * secondsPerDay;
            double secondsPerYears = 365 * secondsPerMonth;

            TimeSpan dispTime = EndTime - StartTime;

            if (SecondsPerPixel < 1 / leastElementWidth)
            {
                Console.WriteLine("Number of elements: " + dispTime.TotalHours);

                MinutesAndSeconds(drawingContext);
                return;
            }

            if (SecondsPerPixel < secondsPerMinute / leastElementWidth)
            {
                Console.WriteLine("Number of elements: " + dispTime.TotalHours);

                HoursAndMinutes(drawingContext);
                return;
            }

            if (SecondsPerPixel < secondsPerHour / leastElementWidth)
            {
                Console.WriteLine("Number of elements: " + dispTime.TotalHours);

                DaysAndHours(drawingContext);
                return;
            }

            if (SecondsPerPixel < secondsPerDay / leastElementWidth)
            {
                Console.WriteLine("Number of elements: " + dispTime.TotalDays);
                MonthsAndDays(drawingContext);
                return;
            }

            if (SecondsPerPixel < secondsPerMonth / leastElementWidth)
            {
                Console.WriteLine("Number of elements: " + dispTime.TotalDays / 30);
                YearsAndMonths(drawingContext);
                return;
            }

            //if (SecondsPerPixel < secondsPerMonth / leastElementWidth)
            {
                Console.WriteLine("Number of elements: " + dispTime.TotalDays / 365);
                DecadesAndYears(drawingContext);
                return;
            }
        }

        protected void DecadesAndYears(System.Windows.Media.DrawingContext drawingContext)
        {
            int decade = StartTime.Year / 10;
            decade = decade * 10;
            DateTime startDecade = new DateTime(decade, 1, 1);

            decade = EndTime.Year / 10;
            decade = decade * 10 + 1;
            DateTime endDecade = new DateTime(decade, 1, 1);

            Brush rBr = new SolidColorBrush(RulerPenColor);
            Pen rPen = new Pen(rBr, 1);
            Pen mrPen = new Pen(rBr, 2);

            for (DateTime d = startDecade; d < endDecade; d = d.AddYears(10))
            {
                int x1 = (int)PXfT(d);
                int x2 = (int)PXfT(d.AddYears(10));
                drawingContext.DrawLine(mrPen, new Point(x1, 0), new Point(x1, ActualHeight / 2));
                drawingContext.DrawLine(rPen, new Point(x1, ActualHeight / 2), new Point(x2, ActualHeight / 2));
                FormattedText ty = new FormattedText(d.Year.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                ty.TextAlignment = TextAlignment.Center;
                ty.MaxTextWidth = x2 - x1;
                drawingContext.DrawText(ty, new Point(x1, 0));

                // Now draw months under the current year
                for (DateTime y = d; y < d.AddYears(10); y = y.AddYears(1))
                {
                    int m1 = (int)PXfT(y);
                    int m2 = (int)PXfT(y.AddYears(1));
                    drawingContext.DrawLine(rPen, new Point(m1, ActualHeight / 2), new Point(m1, ActualHeight));
                    FormattedText tm = new FormattedText((y.Year % 10).ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                    tm.TextAlignment = TextAlignment.Center;
                    if (tm.Width < m2 - m1)
                    {
                        tm.MaxTextWidth = m2 - m1;
                        drawingContext.DrawText(tm, new Point(m1, ActualHeight / 2));
                    }
                }
            }
        }

        protected void YearsAndMonths(System.Windows.Media.DrawingContext drawingContext)
        {
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
                int x2 = (int)PXfT(y.AddYears(1));
                drawingContext.DrawLine(mrPen, new Point(x1, 0), new Point(x1, ActualHeight / 2));
                drawingContext.DrawLine(rPen, new Point(x1, ActualHeight / 2), new Point(x2, ActualHeight / 2));
                FormattedText ty = new FormattedText(y.Year.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                ty.TextAlignment = TextAlignment.Center;
                ty.MaxTextWidth = x2 - x1;
                drawingContext.DrawText(ty, new Point(x1, 0));

                // Now draw months under the current year
                for (DateTime m = y; m < y.AddYears(1); m = m.AddMonths(1))
                {
                    int m1 = (int)PXfT(m);
                    int m2 = (int)PXfT(m.AddMonths(1));
                    drawingContext.DrawLine(rPen, new Point(m1, ActualHeight / 2), new Point(m1, ActualHeight));
                    FormattedText tm = new FormattedText(m.Month.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                    tm.TextAlignment = TextAlignment.Center;
                    if (tm.Width < m2 - m1)
                    {
                        tm.MaxTextWidth = m2 - m1;
                        drawingContext.DrawText(tm, new Point(m1, ActualHeight / 2));
                    }
                }
            }
        }

        protected void MonthsAndDays(System.Windows.Media.DrawingContext drawingContext)
        {
            // Determine start year (that will start before the panel
            DateTime startMonth = new DateTime(StartTime.Year, StartTime.Month, 1);

            DateTime endMonth = new DateTime(EndTime.Year, EndTime.Month, 1);
            endMonth = endMonth.AddMonths(1);

            Brush rBr = new SolidColorBrush(RulerPenColor);
            Pen rPen = new Pen(rBr, 1);
            Pen mrPen = new Pen(rBr, 2);

            for (DateTime m = startMonth; m < endMonth; m = m.AddMonths(1))
            {
                int x1 = (int)PXfT(m);
                int x2 = (int)PXfT(m.AddMonths(1));
                drawingContext.DrawLine(mrPen, new Point(x1, 0), new Point(x1, ActualHeight / 2));
                drawingContext.DrawLine(rPen, new Point(x1, ActualHeight / 2), new Point(x2, ActualHeight / 2));
                FormattedText tm = new FormattedText(m.Month + ", " + m.Year, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                tm.TextAlignment = TextAlignment.Center;
                tm.MaxTextWidth = x2 - x1;
                drawingContext.DrawText(tm, new Point(x1, 0));

                // Now draw months under the current year
                for (DateTime d = m; d < m.AddMonths(1); d = d.AddDays(1))
                {
                    int d1 = (int)PXfT(d);
                    int d2 = (int)PXfT(d.AddDays(1));
                    drawingContext.DrawLine(rPen, new Point(d1, ActualHeight / 2), new Point(d1, ActualHeight));
                    FormattedText td = new FormattedText(d.Day.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                    td.TextAlignment = TextAlignment.Center;
                    if (td.Width < d2 - d1)
                    {
                        td.MaxTextWidth = d2 - d1;
                        drawingContext.DrawText(td, new Point(d1, ActualHeight / 2));
                    }
                }
            }
        }

        protected void DaysAndHours(System.Windows.Media.DrawingContext drawingContext)
        {
            // Determine start year (that will start before the panel
            DateTime startDay = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day);

            // ...all to find end year
            DateTime endDay = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day);
            endDay = endDay.AddDays(1);

            Brush rBr = new SolidColorBrush(RulerPenColor);
            Pen rPen = new Pen(rBr, 1);
            Pen mrPen = new Pen(rBr, 2);

            for (DateTime d = startDay; d < endDay; d = d.AddDays(1))
            {
                int x1 = (int)PXfT(d);
                int x2 = (int)PXfT(d.AddDays(1));
                drawingContext.DrawLine(mrPen, new Point(x1, 0), new Point(x1, ActualHeight / 2));
                drawingContext.DrawLine(rPen, new Point(x1, ActualHeight / 2), new Point(x2, ActualHeight / 2));
                FormattedText tm = new FormattedText(d.Day + ", " + d.Month + ", " + d.Year, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                tm.TextAlignment = TextAlignment.Center;
                tm.MaxTextWidth = x2 - x1;
                drawingContext.DrawText(tm, new Point(x1, 0));

                // Now draw months under the current year
                for (DateTime h = d; h < d.AddDays(1); h = h.AddHours(1))
                {
                    int h1 = (int)PXfT(h);
                    int h2 = (int)PXfT(h.AddHours(1));
                    drawingContext.DrawLine(rPen, new Point(h1, ActualHeight / 2), new Point(h1, ActualHeight));
                    FormattedText td = new FormattedText(h.Hour.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                    td.TextAlignment = TextAlignment.Center;
                    if (td.Width < h2 - h1)
                    {
                        td.MaxTextWidth = h2 - h1;
                        drawingContext.DrawText(td, new Point(h1, ActualHeight / 2));
                    }
                }
            }
        }

        protected void HoursAndMinutes(System.Windows.Media.DrawingContext drawingContext)
        {
            // Determine start year (that will start before the panel
            DateTime startHour = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, StartTime.Hour, 0, 0);

            // ...all to find end year
            DateTime endHour = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, EndTime.Hour, 0, 0);
            endHour = endHour.AddHours(1);

            Brush rBr = new SolidColorBrush(RulerPenColor);
            Pen rPen = new Pen(rBr, 1);
            Pen mrPen = new Pen(rBr, 2);

            for (DateTime d = startHour; d < endHour; d = d.AddHours(1))
            {
                int x1 = (int)PXfT(d);
                int x2 = (int)PXfT(d.AddHours(1));
                drawingContext.DrawLine(mrPen, new Point(x1, 0), new Point(x1, ActualHeight / 2));
                drawingContext.DrawLine(rPen, new Point(x1, ActualHeight / 2), new Point(x2, ActualHeight / 2));
                FormattedText tm = new FormattedText(d.Day + ", " + d.Month + ", " + d.Year + ", " + d.Hour + ":00", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                tm.TextAlignment = TextAlignment.Center;
                tm.MaxTextWidth = x2 - x1;
                drawingContext.DrawText(tm, new Point(x1, 0));

                // Now draw months under the current year
                for (DateTime h = d; h < d.AddHours(1); h = h.AddMinutes(1))
                {
                    int h1 = (int)PXfT(h);
                    int h2 = (int)PXfT(h.AddMinutes(1));
                    drawingContext.DrawLine(rPen, new Point(h1, ActualHeight / 2), new Point(h1, ActualHeight));
                    FormattedText td = new FormattedText(h.Minute.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                    td.TextAlignment = TextAlignment.Center;
                    if (td.Width < h2 - h1)
                    {
                        td.MaxTextWidth = h2 - h1;
                        drawingContext.DrawText(td, new Point(h1, ActualHeight / 2));
                    }
                }
            }
        }

        protected void MinutesAndSeconds(System.Windows.Media.DrawingContext drawingContext)
        {
            // Determine start year (that will start before the panel
            DateTime startMinute = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, StartTime.Hour, StartTime.Minute, 0);

            // ...all to find end year
            DateTime endMinute = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, EndTime.Hour, EndTime.Minute, 0);
            endMinute = endMinute.AddMinutes(1);

            Brush rBr = new SolidColorBrush(RulerPenColor);
            Pen rPen = new Pen(rBr, 1);
            Pen mrPen = new Pen(rBr, 2);

            for (DateTime d = startMinute; d < endMinute; d = d.AddMinutes(1))
            {
                int x1 = (int)PXfT(d);
                int x2 = (int)PXfT(d.AddHours(1));
                drawingContext.DrawLine(mrPen, new Point(x1, 0), new Point(x1, ActualHeight / 2));
                drawingContext.DrawLine(rPen, new Point(x1, ActualHeight / 2), new Point(x2, ActualHeight / 2));
                FormattedText tm = new FormattedText(d.Day + ", " + d.Month + ", " + d.Year + ", " + d.Hour + ":" + d.Minute, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                tm.TextAlignment = TextAlignment.Center;
                tm.MaxTextWidth = x2 - x1;
                drawingContext.DrawText(tm, new Point(x1, 0));

                // Now draw months under the current year
                for (DateTime h = d; h < d.AddMinutes(1); h = h.AddSeconds(1))
                {
                    int h1 = (int)PXfT(h);
                    int h2 = (int)PXfT(h.AddSeconds(1));
                    drawingContext.DrawLine(rPen, new Point(h1, ActualHeight / 2), new Point(h1, ActualHeight));
                    FormattedText td = new FormattedText(h.Second.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10.0, rBr);
                    td.TextAlignment = TextAlignment.Center;
                    if (td.Width < h2 - h1)
                    {
                        td.MaxTextWidth = h2 - h1;
                        drawingContext.DrawText(td, new Point(h1, ActualHeight / 2));
                    }
                }
            }
        }

        // PixelXFromTime (used often, hence the short name)
        protected double PXfT(DateTime time)
        {
            TimeSpan timeFromStart = time - StartTime;
            double secondsFromStart = timeFromStart.TotalSeconds;

            double x = secondsFromStart / SecondsPerPixel;

            return x;
        }

        #endregion
    
    }
}
