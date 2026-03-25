namespace FinancialERP.Entity.Enums
{
    /// <summary>
    /// Bildirim önem seviyesi
    /// </summary>
    public enum NotificationSeverity
    {
        /// <summary>Bilgilendirme amaçlı</summary>
        Info = 0,

        /// <summary>Uyarı seviyesi</summary>
        Warning = 1,

        /// <summary>Kritik seviye - acil aksiyon gerektirir</summary>
        Critical = 2,

        /// <summary>Fırsat bildirimi - kaçırılmaması gereken durum</summary>
        Opportunity = 3
    }
}
