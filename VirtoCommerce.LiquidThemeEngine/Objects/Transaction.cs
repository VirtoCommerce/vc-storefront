using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    /// <summary>
    /// Represent order transaction 
    /// https://help.shopify.com/themes/liquid/objects/transaction
    /// </summary>
    [DataContract]
    public partial class Transaction : Drop
    {
        /// <summary>
        /// Returns a unique numeric identifier for the transaction.
        /// </summary>
        [DataMember]
        public string Id { get; set; }
        /// <summary>
        /// Returns the amount of the transaction. Use one of the money filters to return the value in a monetary format.
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }
        /// <summary>
        /// Returns the name of the transaction.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Returns the status of the transaction.
        /// </summary>
        [DataMember]
        public string Status { get; set; }
        /// <summary>
        /// Returns the translated output of a transaction's status.
        /// </summary>
        [DataMember]
        public string StatusLabel { get; set; }
        /// <summary>
        /// Returns the timestamp of when the transaction was created. Use the date filter to format the timestamp.
        /// </summary>
        [DataMember]
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Returns text with information from the payment gateway about the payment receipt. This includes whether the payment was a test case and an authorization code if one was included in the transaction.
        /// </summary>
        [DataMember]
        public string Receipt { get; set; }
        /// <summary>
        /// Returns the type of transaction. There are five transaction types:
        // authorization is the reserving of money that the customer has agreed to pay.
        // capture is the transfer of the money that was reserved during the authorization stage.
        // sale is a combination of authorization and capture, performed in one step.
        // void is the cancellation of a pending authorization or capture.
        // refund is the partial or full refund of the captured money to the customer.
        /// </summary>
        [DataMember]
        public string Kind { get; set; }
        /// <summary>
        /// Returns the name of the payment gateway used for the transaction.
        /// </summary>
        [DataMember]
        public string Gateway { get; set; }
        /// <summary>
        /// The payment_details object contains additional properties related to the payment method used in the transaction.
        /// </summary>
        [DataMember]
        public string PaymentDetails { get; set; }
        [DataMember]
        public string CreditCardNumber { get; set; }

    }
}
