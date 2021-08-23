using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace WritersBlox.Views
{
    public class TimelineDataGrid : DataGrid
    {

        #region Declarations

        private DateTime _unZoomedStart;
        private DateTime _unZoomedEnd;
        private bool _zoomedView;

        #endregion

        #region Constructor

        public TimelineDataGrid() : base()
        {
            _zoomedView = false;

            EventManager.RegisterClassHandler(typeof(DataGrid), MouseWheelEvent, new RoutedEventHandler(LocalOnMouseRightButtonDown));

            this.AddHandler(UIElement.MouseWheelEvent, new RoutedEventHandler(MyMouseWheelHandler)); //adds the handler for a click event on the most out 
        }

        #endregion

        #region Properties


        #endregion

        #region Dependency Properties

        #region DisplayStartTime

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime DisplayStartTime
        {
            get { return (DateTime)GetValue(DisplayStartTimeProperty); }
            set { SetValue(DisplayStartTimeProperty, value); }
        }

        private static FrameworkPropertyMetadata displayStartTimeMetadata = new FrameworkPropertyMetadata(DateTime.Now.AddYears(5), FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for DisplayStartTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayStartTimeProperty =
            DependencyProperty.Register("DisplayStartTime", typeof(DateTime), typeof(TimelineDataGrid), displayStartTimeMetadata);

        #endregion

        #region DisplayEndTime

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime DisplayEndTime
        {
            get { return (DateTime)GetValue(DisplayEndTimeProperty); }
            set { SetValue(DisplayEndTimeProperty, value); }
        }

        private static FrameworkPropertyMetadata displayEndTimeMetadata = new FrameworkPropertyMetadata(DateTime.Now.AddYears(5), FrameworkPropertyMetadataOptions.AffectsMeasure);

        // Using a DependencyProperty as the backing store for DisplayEndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayEndTimeProperty =
            DependencyProperty.Register("DisplayEndTime", typeof(DateTime), typeof(TimelineDataGrid), displayEndTimeMetadata);

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
        public static readonly DependencyProperty SecondsPerPixelProperty = DependencyProperty.Register("SecondsPerPixel", typeof(double), typeof(TimelineDataGrid), secPerPixMetadata);


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
            DependencyProperty.Register("RulerPenColor", typeof(Color), typeof(TimelineDataGrid), rulerPenColorMetadata);

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
            DependencyProperty.Register("GridlinesVisible", typeof(Boolean), typeof(TimelineDataGrid), gridlinesVisibleMetadata);

        #endregion

        #endregion

        #region Event Handlers

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            SetupTimeline();

        }


        internal static void LocalOnMouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("LocalOnMouseWheel event handler");
        }

        private void MyMouseWheelHandler(object asd, RoutedEventArgs e)
        {
            Console.WriteLine("MyMouseWheelHandler event handler");
        }
        
        protected override void OnMouseWheel( MouseWheelEventArgs e )
        {
            Console.WriteLine("OnMouseWheel event handler");
        }

        #endregion
        
        #region Metods

        public void AutoSizeColumn()
        {
            ColumnFromDisplayIndex(1).Width = new DataGridLength(0);
            UpdateLayout();
            ColumnFromDisplayIndex(1).Width = DataGridLength.Auto;
        }

        public void Zoom(double x, double factor)
        {
            if (x == double.PositiveInfinity)
            {
                SecondsPerPixel *= factor;
                AutoSizeColumn();
            }
            else
            {
                ScrollViewer scrollView = GetScrollbar(this);

                double hOffset = scrollView.ContentHorizontalOffset;

                double vpX = x - hOffset;

                TimeSpan mouseTime = TSfPx(x);

                // Test if new scroll would make view too big
                double testSPP = SecondsPerPixel * factor;

                TimeSpan displayTimeSpan = DisplayEndTime - DisplayStartTime;
                double testWidth = displayTimeSpan.TotalSeconds / testSPP;

                if ((testWidth > 30000 ) && (!_zoomedView))
                {
                    Console.WriteLine("Reached maxwidth, now decrese time edges. _zoomedView: " + _zoomedView.ToString());
                    _unZoomedStart = DisplayStartTime;
                    _unZoomedEnd = DisplayEndTime;
                    _zoomedView = true;
                }

                if (_zoomedView)
                {
                    // Now, zoom-in is being restricted
                    // Decrease display timespan to match logical width with the new scale

                    double logWidth = WfTS(displayTimeSpan);

                    SecondsPerPixel *= factor;

                    TimeSpan newDisplaySpan = TSfPx(logWidth);

                    // deltaEdgeTimeSpan will be positive for zooming out, i.e. when new DisplayStartTime is erlier
                    TimeSpan deltaEdgeTimeSpan = TimeSpan.FromTicks((displayTimeSpan.Ticks - newDisplaySpan.Ticks) / 2);

                    double newOffset;
                    TimeSpan newMouseTime;

                    // Check if zoom out lead to that we should leave zoomedView
                    if ( (factor > 1) && (DisplayStartTime - deltaEdgeTimeSpan < _unZoomedStart))
                    {
                        Console.WriteLine("Leaving zoomed mode");

                        // Borrow delataEdgeTimeSpan to find differenece between old display start, and current display start.
                        deltaEdgeTimeSpan = _unZoomedStart - DisplayStartTime;

                        SetValue(DisplayStartTimeProperty, _unZoomedStart);
                        SetValue(DisplayEndTimeProperty, _unZoomedEnd);
                        _zoomedView = false;

                        
                        newMouseTime = mouseTime - deltaEdgeTimeSpan;
                        newOffset = WfTS(newMouseTime) - vpX;

                        AutoSizeColumn();

                        scrollView.ScrollToHorizontalOffset(newOffset);

                        return;
                    }
                    
                    
                    SetValue(DisplayStartTimeProperty, DisplayStartTime + deltaEdgeTimeSpan);
                    SetValue(DisplayEndTimeProperty, DisplayEndTime - deltaEdgeTimeSpan);
                    newMouseTime = mouseTime - deltaEdgeTimeSpan;
                    newOffset = WfTS(newMouseTime) - vpX;

                    AutoSizeColumn();

                    scrollView.ScrollToHorizontalOffset(newOffset);

                }
                else
                {

                    SecondsPerPixel *= factor;

                    double newOffset = WfTS(mouseTime) - vpX;

                    AutoSizeColumn();

                    scrollView.ScrollToHorizontalOffset(newOffset);
                }
            }
        }

        public void SetupTimeline()
        {
            // Here Itemssource have been set and we are pretty sure it is a Blox, but we can check each item in the ItemsSource
            if (Items.Count == 0)
            {
                SetValue(VisibilityProperty, Visibility.Hidden);
                return;
            }

            DateTime minTime = DateTime.MaxValue;
            DateTime maxTime = DateTime.MinValue;

            foreach (StoryEvent se in Items)
            {
                if (se.EventMode == TimeMode.eSingleTime)
                {
                    if (se.StartTime < minTime) minTime = se.StartTime;
                    if (se.StartTime.AddDays(1) > maxTime) maxTime = se.StartTime.AddDays(1);
                }
                else if (se.EventMode == TimeMode.eStartEndTime)
                {
                    if (se.StartTime < minTime) minTime = se.StartTime;

                    // Handle special case that start and end is the same!
                    if (se.EndTime == se.StartTime)
                    {
                        if (se.EndTime.AddDays(1) > maxTime) maxTime = se.EndTime.AddDays(1);
                    }
                    else if (se.EndTime > maxTime) maxTime = se.EndTime;
                }
            }

            // Check if there is nothing to show, in which case we hide the Timeline
            if (minTime == DateTime.MaxValue)
            {
                // Hide Timeline
                SetValue(VisibilityProperty, Visibility.Hidden);
                return;
            }

            // Now we know that Timeline has a span, inflate it with 20%
            TimeSpan span = maxTime - minTime;
            long tickSpan = span.Ticks;

            tickSpan = tickSpan / 5;   // 20 %

            // Half before, half after
            long inflate = tickSpan / 2;
            TimeSpan inflateTime = TimeSpan.FromTicks(inflate);

            minTime = minTime - inflateTime;
            maxTime = maxTime + inflateTime;
            SetValue(DisplayStartTimeProperty, minTime);
            SetValue(DisplayEndTimeProperty, maxTime);
            _zoomedView = false;

            // Now find out the scale (SecondsPerPixel) to fill the visible client area
            // of the Timeline. 

            double w = Math.Max(TimelineWidth(), 25);

            //Message2 = w.ToString();

            TimeSpan ts = maxTime - minTime;

            SetValue(SecondsPerPixelProperty, ts.TotalSeconds / w);

            SetValue(VisibilityProperty, Visibility.Visible);

            AutoSizeColumn();

        }


        private static ScrollViewer GetScrollbar(DependencyObject dep) 
        { 
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++) 
            { 
                var child = VisualTreeHelper.GetChild(dep, i); 
                if (child != null && child is ScrollViewer)             
                    return child as ScrollViewer; 
                else { 
                    ScrollViewer sub = GetScrollbar(child); 
                    if (sub != null)                 
                        return sub; 
                } 
            } 
            return null; 
        }


        private static TimelinePanel GetTimelinePanel(DependencyObject dep)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                if (child != null && child is TimelinePanel)
                    return child as TimelinePanel;
                else
                {
                    TimelinePanel sub = GetTimelinePanel(child);
                    if (sub != null)
                        return sub;
                }
            }
            return null;
        }


        // PixelXFromTime (used often, hence the short name)
        protected double PXfT(DateTime time)
        {
            TimeSpan timeFromStart = time - DisplayStartTime;
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

        // TimeSpanFromPixels (used often, hence the short name)
        protected TimeSpan TSfPx( double x)
        {
            return new TimeSpan((long) (x * SecondsPerPixel * 10000000));
        }

        public double TimelineWidth()
        {

            // So, here comes the big hack UpdateLayout is needed to determine the space of all freezed
            // columns which can change whenever. But during this we are not interested in the 
            // TimelinePanel itself and because of performance issues we don't want it to take any time
            // Therefore we set scale to low and then reset it.

            double rememberScale = SecondsPerPixel;

            SecondsPerPixel = 20000000;
            
            UpdateLayout();

            SecondsPerPixel = rememberScale;

            ScrollViewer scrollView = GetScrollbar(this);

            double w2 = GetTimelinePanel(this).ActualWidth;

            // Do I need to compensate for the Scrollbar?
            double w;

            w = ActualWidth - ColumnFromDisplayIndex(0).ActualWidth - 18;

            return w;

        }

        #endregion
    }
}
