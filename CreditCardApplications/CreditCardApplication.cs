namespace CreditCardApplications
{
    public class CreditCardApplication
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        /// <summary>
        /// Brutto årslön
        /// </summary>
        public decimal GrossAnnualIncome { get; set; } 

        /// <summary>
        /// Flygbonusnummer
        /// </summary>
        public string FrequentFlyerNumber { get; set; } 
    }
}
