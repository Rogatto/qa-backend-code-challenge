using static RestAssured.Dsl;
using System.Text.Json;
public class ApiFixture

{
    public static string hostApi = "http://localhost:5047";
    public static string pathBalance = "/onlinewallet/balance";
    public static string pathWithDraw = "/onlinewallet/withdraw";
    public static string pathDeposit= "/onlinewallet/deposit";


    public static void verifyFundsAndResetBalance(){

        //Verify initializing wallet it might not contain funds
        double balanceAmount = currentBalance();

        if(balanceAmount > 0){
            withdrawFundFromWallet(balanceAmount);
        }
    }

    public static double currentBalance (){
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

    public static void withdrawFundFromWallet (double amountToWithdraw){
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

    public static void depositFundToWallet (double amountToDeposit){
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