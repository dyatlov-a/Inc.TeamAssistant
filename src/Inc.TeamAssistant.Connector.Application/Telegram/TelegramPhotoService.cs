using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramPhotoService : IPersonPhotoService
{
    private readonly IBotReader _botReader;
    private readonly TelegramBotClientProvider _botClientProvider;
    private readonly ILogger<TelegramPhotoService> _logger;

    public TelegramPhotoService(
        TelegramBotClientProvider botClientProvider,
        ILogger<TelegramPhotoService> logger,
        IBotReader botReader)
    {
        _botClientProvider = botClientProvider ?? throw new ArgumentNullException(nameof(botClientProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
    }

    public async Task<byte[]?> GetPersonPhoto(long personId, CancellationToken token)
    {
        try
        {
            var botId = await _botReader.FindBotId(personId, token);
            if (!botId.HasValue)
                return null;
            
            var telegramBotClient = await _botClientProvider.Get(botId.Value, token);
            var userProfilePhotos = await telegramBotClient.GetUserProfilePhotos(
                userId: personId,
                offset: 0,
                limit: 1,
                cancellationToken: token);
            var userProfilePhoto = userProfilePhotos.Photos.FirstOrDefault()?.MinBy(p => p.Width * p.Height);
            if (userProfilePhoto is null)
                return null;
            
            var fileInfo = await telegramBotClient.GetFile(
                fileId: userProfilePhoto.FileId,
                cancellationToken: token);
            if (string.IsNullOrWhiteSpace(fileInfo.FilePath))
                return null;
            
            using var stream = new MemoryStream();
            
            await telegramBotClient.DownloadFile(fileInfo.FilePath, stream, token);
            
            return stream.ToArray();
        }
        catch(Exception ex)
        {
            _logger.LogWarning(ex, "Error on sync photo for person {PersonId}", personId);
        }

        return null;
    }
}