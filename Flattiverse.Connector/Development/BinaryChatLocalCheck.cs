using System.Collections.Concurrent;
using System.Diagnostics;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Development;

partial class Program
{
    private static async Task RunBinaryChatCheckLocal()
    {
        const int SetupDelayMs = 250;
        const int EventTimeoutMs = 5000;

        string adminAuth = Environment.GetEnvironmentVariable("FV_BINARY_CHAT_ADMIN_AUTH") ?? LocalSwitchGateAdminAuth;
        string pinkAuth = Environment.GetEnvironmentVariable("FV_BINARY_CHAT_PINK_AUTH") ?? LocalSwitchGatePinkPlayerAuth;
        string greenAuth = Environment.GetEnvironmentVariable("FV_BINARY_CHAT_GREEN_AUTH") ?? LocalSwitchGatePlayerAuth;

        Process? galaxyProcess = null;
        Galaxy? adminGalaxy = null;
        Galaxy? pinkGalaxy = null;
        Galaxy? greenGalaxy = null;
        Task? pinkEventPump = null;
        Task? greenEventPump = null;

        ConcurrentQueue<FlattiverseEvent> pinkEvents = new ConcurrentQueue<FlattiverseEvent>();
        ConcurrentQueue<FlattiverseEvent> greenEvents = new ConcurrentQueue<FlattiverseEvent>();
        List<FlattiverseEvent> pinkTrace = new List<FlattiverseEvent>();
        List<FlattiverseEvent> greenTrace = new List<FlattiverseEvent>();

        await EnsureSessionCleared(adminAuth, "BINARY-LOCAL:admin").ConfigureAwait(false);
        await EnsureSessionCleared(pinkAuth, "BINARY-LOCAL:pink").ConfigureAwait(false);
        await EnsureSessionCleared(greenAuth, "BINARY-LOCAL:green").ConfigureAwait(false);

        try
        {
            Console.WriteLine("BINARY-LOCAL: starting local galaxy 666...");
            galaxyProcess = StartLocalGalaxyProcess();
            (adminGalaxy, _) = await ConnectLocalAdminAfterInitialRebuild(galaxyProcess, adminAuth).ConfigureAwait(false);

            adminGalaxy.Dispose();
            adminGalaxy = null;
            await WaitForSessionGalaxy(adminAuth, null, 7000).ConfigureAwait(false);

            Console.WriteLine("BINARY-LOCAL: connecting pink player...");
            pinkGalaxy = await ConnectLocalPlayer(pinkAuth, TeamName, "BINARY-LOCAL:PINK").ConfigureAwait(false);

            Console.WriteLine("BINARY-LOCAL: connecting green player...");
            greenGalaxy = await ConnectLocalPlayer(greenAuth, LocalSwitchGateGreenTeamName, "BINARY-LOCAL:GREEN").ConfigureAwait(false);

            pinkEventPump = StartEventPump("BINARY-LOCAL:PINK", pinkGalaxy, pinkEvents);
            greenEventPump = StartEventPump("BINARY-LOCAL:GREEN", greenGalaxy, greenEvents);

            await Task.Delay(SetupDelayMs).ConfigureAwait(false);
            DrainEvents(pinkEvents);
            DrainEvents(greenEvents);

            Player greenPlayerFromPinkView = pinkGalaxy.Players[greenGalaxy.Player.Name];
            Player pinkPlayerFromGreenView = greenGalaxy.Players[pinkGalaxy.Player.Name];

            Console.WriteLine($"BINARY-LOCAL: pink={pinkGalaxy.Player.Name}, green={greenGalaxy.Player.Name}");

            byte[] openerMessage = new byte[] { 0x10, 0x20, 0x30 };

            await greenPlayerFromPinkView.Chat(openerMessage).ConfigureAwait(false);
            await WaitForBinaryChat(greenEvents, greenTrace, pinkGalaxy.Player, openerMessage, greenGalaxy.Player, EventTimeoutMs, "BINARY-LOCAL: opener")
                .ConfigureAwait(false);

            try
            {
                await greenPlayerFromPinkView.Chat(new byte[] { 0x40 }).ConfigureAwait(false);
                throw new InvalidOperationException("BINARY-LOCAL: expected BinaryChatAckRequiredGameException for second unacknowledged binary message.");
            }
            catch (BinaryChatAckRequiredGameException exception)
            {
                Console.WriteLine($"BINARY-LOCAL: second unacknowledged single blocked as expected ({exception.Message})");
            }

            List<byte[]> deniedBulk = new List<byte[]>();
            deniedBulk.Add(new byte[] { 0x41 });
            deniedBulk.Add(new byte[] { 0x42, 0x43 });

            try
            {
                await greenPlayerFromPinkView.Chat(deniedBulk).ConfigureAwait(false);
                throw new InvalidOperationException("BINARY-LOCAL: expected BinaryChatAckRequiredGameException for unacknowledged bulk binary chat.");
            }
            catch (BinaryChatAckRequiredGameException exception)
            {
                Console.WriteLine($"BINARY-LOCAL: unacknowledged bulk blocked as expected ({exception.Message})");
            }

            byte[] ackMessage = new byte[] { 0x7A, 0x7B };
            await pinkPlayerFromGreenView.Chat(ackMessage).ConfigureAwait(false);
            await WaitForBinaryChat(pinkEvents, pinkTrace, greenGalaxy.Player, ackMessage, pinkGalaxy.Player, EventTimeoutMs, "BINARY-LOCAL: ack")
                .ConfigureAwait(false);

            byte[] tooLargeMessage = GC.AllocateUninitializedArray<byte>(1025, false);

            for (int index = 0; index < tooLargeMessage.Length; index++)
                tooLargeMessage[index] = (byte)index;

            try
            {
                await greenPlayerFromPinkView.Chat(tooLargeMessage).ConfigureAwait(false);
                throw new InvalidOperationException("BINARY-LOCAL: expected InvalidArgumentGameException for oversized binary message.");
            }
            catch (InvalidArgumentGameException exception)
            {
                if (exception.Reason != InvalidArgumentKind.TooLarge || exception.Parameter != "message")
                    throw new InvalidOperationException(
                        $"BINARY-LOCAL: oversized binary message returned unexpected validation {exception.Reason} on {exception.Parameter}.");

                Console.WriteLine("BINARY-LOCAL: oversized single message rejected as expected.");
            }

            List<byte[]> tooManyMessages = new List<byte[]>();

            for (int index = 0; index < 33; index++)
                tooManyMessages.Add(new byte[] { (byte)index });

            try
            {
                await greenPlayerFromPinkView.Chat(tooManyMessages).ConfigureAwait(false);
                throw new InvalidOperationException("BINARY-LOCAL: expected InvalidArgumentGameException for oversized binary bulk.");
            }
            catch (InvalidArgumentGameException exception)
            {
                if (exception.Reason != InvalidArgumentKind.TooLarge || exception.Parameter != "messages")
                    throw new InvalidOperationException(
                        $"BINARY-LOCAL: oversized binary bulk returned unexpected validation {exception.Reason} on {exception.Parameter}.");

                Console.WriteLine("BINARY-LOCAL: oversized bulk rejected as expected.");
            }

            List<byte[]> acceptedBulk = new List<byte[]>();
            byte[] maximumMessage = GC.AllocateUninitializedArray<byte>(1024, false);

            for (int index = 0; index < maximumMessage.Length; index++)
                maximumMessage[index] = (byte)(index * 13);

            acceptedBulk.Add(maximumMessage);

            for (int index = 1; index < 32; index++)
            {
                byte[] message = GC.AllocateUninitializedArray<byte>(index + 1, false);

                for (int byteIndex = 0; byteIndex < message.Length; byteIndex++)
                    message[byteIndex] = (byte)(index + byteIndex * 3);

                acceptedBulk.Add(message);
            }

            await greenPlayerFromPinkView.Chat(acceptedBulk).ConfigureAwait(false);

            for (int index = 0; index < acceptedBulk.Count; index++)
                await WaitForBinaryChat(greenEvents, greenTrace, pinkGalaxy.Player, acceptedBulk[index], greenGalaxy.Player, EventTimeoutMs,
                    $"BINARY-LOCAL: bulk[{index}]").ConfigureAwait(false);

            Console.WriteLine("BINARY-LOCAL: success");
        }
        finally
        {
            if (greenGalaxy is not null)
                greenGalaxy.Dispose();

            if (pinkGalaxy is not null)
                pinkGalaxy.Dispose();

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            await WaitForSessionGalaxy(greenAuth, null, 7000).ConfigureAwait(false);
            await WaitForSessionGalaxy(pinkAuth, null, 7000).ConfigureAwait(false);
            await WaitForSessionGalaxy(adminAuth, null, 7000).ConfigureAwait(false);

            if (galaxyProcess is not null)
                StopProcess(galaxyProcess);
        }
    }

    private static async Task<PlayerBinaryChatEvent> WaitForBinaryChat(ConcurrentQueue<FlattiverseEvent> events, List<FlattiverseEvent> trace,
        Player expectedSender, byte[] expectedMessage, Player expectedDestination, int timeoutMs, string label)
    {
        PlayerBinaryChatEvent? binaryEvent = await WaitForQueuedEvent<PlayerBinaryChatEvent>(events, timeoutMs, trace,
            delegate (PlayerBinaryChatEvent @event)
            {
                return @event.Player.Id == expectedSender.Id;
            }).ConfigureAwait(false);

        if (binaryEvent is null)
            throw new InvalidOperationException($"{label}: expected binary chat event within {timeoutMs} ms.");

        if (!ReferenceEquals(binaryEvent.Destination, expectedDestination))
            throw new InvalidOperationException(
                $"{label}: binary chat destination mismatch. Expected {expectedDestination.Name}, got {binaryEvent.Destination.Name}.");

        if (!binaryEvent.Message.AsSpan().SequenceEqual(expectedMessage))
            throw new InvalidOperationException(
                $"{label}: binary chat payload mismatch. Expected 0x{FormatBinary(expectedMessage)}, got 0x{FormatBinary(binaryEvent.Message)}.");

        return binaryEvent;
    }

    private static string FormatBinary(byte[] message)
    {
        int previewLength = Math.Min(message.Length, 32);
        string preview = Convert.ToHexString(message.AsSpan(0, previewLength));

        if (message.Length > previewLength)
            return $"{preview}...";

        return preview;
    }
}
