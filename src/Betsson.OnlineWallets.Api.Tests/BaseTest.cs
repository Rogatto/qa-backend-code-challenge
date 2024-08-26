using static RestAssured.Dsl;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
public class BaseTest

{
    public static string hostApi = GetHostOnlineWallet();
    public static string pathBalance = "/onlinewallet/balance";
    public static string pathWithDraw = "/onlinewallet/withdraw";
    public static string pathDeposit= "/onlinewallet/deposit";

    public static IConfigurationRoot GetIConfigurationRoot()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static string GetHostOnlineWallet()
    {
        var configuration = GetIConfigurationRoot();
        string baseUrl = configuration.GetValue<string>("OnlineWallet:BaseUrl");
        return baseUrl;
    }

    public static void VerifyFundsAndResetBalance()
    {
        //Verify initializing wallet it might not contain funds
        double balanceAmount = CurrentBalance();

        if(balanceAmount > 0){
            WithdrawFundFromWallet(balanceAmount);
        }
    }

    public static double CurrentBalance ()
    {
        double amountBalance =
        (double)Given()
            .When()
            .Get(hostApi + pathBalance)
            .Then()
            .StatusCode(200)
            .And()
            .Extract().Body("$.amount");
        
        return amountBalance;
    }

    public static void WithdrawFundFromWallet (double amountToWithdraw)
    {
        //Check new withdraw from the wallet
        string requestBody = JsonSerializer.Serialize(new 
        {
            amount = amountToWithdraw
        });
        Given()
            .Body(requestBody)
            .When()
            .Post(hostApi + pathWithDraw)
            .Then()
            .StatusCode(200);
    }

    public static void DepositFundToWallet (double amountToDeposit)
    {
        //Check deposit from the wallet
        string requestBody = JsonSerializer.Serialize(new 
        {
            amount = amountToDeposit
        });
        Given()
            .Body(requestBody)
            .When()
            .Post(hostApi + pathDeposit)
            .Then()
            .StatusCode(200);
    }
}