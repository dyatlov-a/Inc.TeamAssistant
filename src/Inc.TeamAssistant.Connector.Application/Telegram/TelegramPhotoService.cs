using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramPhotoService : IPersonPhotoService
{
    private readonly IBotReader _botReader;
    private readonly TelegramBotClientProvider _botClientProvider;
    private readonly ILogger<TelegramPhotoService> _logger;
    private readonly Guid _authBotId;

    public TelegramPhotoService(
        IBotReader botReader,
        TelegramBotClientProvider botClientProvider,
        ILogger<TelegramPhotoService> logger,
        Guid authBotId)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _botClientProvider = botClientProvider ?? throw new ArgumentNullException(nameof(botClientProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authBotId = authBotId;
    }

    public async Task<byte[]> GetPersonPhoto(long personId, CancellationToken token)
    {
        try
        {
            var botId = await _botReader.FindBotId(personId, token) ?? _authBotId;
            var telegramBotClient = await _botClientProvider.Get(botId, token);
            var userProfilePhotos = await telegramBotClient.GetUserProfilePhotos(
                userId: personId,
                offset: 0,
                limit: 1,
                cancellationToken: token);
            var userProfilePhoto = userProfilePhotos.Photos.FirstOrDefault()?.MinBy(p => p.Width * p.Height);
            if (userProfilePhoto is null)
                return [];
            
            var fileInfo = await telegramBotClient.GetFile(
                fileId: userProfilePhoto.FileId,
                cancellationToken: token);
            if (string.IsNullOrWhiteSpace(fileInfo.FilePath))
                return [];
            
            using var stream = new MemoryStream();
            
            await telegramBotClient.DownloadFile(fileInfo.FilePath, stream, token);
            
            return stream.ToArray();
        }
        catch(Exception ex)
        {
            _logger.LogWarning(ex, "Error on sync photo for person {PersonId}", personId);
        }

        return [];
    }
}