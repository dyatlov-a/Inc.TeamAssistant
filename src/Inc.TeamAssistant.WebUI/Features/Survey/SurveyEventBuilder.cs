using Inc.TeamAssistant.Survey.Model.Commands.SetAnswer;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Inc.TeamAssistant.WebUI.Features.Survey;

internal sealed class SurveyEventBuilder : ISurveyEventProvider, IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private Action? _rerender;
    private Func<Task>? _reload;
    
    public SurveyEventBuilder(NavigationManager navigationManager)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri(HubDescriptors.SurveyHub.Endpoint))
            .WithAutomaticReconnect(HubRetryPolicy.Default)
            .ConfigureLogging(c => c.SetMinimumLevel(LogLevel.Error))
            .Build();

        _hubConnection.Reconnecting += OnReconnecting;
        _hubConnection.Closed += OnReconnecting;
        _hubConnection.Reconnected += OnReconnected;
    }
    
    public HubConnectionState State => _hubConnection.State;

    public SurveyEventBuilder AddAccessors(Action rerender, Func<Task> reload)
    {
        _rerender = rerender ?? throw new ArgumentNullException(nameof(rerender));
        _reload = reload ?? throw new ArgumentNullException(nameof(reload));

        return this;
    }
    
    public async Task<SurveyEventBuilder> Start()
    {
        await _hubConnection.StartAsync();

        return this;
    }
    
    public async Task GiveFacilitator(Guid roomId)
    {
        await _hubConnection.SendAsync(HubDescriptors.SurveyHub.GiveFacilitatorMethod, roomId);
    }
    
    public async Task SetAnswer(SetAnswerCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _hubConnection.SendAsync(HubDescriptors.SurveyHub.SetAnswerMethod, command);
    }
    
    public async Task<IDisposable> Build(
        Guid roomId,
        params IReadOnlyCollection<Func<ISurveyEventProvider, IDisposable>> eventHandlers)
    {
        ArgumentNullException.ThrowIfNull(eventHandlers);

        var handlers = eventHandlers
            .Select(i => i(this))
            .ToArray();

        await _hubConnection.InvokeAsync(HubDescriptors.SurveyHub.JoinSurveyMethod, roomId);

        return new PostActionScope(() =>
        {
            foreach (var handler in handlers)
                handler.Dispose();
        });
    }
    
    IDisposable ISurveyEventProvider.OnFacilitatorChanged(Func<long, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
		
        return _hubConnection.On(nameof(ISurveyHubClient.FacilitatorChanged), changed);
    }
    
    private Task OnReconnecting(Exception? ex)
    {
        _rerender?.Invoke();
        
        return Task.CompletedTask;
    }
    
    private async Task OnReconnected(string? connectionId)
    {
        if (_reload is not null)
            await _reload();
    }

    public async ValueTask DisposeAsync()
    {
        _hubConnection.Reconnecting -= OnReconnecting;
        _hubConnection.Closed -= OnReconnecting;
        _hubConnection.Reconnected -= OnReconnected;

        if (_hubConnection.State == HubConnectionState.Connected)
            await _hubConnection.StopAsync();
        
        await _hubConnection.DisposeAsync();
    }
}