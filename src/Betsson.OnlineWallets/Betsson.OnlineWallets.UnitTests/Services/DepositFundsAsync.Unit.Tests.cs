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
    public class DepositFundsAsyncTests
    {
        private readonly Mock<IOnlineWalletRepository> _onlineWalletRepositoryMock;
        private readonly OnlineWalletService _onlineWalletService;

        public DepositFundsAsyncTests()
        {
            _onlineWalletRepositoryMock = new Mock<IOnlineWalletRepository>();
            _onlineWalletService = new OnlineWalletService(_onlineWalletRepositoryMock.Object);
        }

        [Fact]
        public async Task DepositFunds_ShouldAddDepositAmountToCurrentBalance()
        {
            // Arrange
            var deposit = new Deposit { Amount = 50.00m };

            var currentBalance = new Balance { Amount = 100.00m };

            _onlineWalletRepositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry
                {
                    BalanceBefore = currentBalance.Amount,
                    Amount = 0.00m
                });

            // Act
            Balance result = await _onlineWalletService.DepositFundsAsync(deposit);

            // Assert
            Assert.Equal(150.00m, result.Amount);
        }
    }
}