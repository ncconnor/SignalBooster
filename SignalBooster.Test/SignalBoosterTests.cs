using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Synapse.SignalBoosterExample;
using Synapse.SignalBoosterExample.Models;
using Synapse.SignalBoosterExample.NoteOrderProcessor;
using Synapse.SignalBoosterExample.NoteReader;
using Synapse.SignalBoosterExample.OrderSender;
using Xunit;

namespace Synapse.SignalBoosterExample.Tests
{
    public class SignalBoosterTests
    {
        [Fact]
        public void ProcessAndSubmitPhysicianNote_CreatesExpectedMedicalOrder()
        {
            // Arrange
            var expectedNote = "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";

            var expectedOrder = new MedicalOrder
            {
                Device = "CPAP",
                MaskType = "full face",
                AddOns = new[] { "humidifier" },
                Qualifier = "AHI > 20",
                OrderingProvider = "Dr. Cameron",
                Liters = null,
                Usage = null
            };

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger<SignalBooster>>();
            // Use typeof(SignalBooster).FullName as the category name
            mockLoggerFactory
                .Setup(f => f.CreateLogger(It.Is<string>(s => s == typeof(SignalBooster).FullName)))
                .Returns(mockLogger.Object);

            var mockNoteReader = new Mock<IPhysicianNoteReader>();
            mockNoteReader.Setup(r => r.ReadPhysicianNote()).Returns(expectedNote);

            var mockOrderProcessor = new Mock<IPhysicianNoteOrderProcessor>();
            mockOrderProcessor.Setup(p => p.Parse(expectedNote)).Returns(expectedOrder);

            string capturedJson = null;
            var mockOrderSender = new Mock<IOrderSender>();
            mockOrderSender
                .Setup(s => s.Submit(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((json, url) => capturedJson = json);

            var booster = new SignalBooster(
                loggerFactory: mockLoggerFactory.Object,
                orderProcessor: mockOrderProcessor.Object,
                orderSender: mockOrderSender.Object,
                physicianNoteReader: mockNoteReader.Object,
                apiUrl: "https://api.example.com/submit-order"
            );

            // Act
            booster.ProcessAndSubmitPhysicianNote();

            // Assert
            Assert.NotNull(capturedJson);
            Assert.Contains("\"device\":\"CPAP\"", capturedJson);
            Assert.Contains("\"maskType\":\"full face\"", capturedJson);
            Assert.Contains("\"addOns\":[\"humidifier\"]", capturedJson);
            Assert.Contains("\"orderingProvider\":\"Dr. Cameron\"", capturedJson);
            
            var actualOrder = JsonSerializer.Deserialize<MedicalOrder>(capturedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Equal(expectedOrder.Device, actualOrder.Device);
            Assert.Equal(expectedOrder.MaskType, actualOrder.MaskType);
            Assert.Equal(expectedOrder.AddOns, actualOrder.AddOns);
            Assert.Equal(expectedOrder.Qualifier, actualOrder.Qualifier);
            Assert.Equal(expectedOrder.OrderingProvider, actualOrder.OrderingProvider);
            Assert.Equal(expectedOrder.Liters, actualOrder.Liters);
            Assert.Equal(expectedOrder.Usage, actualOrder.Usage);
        }
    }
}