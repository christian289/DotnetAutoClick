using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClickTest
{
    public class MainVM : BaseVM
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        private int _PointX;
        public int PointX
        {
            get => _PointX;
            set
            {
                if (value == _PointX) return;

                _PointX = value;
                OnPropertyChagned();
            }
        }

        private int _PointY;
        public int PointY
        {
            get => _PointY;
            set
            {
                if (value == _PointY) return;

                _PointY = value;
                OnPropertyChagned();
            }
        }

        private bool _IsRunning;
        public bool IsRunning
        {
            get => _IsRunning;
            set
            {
                if (value == _IsRunning) return;

                _IsRunning = value;
                OnPropertyChagned();
            }
        }

        private int _Countdown;
        public int Countdown
        {
            get => _Countdown;
            set
            {
                if (value == _Countdown) return;

                _Countdown = value;
                OnPropertyChagned();
            }
        }


        private ICommand _StartCommand;
        public ICommand StartCommand { get => _StartCommand; }

        private ICommand _StopCommand;
        public ICommand StopCommand { get => _StopCommand; }

        CancellationTokenSource tokenSource;
        ConcurrentQueue<(int, int)> MessageQueue;
        Timer timer;

        public MainVM()
        {
            _StartCommand = new RelayCommand(StartExecute, StartCanExecute);
            _StopCommand = new RelayCommand(StopExecute, StopCanExecute);
            MessageQueue = new ConcurrentQueue<(int, int)>();
            Countdown = 3;
        }

        public void StartExecute()
        {
            Countdown = 3;
            IsRunning = true;

            timer = new Timer((obj) =>
            {
                MessageQueue.Enqueue((PointX, PointY));
            }, null, Countdown * 1000, 1000);

            tokenSource = new CancellationTokenSource();

            Task taskGetMousePosition = Task.Factory.StartNew(() =>
            {
                while (!tokenSource.IsCancellationRequested)
                {
                    if (GetCursorPos(out POINT lpPoint))
                    {
                        PointX = lpPoint.X;
                        PointY = lpPoint.Y;
                    }
                }
            }, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            Task taskDequeue = Task.Factory.StartNew(() =>
            {
                while (!tokenSource.IsCancellationRequested)
                {
                    if (MessageQueue.TryDequeue(out (int, int) message))
                    {
                        int point_x = message.Item1;
                        int point_y = message.Item2;
                        SetCursorPos(point_x, point_y);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, point_x, point_y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, point_x, point_y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, point_x, point_y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, point_x, point_y, 0, 0);
                    }
                }
            }, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public bool StartCanExecute()
        {
            return !IsRunning;
        }

        public void StopExecute()
        {
            IsRunning = false;
            tokenSource.Cancel();
            timer.Dispose();
        }

        public bool StopCanExecute()
        {
            return IsRunning;
        }
    }
}
