using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("5843025151:AAHZduqlAoFoNervUbWI7CR9sqMVRrUZ1Lc");

// key de teste 01: "5759172029:AAGr-gL6iTD0EIgrYj-e0-TZUKZF-vSBMMs" (@CharlesHotmart_bot)
// key de teste 02: "5843025151:AAHZduqlAoFoNervUbWI7CR9sqMVRrUZ1Lc" (@CharlesTeste01Bot)

using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    switch (messageText)
    {
        case "/carteira":
            messageText = @"
    CARTEIRA RECOMENDADA

    BTC
    ETH
    ADA
    MATIC

    Compre um pouco todo mês!

    Como comprar? https://www.google.com.br/
    ";
            break;

        case "/saldo":
            messageText = "Seu saldo é de 100 Bitcoins";
            break;

        case "/staking":
            messageText = "Basta comprar a criptomoedas e realizar o staking";
            break;

        default:
            messageText = "";
            break;
    }

    if(!string.IsNullOrEmpty(messageText))
    {
        Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: messageText,
        cancellationToken: cancellationToken);
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}