namespace Kulipa.Sdk.Models.Enums
{
    /// <summary>
    ///     Represents the type of spending control that can be applied to a card.
    /// </summary>
    public enum SpendingControlType
    {
        /// <summary>
        ///     A purchase amount limit control that restricts spending within a defined time period.
        ///     <para>
        ///         This control type allows you to set a maximum spending amount (e.g., $1000) over a
        ///         specified period (daily, monthly, or yearly). The available amount decreases as
        ///         transactions are made and resets at the start of each new period.
        ///     </para>
        ///     <example>
        ///         <code>
        ///     // Create a daily spending limit of $500
        ///     var request = new CreateSpendingControlRequest
        ///     {
        ///         Description = "Daily spending limit",
        ///         Type = SpendingControlType.Purchase,
        ///         Config = SpendingControlConfig.ForPurchaseLimit(SpendingControlPeriod.Daily, 500)
        ///     };
        ///     </code>
        ///     </example>
        /// </summary>
        Purchase,

        /// <summary>
        ///     A merchant category code (MCC) blocking control that prevents transactions at specific merchant types.
        ///     <para>
        ///         This control type allows you to block transactions at merchants belonging to specific
        ///         category codes. MCCs are four-digit codes used by payment networks to classify merchants
        ///         by the type of goods or services they provide (e.g., 7995 for gambling, 5912 for drug stores).
        ///     </para>
        ///     <example>
        ///     <code>
        ///     // Block gambling and lottery merchants
        ///     var request = new CreateSpendingControlRequest
        ///     {
        ///         Description = "Block gambling transactions",
        ///         Type = SpendingControlType.BlockedMcc,
        ///         Config = SpendingControlConfig.ForBlockedMcc(["7995", "7800", "7801"])
        ///     };
        ///     </code>
        ///     </example>
        /// </summary>
        BlockedMcc
    }
}