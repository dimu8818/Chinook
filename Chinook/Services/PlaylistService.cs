using Chinook.ClientModels;
using Chinook.Exceptions;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Chinook.Services {
    public class PlaylistService : IPlaylistService {
        private readonly IDbContextFactory<ChinookContext> _dbContextFactory;

        public List<Playlist> Playlists { get; private set; } = new List<Playlist>();

        public event Action OnChange;

        public PlaylistService(IDbContextFactory<ChinookContext> dbContextFactory) {
            _dbContextFactory = dbContextFactory;
        }

        public async Task LoadPlaylists(string userId) {
            try {
                using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                Playlists = await dbContext.Playlists
                    .Where(p => p.UserPlaylists.Any(up => up.UserId == userId))
                    .Select(p => new Playlist {
                        PlaylistId = p.PlaylistId,
                        Name = p.Name
                    })
                    .ToListAsync();
            }
            catch (DbException ex) {
                throw new PlaylistServiceException("An error occurred while accessing the database.", ex);
            }
            catch (Exception ex) {
                throw new PlaylistServiceException("An unexpected error occurred.", ex);
            }

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
