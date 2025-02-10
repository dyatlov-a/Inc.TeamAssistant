using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramPhotoService : IPersonPhotoService
{
    private readonly IPersonRepository _personRepository;
    private readonly TelegramBotClientProvider _botClientProvider;
    private readonly ILogger<TelegramPhotoService> _logger;

    public TelegramPhotoService(
        TelegramBotClientProvider botClientProvider,
        ILogger<TelegramPhotoService> logger,
        IPersonRepository personRepository)
    {
        _botClientProvider = botClientProvider ?? throw new ArgumentNullException(nameof(botClientProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<byte[]?> GetPersonPhoto(long personId, CancellationToken token)
    {
        try
        {
            var botId = await _personRepository.FindBotId(personId, token);
            if (!botId.HasValue)
                return null;
            
            var telegramBotClient = await _botClientProvider.Get(botId.Value, token);
            var userProfilePhotos = await telegramBotClient.GetUserProfilePhotosAsync(
                personId,
                offset: 0,
                limit: 1,
                token);
            var userProfilePhoto = userProfilePhotos.Photos.FirstOrDefault()?.MinBy(p => p.Width * p.Height);
            if (userProfilePhoto is null)
                return null;
            
            var fileInfo = await telegramBotClient.GetFileAsync(userProfilePhoto.FileId, token);
            if (string.IsNullOrWhiteSpace(fileInfo.FilePath))
                return null;
            
            using var stream = new MemoryStream();
            
            await telegramBotClient.DownloadFileAsync(fileInfo.FilePath, stream, token);
            
            return stream.ToArray();
        }
        catch(Exception ex)
        {
            _logger.LogWarning(ex, "Error on sync photo for person {PersonId}", personId);
        }

        return null;
    }
}