using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ClickTest
{
    // https://codereview.stackexchange.com/questions/197197/countdown-control-with-arc-animation
    public partial class Countdown : UserControl
    {
        public int Seconds
        {
            get => (int)GetValue(SecondsProperty);
            set => SetValue(SecondsProperty, value);
        }

        public static readonly DependencyProperty SecondsProperty =
            DependencyProperty.Register("Seconds", typeof(int), typeof(Countdown), new PropertyMetadata(0));

        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static new readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(Countdown), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsEnabledChanged)));

        private readonly DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        public Countdown()
        {
            InitializeComponent();
        }

        private static new void IsEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is not Countdown obj) return;

            if (obj.IsEnabled)
            {
                if (obj.Seconds > 0)
                {
                    (obj.FindResource("Animation") as Storyboard).Duration = new Duration(TimeSpan.FromSeconds(obj.Seconds));
                    (obj.FindResource("Animation") as Storyboard).Begin();
                    obj._timer.Start();
                    obj._timer.Tick += TimerTick;
                }
            }

            void TimerTick(object? o, EventArgs e)
            {
                obj.Seconds--;

                if (obj.Seconds == 0)
                {
                    obj._timer.Stop();
                    obj._timer.Tick -= TimerTick;
                    return;
                }
                
                (obj.FindResource("Animation") as Storyboard).Begin();
            }
        }
    }
}
