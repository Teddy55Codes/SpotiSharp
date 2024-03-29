﻿using System.Windows.Input;
using SpotifyAPI.Web;
using SpotiSharp.Enums;
using SpotiSharp.Interfaces;
using SpotiSharp.Models;

namespace SpotiSharp.ViewModels.Filters;

public class PlaylistNumberFilterViewModel : BaseFilter, IFilterViewModel
{
    private List<char> _allowedChars = new List<char>
    {
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9'
    };
    
    private TrackFilter _trackFilterName;

    public string FilterName
    {
        get { return _trackFilterName.ToString(); }
        set { SetProperty(ref _trackFilterName, Enum.Parse<TrackFilter>(value)); }
    }
    
    private string _enteredNumber = string.Empty;

    public string EnteredNumber
    {
        get { return _enteredNumber; }
        set
        {
            OnlyNumerics(value);
            SetProperty(ref _enteredNumber, value);
        }
    }

    private string _validationMessage = string.Empty;
    
    public string ValidationMessage
    {
        get { return _validationMessage; }
        set { SetProperty(ref _validationMessage, value); }
    }

    private NumericFilterOption _selectedfilterOption = NumericFilterOption.Equal;

    public NumericFilterOption SelectedFilterOption
    {
        get { return _selectedfilterOption; }
        set { SetProperty(ref _selectedfilterOption, value); }
    }
    
    private List<NumericFilterOption> _filterOptions = Enum.GetValues<NumericFilterOption>().ToList();

    public List<NumericFilterOption> FilterOptions
    {
        get { return _filterOptions; }
        set { SetProperty(ref _filterOptions, value); }
    }

    public PlaylistNumberFilterViewModel(TrackFilter trackFilter)
    {
        RemoveFilterCommand = new Command(RemoveFilter);
        PlaylistCreatorPageModel.Filters.Add(this);
        FilterName = trackFilter.ToString();
    }

    private void OnlyNumerics(string input)
    {
        string result = string.Empty;
        List<char> characters = input.ToList();
        foreach (var character in characters)
        {
            if (_allowedChars.Contains(character)) result += character;
        }

        if (!input.Equals(result))
        {
            ValidationMessage = "Value entered in number field must be a number.\nFilter will be ignored.";
        }
        else
        {
            ValidationMessage = string.Empty;
        }
    }
     
    public async Task<List<FullTrack>> FilterSongs(List<FullTrack> fullTracks, List<TrackAudioFeatures> audioFeatures)
    {
        if (ValidationMessage == string.Empty && EnteredNumber != string.Empty) return await _trackFilterName.GetFilterFunction()(fullTracks, audioFeatures, int.Parse(EnteredNumber), SelectedFilterOption);
        return fullTracks;
    }

    public void RemoveFilter()
    {
        int index = PlaylistCreatorPageModel.Filters.IndexOf(this);
        InvokeEvent(index);
    }
    
    public ICommand RemoveFilterCommand { private set; get; }
}