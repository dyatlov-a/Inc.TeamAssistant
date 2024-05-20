using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class PhotosService : BackgroundService
{
    private readonly IPhotosRepository _photosRepository;
    private readonly TelegramBotClientProvider _telegramBotClientProvider;
    private readonly ILogger<PhotosService> _logger;

    public PhotosService(
        IPhotosRepository photosRepository,
        TelegramBotClientProvider telegramBotClientProvider,
        ILogger<PhotosService> logger)
    {
        _photosRepository = photosRepository ?? throw new ArgumentNullException(nameof(photosRepository));
        _telegramBotClientProvider =
            telegramBotClientProvider ?? throw new ArgumentNullException(nameof(telegramBotClientProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await SyncPhotos(token);
            
            await Task.Delay(TimeSpan.FromHours(1), token);
        }
    }
    
    private async Task SyncPhotos(CancellationToken token)
    {
        var persons = await _photosRepository.Get(DateTimeOffset.UtcNow.AddDays(-1), token);

        foreach (var person in persons)
        {
            try
            {
                var telegramBotClient = await _telegramBotClientProvider.Get(person.BotId, token);
                var userProfilePhotos = await telegramBotClient.GetUserProfilePhotosAsync(person.PersonId, 0, 1, token);
                var userProfilePhoto = userProfilePhotos.Photos.FirstOrDefault()?.MinBy(p => p.Width * p.Height);
                if (userProfilePhoto is null)
                    continue;
                
                var fileInfo = await telegramBotClient.GetFileAsync(userProfilePhoto.FileId, token);
                var photo = await _photosRepository.Find(person.PersonId, token) ?? new Photo(person.PersonId);
                if (string.IsNullOrWhiteSpace(fileInfo.FilePath))
                    continue;
                    
                using var stream = new MemoryStream();
                        
                await telegramBotClient.DownloadFileAsync(fileInfo.FilePath, stream, token);
                        
                photo.SetData(stream.ToArray());
                        
                await _photosRepository.Upsert(photo, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on sync photo for person {PersonId}", person.PersonId);
            }
        }
    }
}