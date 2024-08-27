using System.Threading.Tasks;
using Moq;
using Xunit;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Exceptions;

namespace Betsson.OnlineWallets.UnitTests.Services
{
    public class GetBalanceAsyncTests
    {
        private readonly Mock<IOnlineWalletRepository> _onlineWalletRepositoryMock;
        private readonly OnlineWalletService _onlineWalletService;

        public GetBalanceAsyncTests()
        {
            _onlineWalletRepositoryMock = new Mock<IOnlineWalletRepository>();
            _onlineWalletService = new OnlineWalletService(_onlineWalletRepositoryMock.Object);
        }

        [Fact]
        public async Task GetBalance_ShouldReturnZero_WhenInitializingBalance()
        {
            // Arrange
            _onlineWalletRepositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync((OnlineWalletEntry)null);

            // Act
            Balance result = await _onlineWalletService.GetBalanceAsync();

            // Assert
            Assert.Equal(0.00m, result.Amount);
        }

        [Fact]
        public async Task GetBalance_ShouldReturnCorrectBalance_WhenItHasFunds()
        {
            // Arrange
            var lastEntry = new OnlineWalletEntry
            {
                BalanceBefore = 50.00m,
                Amount = 25.00m
            };

            _onlineWalletRepositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(lastEntry);

            // Act
            Balance result = await _onlineWalletService.GetBalanceAsync();

            // Assert
            Assert.Equal(75.00m, result.Amount);
        }
    }
}