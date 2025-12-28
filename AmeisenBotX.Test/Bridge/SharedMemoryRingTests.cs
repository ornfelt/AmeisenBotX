using AmeisenBotX.Bridge;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace AmeisenBotX.Test.Bridge;

[TestClass]
public unsafe class SharedMemoryRingTests
{
    private SharedMemoryRing? botRing;
    private SharedMemoryRing? implantRing;
    private const string TestIpcName = "Local\\WM_IPC_TEST_12345";

    [TestInitialize]
    public void Setup()
    {
        // Clean up any existing shared memory from previous failed tests
        Cleanup();
    }

    [TestCleanup]
    public void Cleanup()
    {
        botRing?.Dispose();
        implantRing?.Dispose();
        botRing = null;
        implantRing = null;
    }

    [TestMethod]
    public void CreateSharedMemory_BotSide_InitializesHeader()
    {
        // Arrange & Act
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);

        // Assert
        Assert.AreEqual(BridgeStatus.Ready | BridgeStatus.BotConnected, botRing.Status);
    }

    [TestMethod]
    public void OpenSharedMemory_ImplantSide_ValidatesHeader()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);

        // Act
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);

        // Assert
        Assert.AreEqual(BridgeStatus.Ready | BridgeStatus.BotConnected | BridgeStatus.ImplantConnected,
            implantRing.Status);
    }

    [TestMethod]
    public void SingleMessage_BotToImplant_SuccessfulTransfer()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);

        byte[] sendData = new byte[] { 1, 2, 3, 4, 5 };
        byte[] receiveBuffer = new byte[10];

        // Act
        bool written = botRing.TryWrite(sendData);
        bool read = implantRing.TryRead(receiveBuffer, out int bytesRead);

        // Assert
        Assert.IsTrue(written, "Write should succeed");
        Assert.IsTrue(read, "Read should succeed");
        Assert.AreEqual(sendData.Length, bytesRead);
        CollectionAssert.AreEqual(sendData, receiveBuffer[..bytesRead]);
    }

    [TestMethod]
    public void SingleMessage_ImplantToBot_SuccessfulTransfer()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);

        byte[] sendData = new byte[] { 10, 20, 30, 40 };
        byte[] receiveBuffer = new byte[10];

        // Act
        bool written = implantRing.TryWrite(sendData);
        bool read = botRing.TryRead(receiveBuffer, out int bytesRead);

        // Assert
        Assert.IsTrue(written);
        Assert.IsTrue(read);
        Assert.AreEqual(sendData.Length, bytesRead);
        CollectionAssert.AreEqual(sendData, receiveBuffer[..bytesRead]);
    }

    [TestMethod]
    public void MultipleMessages_Sequential_AllTransferred()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);

        int messageCount = 100;
        byte[] receiveBuffer = new byte[10];

        // Act & Assert
        for (int i = 0; i < messageCount; i++)
        {
            byte[] sendData = new byte[] { (byte)i, (byte)(i + 1), (byte)(i + 2) };

            Assert.IsTrue(botRing.TryWrite(sendData), $"Write {i} failed");
            Assert.IsTrue(implantRing.TryRead(receiveBuffer, out int bytesRead), $"Read {i} failed");
            Assert.AreEqual(sendData.Length, bytesRead);
            CollectionAssert.AreEqual(sendData, receiveBuffer[..bytesRead]);
        }
    }

    [TestMethod]
    public void LargeMessage_NearMaxSize_SuccessfulTransfer()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);

        int largeSize = BridgeProtocol.MaxPayloadSize + Marshal.SizeOf<MessageHeader>();
        byte[] sendData = new byte[largeSize];
        new Random(42).NextBytes(sendData);
        byte[] receiveBuffer = new byte[largeSize + 100];

        // Act
        bool written = botRing.TryWrite(sendData);
        bool read = implantRing.TryRead(receiveBuffer, out int bytesRead);

        // Assert
        Assert.IsTrue(written);
        Assert.IsTrue(read);
        Assert.AreEqual(largeSize, bytesRead);
        CollectionAssert.AreEqual(sendData, receiveBuffer[..bytesRead]);
    }

    [TestMethod]
    public void OversizedMessage_ExceedsMaxSize_WriteFails()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);

        int tooLargeSize = BridgeProtocol.MaxPayloadSize + Marshal.SizeOf<MessageHeader>() + 1;
        byte[] sendData = new byte[tooLargeSize];

        // Act
        bool written = botRing.TryWrite(sendData);

        // Assert
        Assert.IsFalse(written, "Oversized write should fail");
    }

    [TestMethod]
    public void ReadEmptyBuffer_NoData_ReturnsFalse()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);
        byte[] receiveBuffer = new byte[10];

        // Act
        bool read = implantRing.TryRead(receiveBuffer, out int bytesRead);

        // Assert
        Assert.IsFalse(read, "Read from empty buffer should return false");
        Assert.AreEqual(0, bytesRead);
    }

    [TestMethod]
    public void RingBufferWrapping_CrossesBoundary_DataIntact()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);

        // Fill buffer to near capacity to force wrapping
        int messageSize = 8192; // 8KB messages
        int messageCount = (BridgeProtocol.RingBufferSize / messageSize) - 1;

        byte[] receiveBuffer = new byte[messageSize];

        // Write and read messages to advance write position near end
        for (int i = 0; i < messageCount; i++)
        {
            byte[] data = new byte[messageSize];
            data[0] = (byte)i;
            botRing.TryWrite(data);
            implantRing.TryRead(receiveBuffer, out _);
        }

        // Act - Write message that will wrap around
        byte[] wrapData = new byte[messageSize];
        new Random(123).NextBytes(wrapData);

        bool written = botRing.TryWrite(wrapData);
        bool read = implantRing.TryRead(receiveBuffer, out int bytesRead);

        // Assert
        Assert.IsTrue(written, "Wrap-around write should succeed");
        Assert.IsTrue(read, "Wrap-around read should succeed");
        Assert.AreEqual(wrapData.Length, bytesRead);
        CollectionAssert.AreEqual(wrapData, receiveBuffer[..bytesRead]);
    }

    [TestMethod]
    public void ConcurrentReadWrite_StressTest_NoDataCorruption()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);

        int messageCount = 1000;
        int[] writeCounters = new int[messageCount];
        int[] readCounters = new int[messageCount];
        bool testFailed = false;

        // Act - Producer thread (bot writes)
        Thread writerThread = new(() =>
        {
            try
            {
                for (int i = 0; i < messageCount; i++)
                {
                    byte[] data = BitConverter.GetBytes(i);
                    while (!botRing.TryWrite(data))
                    {
                        Thread.Sleep(1); // Buffer full, wait
                    }
                    Interlocked.Increment(ref writeCounters[i]);
                }
            }
            catch
            {
                testFailed = true;
            }
        });

        // Consumer thread (implant reads)
        Thread readerThread = new(() =>
        {
            try
            {
                byte[] buffer = new byte[256];
                int received = 0;

                while (received < messageCount)
                {
                    if (implantRing.TryRead(buffer, out int bytesRead))
                    {
                        int value = BitConverter.ToInt32(buffer, 0);
                        Assert.AreEqual(received, value, "Messages out of order!");
                        Interlocked.Increment(ref readCounters[value]);
                        received++;
                    }
                    else
                    {
                        Thread.Sleep(1); // Buffer empty, wait
                    }
                }
            }
            catch
            {
                testFailed = true;
            }
        });

        writerThread.Start();
        readerThread.Start();

        // Assert
        Assert.IsTrue(writerThread.Join(TimeSpan.FromSeconds(10)), "Writer timeout");
        Assert.IsTrue(readerThread.Join(TimeSpan.FromSeconds(10)), "Reader timeout");
        Assert.IsFalse(testFailed, "Test encountered exception");

        // Verify all messages sent and received exactly once
        for (int i = 0; i < messageCount; i++)
        {
            Assert.AreEqual(1, writeCounters[i], $"Message {i} write count wrong");
            Assert.AreEqual(1, readCounters[i], $"Message {i} read count wrong");
        }
    }

    [TestMethod]
    public void PerformanceBenchmark_Throughput_MeetsTarget()
    {
        // Arrange
        botRing = new SharedMemoryRing(TestIpcName, isBotSide: true);
        implantRing = new SharedMemoryRing(TestIpcName, isBotSide: false);

        int messageCount = 100_000;
        byte[] sendData = new byte[64]; // Small messages
        byte[] receiveBuffer = new byte[128];

        // Warm up
        for (int i = 0; i < 100; i++)
        {
            botRing.TryWrite(sendData);
            implantRing.TryRead(receiveBuffer, out _);
        }

        // Act
        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < messageCount; i++)
        {
            while (!botRing.TryWrite(sendData))
            {
                Thread.SpinWait(10);
            }

            while (!implantRing.TryRead(receiveBuffer, out _))
            {
                Thread.SpinWait(10);
            }
        }

        sw.Stop();

        // Assert
        double messagesPerSecond = messageCount / sw.Elapsed.TotalSeconds;
        double avgLatencyMicros = sw.Elapsed.TotalMilliseconds * 1000 / messageCount;

        Console.WriteLine($"Throughput: {messagesPerSecond:N0} msg/sec");
        Console.WriteLine($"Avg Latency: {avgLatencyMicros:F2} μs");

        // Target: >100k messages/sec, <100μs avg latency
        Assert.IsTrue(messagesPerSecond > 100_000, $"Throughput too low: {messagesPerSecond:N0} msg/sec");
        Assert.IsTrue(avgLatencyMicros < 100, $"Latency too high: {avgLatencyMicros:F2} μs");
    }
}
