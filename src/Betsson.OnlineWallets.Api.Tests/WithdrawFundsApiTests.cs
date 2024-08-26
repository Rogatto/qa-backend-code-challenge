namespace Betsson.OnlineWallets.Api.Tests;
using static RestAssured.Dsl;
using Faker;
using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using System.Text.Json;

[Collection("Sequential")]
[AllureFeature("Withdraw Funds")]
[AllureSuite("Withdraw Funds")]
public class WithdrawFundsApiTests : BaseTest

{
    [Fact(DisplayName = "Should not withdraw with insufficient funds")]
    public void ShouldNotWithDrawWithInsufficientFunds()
    {
        //First reset balance to keep this test independent
        VerifyFundsAndResetBalance();

        double amount = Faker.RandomNumber.Next(999);
        string requestBody = JsonSerializer.Serialize(new 
        {
            amount = amount
        });

        Given()
            .Body(requestBody)
            .When()
            .Post(hostApi + pathWithDraw)
            .Then()
            .StatusCode(400)
            .And()
            .Body("$.type", NHamcrest.Is.EqualTo("InsufficientBalanceException"))
            .Body("$.title", NHamcrest.Is.EqualTo("Invalid withdrawal amount. There are insufficient funds."))
            .Body("$.status", NHamcrest.Is.EqualTo(400));
    }

    [Fact(DisplayName = "Should not withdraw negative amount from the wallet")]
    public void ShouldNotWithdrawNegativeAmount()
    {
        double amount = Faker.RandomNumber.Next(-999);
        string requestBody = JsonSerializer.Serialize(new 
        {
            amount = amount
        });

        Given()
            .Body(requestBody)
            .When()
            .Post(hostApi + pathWithDraw)
            .Then()
            .StatusCode(400)
            .And()
            .Body("$.title", NHamcrest.Is.EqualTo("One or more validation errors occurred."))
            .Body("$.status", NHamcrest.Is.EqualTo(400))
            .Body("$.errors.Amount[0]", NHamcrest.Is.EqualTo("'Amount' must be greater than or equal to '0'."));
    }

    [Fact(DisplayName = "Should withdraw amount from the wallet's funds")]
    [AllureSeverity(SeverityLevel.critical)]
    public void ShouldWithdrawAmountFromBalance()
    {

        double amount = Faker.RandomNumber.Next(999);
        string requestBody = JsonSerializer.Serialize(new 
        {
            amount = amount
        });

        //First reset balance to keep this test independent
        VerifyFundsAndResetBalance();

        //Deposit funds before starting to withdraw
        DepositFundToWallet(amount);

        Given()
            .Body(requestBody)
            .When()
            .Post(hostApi + pathWithDraw)
            .Then()
            .StatusCode(200)
            .And()
            .Body("$.amount", NHamcrest.Is.EqualTo(0));

        //Check if balance is updated after withdraw
        Given()
            .When()
            .Get(hostApi + pathBalance)
            .Then()
            .StatusCode(200)
            .And()
            .Body("$.amount", NHamcrest.Is.EqualTo(0));
    }
}