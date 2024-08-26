namespace Betsson.OnlineWallets.Api.Tests;
using static RestAssured.Dsl;
using Faker;
using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using System.Text.Json;

[Collection("Sequential")]
[AllureFeature("Deposit Funds")]
[AllureSuite("Deposit Funds")]
public class DepositFundsApiTests : BaseTest

{
    [Fact(DisplayName = "Should not deposit negative amount to a wallet's balance")]
    public void ShouldNotDepositNegativeAmount()
    {
        double amount = Faker.RandomNumber.Next(-999);
        string requestBody = JsonSerializer.Serialize(new 
        {
            amount = amount
        });

        Given()
            .Body(requestBody)
            .When()
            .Post(hostApi + pathDeposit)
            .Then()
            .StatusCode(400)
            .And()
            .Body("$.title", NHamcrest.Is.EqualTo("One or more validation errors occurred."))
            .Body("$.status", NHamcrest.Is.EqualTo(400))
            .Body("$.errors.Amount[0]", NHamcrest.Is.EqualTo("'Amount' must be greater than or equal to '0'."));
    }

    [Fact(DisplayName = "Should deposit amount to a wallet balance successfully")]
    [AllureSeverity(SeverityLevel.critical)]
    public void ShouldDepositAmountToWallet()
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
            .Post(hostApi + pathDeposit)
            .Then()
            .StatusCode(200)
            .And()
            .Body("$.amount", NHamcrest.Is.EqualTo(amount));

         //Check if balance is updated after deposit
        Given()
            .When()
            .Get(hostApi + pathBalance)
            .Then()
            .StatusCode(200)
            .And()
            .Body("$.amount", NHamcrest.Is.EqualTo(amount));
    }
}