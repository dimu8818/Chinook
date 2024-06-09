using Chinook.ClientModels;

namespace Chinook.Services.Interfaces {
    public interface IPlaylistService {
        Task LoadPlaylists(string userId);

        List<Playlist> Playlists { get; }

        event Action OnChange;
    }
}
