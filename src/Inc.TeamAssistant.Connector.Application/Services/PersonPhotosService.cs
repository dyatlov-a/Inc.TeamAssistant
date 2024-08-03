using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class PersonPhotosService : IPersonPhotosService
{
    private readonly IPersonRepository _personRepository;
    private readonly TelegramBotClientProvider _telegramBotClientProvider;
    private readonly ILogger<PersonPhotosService> _logger;

    public PersonPhotosService(
        TelegramBotClientProvider telegramBotClientProvider,
        ILogger<PersonPhotosService> logger,
        IPersonRepository personRepository)
    {
        _telegramBotClientProvider =
            telegramBotClientProvider ?? throw new ArgumentNullException(nameof(telegramBotClientProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<byte[]?> GetPersonPhoto(long personId, CancellationToken token)
    {
        try
        {
            var telegramBotId = await _personRepository.GetBotId(personId, token);
            var telegramBotClient = await _telegramBotClientProvider.Get(telegramBotId, token);
            var userProfilePhotos = await telegramBotClient.GetUserProfilePhotosAsync(personId, 0, 1, token);
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
        catch(Exception e)
        {
            _logger.LogError(e, "Error on sync photo for person {personId}", personId);
        }

        return null;
    }
}