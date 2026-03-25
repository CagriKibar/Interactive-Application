namespace FinancialERP.Entity.DTOs
{
    /// <summary>
    /// Nakit akışı sağlık durumu DTO'su.
    /// İşletmenin finansal sağlığını kapsamlı şekilde değerlendirir.
    /// </summary>
    public class CashFlowHealthDto
    {
        /// <summary>Toplam banka bakiyesi (TRY)</summary>
        public decimal TotalBankBalance { get; set; }

        /// <summary>Toplam kredi kartı borcu (TRY)</summary>
        public decimal TotalCreditCardDebt { get; set; }

        /// <summary>Yaklaşan ödemeler toplamı (TRY)</summary>
        public decimal UpcomingPayments { get; set; }

        /// <summary>Net nakit pozisyonu (TRY)</summary>
        public decimal NetCashPosition { get; set; }

        /// <summary>Nakit akışı sağlık skoru (0-100 arası)</summary>
        public decimal CashFlowHealthScore { get; set; }

        /// <summary>Reel değer - USD karşılığı</summary>
        public decimal RealValueInUSD { get; set; }

        /// <summary>Enflasyona göre düzeltilmiş reel değer (TRY)</summary>
        public decimal RealValueAdjustedForInflation { get; set; }

        /// <summary>AI önerileri listesi</summary>
        public List<string> Recommendations { get; set; } = new();
    }
}
