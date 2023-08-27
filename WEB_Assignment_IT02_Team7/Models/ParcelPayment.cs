namespace WEB_Assignment_IT02_Team7.Models
{
    public class ParcelPayment
    {
        public Parcel Parcels { get; set; }

        public PaymentTransaction PaymentTransactions { get; set; }

        public CashVoucher CashVouchers { get; set; }

        public ParcelPayment()
        {
            Parcels = new Parcel();
            CashVouchers = new CashVoucher();
            PaymentTransactions = new PaymentTransaction();
        }
    }
}
