namespace FikaServer.Models.Fika;

public record FikaItemEventRouter
{
    public const string SENDTOPLAYER = "SendToPlayer";
    public const string HIDEOUT_COOP_SPEND = "FikaHideoutCoopSpend";
    public const string HIDEOUT_COOP_APPLY_UPGRADE = "FikaHideoutCoopApplyUpgrade";
    public const string HIDEOUT_COOP_COMPLETE = "FikaHideoutCoopComplete";
    public const string HIDEOUT_COOP_REFUND = "FikaHideoutCoopRefund";
}