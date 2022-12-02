﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SpotifyAPI;

namespace SpotiSharp.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public MainPageViewModel()
    {
        ConnectToSpotifyAPI = new Command(ConnectToSpotifyAPIFunc);
        SongBack = new Command(SongBackFunc);
        SongNext = new Command(SongNextFunc);
        SongPausePlay = new Command(SongPausePlayFunc);
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Object.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void ConnectToSpotifyAPIFunc()
    {
        Authentication.Authenticate();
    }

    private async void SongBackFunc()
    {
        await Authentication.SpotifyClient.Player.SkipPrevious();
    }
    
    private async void SongNextFunc()
    {
        await Authentication.SpotifyClient.Player.SkipNext();
    }
    
    private async void SongPausePlayFunc()
    {
        var playContext = await Authentication.SpotifyClient.Player.GetCurrentPlayback();
        if (playContext == null || playContext.IsPlaying == false)
        {
            await Authentication.SpotifyClient.Player.ResumePlayback();
        }
        else
        {
            await Authentication.SpotifyClient.Player.PausePlayback();
        }
    }
    
    public ICommand ConnectToSpotifyAPI { private set; get; }
    public ICommand SongBack { private set; get; }
    public ICommand SongNext { private set; get; }
    public ICommand SongPausePlay { private set; get; }

    
}