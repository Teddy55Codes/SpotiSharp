﻿namespace SpotiSharp.Models;

public class PlaylistListModel
{
    public List<Playlist> PlayLists { get; private set; } = new List<Playlist>();
    
    public PlaylistListModel()
    {
        var userPlaylists = SpotifyAPI.PlayList.GetAllUserPlaylists();
        if (userPlaylists.Items == null) return;
        foreach (var playlist in userPlaylists.Items)
        {
            PlayLists.Add(new Playlist(playlist.Id, playlist.Images[0].Url, playlist.Name, playlist.Tracks.Total));
        }
    }
}