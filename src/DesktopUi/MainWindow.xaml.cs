using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace DesktopUi;

public partial class MainWindow
{
    private bool _isPlaying;
    private bool _isSliderDragging;

    public MainWindow()
    {
        InitializeComponent();

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100),
        };

        timer.Tick += timer_Tick!;
        timer.Start();
    }

    private void timer_Tick(object sender, EventArgs e)
    {
        if (Player.Source == null || !Player.NaturalDuration.HasTimeSpan || _isSliderDragging) return;
        Slider.Minimum = 0;
        Slider.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
        Slider.Value = Player.Position.TotalSeconds;
    }

    private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter =
                "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg;*.mp4;*.avi;*.wav;*.gif;*.jpg|All files (*.*)|*.*",
        };

        if (openFileDialog.ShowDialog() == true) Player.Source = new Uri(openFileDialog.FileName);
    }

    private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Player != null && Player.Source != null;
    }

    private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Player.Play();
        _isPlaying = true;
    }

    private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _isPlaying;
    }

    private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Player.Pause();
    }

    private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _isPlaying;
    }

    private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Player.Stop();
        _isPlaying = false;
    }

    private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
    {
        _isSliderDragging = true;
        Player.Pause();
    }

    private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _isSliderDragging = false;
        Player.Position = TimeSpan.FromSeconds(Slider.Value);
        Player.Play();
    }

    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ProgressStatus.Text = TimeSpan.FromSeconds(Slider.Value).ToString(@"hh\:mm\:ss");
    }

    private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        Player.Volume += e.Delta > 0 ? 0.1 : -0.1;
    }
}
