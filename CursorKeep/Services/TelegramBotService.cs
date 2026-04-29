using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CursorKeep.Services
{
    public class TelegramBotService : IDisposable
    {
        private readonly ConfigService _config;
        private readonly Func<bool> _isMoving;
        private readonly Action _onStart;
        private readonly Action _onStop;
        private readonly Action<Action> _uiInvoke;

        private TelegramBotClient? _bot;
        private CancellationTokenSource? _cts;

        public bool IsConnected { get; private set; }

        public TelegramBotService(ConfigService config, Func<bool> isMoving, Action onStart, Action onStop, Action<Action> uiInvoke)
        {
            _config = config;
            _isMoving = isMoving;
            _onStart = onStart;
            _onStop = onStop;
            _uiInvoke = uiInvoke;
        }

        public bool Start(string token)
        {
            Stop();
            try
            {
                _cts = new CancellationTokenSource();
                _bot = new TelegramBotClient(token);
                _bot.StartReceiving(
                    HandleUpdate,
                    HandleError,
                    new ReceiverOptions { AllowedUpdates = [UpdateType.Message] },
                    _cts.Token);
                _ = RegisterCommandsAsync(_cts.Token);
                IsConnected = true;
                return true;
            }
            catch
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
                _bot = null;
                IsConnected = false;
                return false;
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _bot = null;
            IsConnected = false;
        }

        private async Task RegisterCommandsAsync(CancellationToken ct)
        {
            try
            {
                await _bot!.SetMyCommands(
                [
                    new() { Command = "start",  Description = "Start mouse movement" },
                    new() { Command = "stop",   Description = "Stop mouse movement"  },
                    new() { Command = "status", Description = "Check current status" },
                ], cancellationToken: ct);
            }
            catch { }
        }

        private async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            if (update.Message is not { Text: { } text, Chat.Id: var chatId })
                return;

            if (_config.AuthorizedChatId.HasValue && _config.AuthorizedChatId != chatId)
                return;

            if (!_config.AuthorizedChatId.HasValue)
            {
                _config.SaveAuthorizedChatId(chatId);
                await bot.SendMessage(chatId,
                    "👋 You are now authorized to control CursorKeep.\n\nCommands:\n/start – Start mouse movement\n/stop – Stop mouse movement\n/status – Check current status",
                    cancellationToken: ct);
                return;
            }

            var reply = text.Trim().ToLowerInvariant() switch
            {
                "/start" => RunOnUi(_onStart, "✅ Started"),
                "/stop" => RunOnUi(_onStop, "⛔ Stopped"),
                "/status" => _isMoving() ? "▶ Running" : "⏹ Stopped",
                _ => "Commands: /start  /stop  /status"
            };

            await bot.SendMessage(chatId, reply, cancellationToken: ct);
        }

        private string RunOnUi(Action action, string result)
        {
            _uiInvoke(action);
            return result;
        }

        private Task HandleError(ITelegramBotClient bot, Exception ex, HandleErrorSource source, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public static async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                await new TelegramBotClient(token).GetMe();
                return true;
            }
            catch { return false; }
        }

        public void Dispose() => Stop();
    }
}
