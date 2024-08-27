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
    public class WithdrawFundsAsyncTests
    {
        private readonly Mock<IOnlineWalletRepository> _onlineWalletRepositoryMock;
        private readonly OnlineWalletService _onlineWalletService;

        public WithdrawFundsAsyncTests()
        {
            _onlineWalletRepositoryMock = new Mock<IOnlineWalletRepository>();
            _onlineWalletService = new OnlineWalletService(_onlineWalletRepositoryMock.Object);
        }

        [Fact]
        public async Task WithdrawFundsAsync_ShouldThrowInsufficientBalanceException_WhenWithdrawalAmountExceedsBalance()
        {
            // Arrange
            var withdrawal = new Withdrawal { Amount = 150.00m };

            var currentBalance = new Balance { Amount = 100.00m };

            _onlineWalletRepositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry
                {
                    BalanceBefore = currentBalance.Amount,
                    Amount = 0.00m
                });

            // Act & Assert
            await Assert.ThrowsAsync<InsufficientBalanceException>(() => _onlineWalletService.WithdrawFundsAsync(withdrawal));
        }

        [Fact]
        public async Task WithdrawFundsAsync_ShouldReturnCorrectNewBalanceAfterWithDrawFunds()
        {
            // Arrange
            var withdrawal = new Withdrawal { Amount = 30.00m };

            var currentBalance = new Balance { Amount = 80.00m };

            _onlineWalletRepositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry
                {
                    BalanceBefore = currentBalance.Amount,
                    Amount = 0.00m
                });

            // Act
            Balance result = await _onlineWalletService.WithdrawFundsAsync(withdrawal);

            // Assert
            Assert.Equal(50.00m, result.Amount);
        }
    }
}