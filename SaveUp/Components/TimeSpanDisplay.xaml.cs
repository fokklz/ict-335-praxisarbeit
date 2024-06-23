using SaveUp.Common;

namespace SaveUp.Components;

public partial class TimeSpanDisplay : ContentView
{
    private Timer _updateTimer;

    public string TimeSpan { get; set; }

	public TimeSpanDisplay()
    {
        _updateTimer = new Timer(UpdateCallback, null, Timeout.Infinite, Timeout.Infinite);

		InitializeComponent();

        UpdateTimeSpan();

        SettingsManager.PropertyChanged += SettingsManager_PropertyChanged;
    }

    private void SettingsManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SettingsManager.TimeSpan))
        {
            UpdateTimeSpan();
        } else if (e.PropertyName == nameof(SettingsManager.Language))
        {
            UpdateTimeSpan();
        }
    }

    private void UpdateCallback(object state)
    {
        UpdateTimeSpan();
    }

    private void UpdateTimeSpan()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TimeSpan difference = DateTime.UtcNow - SettingsManager.TimeSpan;
            int updateInterval = 1000;
            if (difference.TotalDays >= 1)
            {
                var value = (int)difference.TotalDays;
                updateInterval = 1000 * 60 * 60 * 24;
                TimeSpanLabel.Text = value == 1 ? $"{value} {Localization.Instance.App_Time_Day}" : $"{value} {Localization.Instance.App_Time_Days}";
            }
            else if (difference.TotalHours >= 1)
            {
                var value = (int)difference.TotalHours;
                updateInterval = 1000 * 60 * 60;
                TimeSpanLabel.Text = value == 1 ? $"{value} {Localization.Instance.App_Time_Hour}" : $"{value} {Localization.Instance.App_Time_Hours}";
            }
            else if (difference.TotalMinutes >= 1)
            {
                var value = (int)difference.TotalMinutes;
                updateInterval = 1000 * 60;
                TimeSpanLabel.Text = value == 1 ? $"{value} {Localization.Instance.App_Time_Minute}" : $"{value} {Localization.Instance.App_Time_Minutes}";
            }
            else
            {
                var value = (int)difference.TotalSeconds;
                updateInterval = 1000;
                TimeSpanLabel.Text = value == 1 ? $"{value} {Localization.Instance.App_Time_Second}" : $"{value} {Localization.Instance.App_Time_Seconds}";
            }

            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _updateTimer.Change(updateInterval, Timeout.Infinite);
        });
    }
}