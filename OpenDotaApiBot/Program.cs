using DotaOpenApiBot.Tools;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static DotaOpenApiBot.EventHandling.MessageHandler;

var botSettings = SettingsLoader.LoadBotSettings();
var botClient = new TelegramBotClient(botSettings.BotToken);
using var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };

botClient.StartReceiving(
	updateHandler: HandleUpdateAsync,
	pollingErrorHandler: HandlePollingErrorAsync,
	receiverOptions: receiverOptions,
	cancellationToken: cts.Token
);

// TextMessageHandler.LoadHandlers(botClient);

async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken) {
	if (update.Type is UpdateType.Message && update.Message!.Chat.Id != botSettings.MasterId) return;
	await (update.Type switch {
		UpdateType.Message => bot.BotOnMessageReceived(update.Message!),
		// UpdateType.InlineQuery => bot.BotOnInLineQueryReceived(update.InlineQuery!),
		// UpdateType.ChosenInlineResult => bot.BotOnChosenInlineResultReceived(update.ChosenInlineResult!),
		// UpdateType.CallbackQuery => bot.BotOnCallbackQueryReceived(update.CallbackQuery!),
		_ => Task.CompletedTask
	});
}

Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken) {
	var errorMessage = exception switch {
		ApiRequestException apiRequestException
			=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
		_ => exception.ToString()
	};

	Console.WriteLine(errorMessage);
	return Task.FromResult(Task.CompletedTask);
}

Console.ReadKey();
