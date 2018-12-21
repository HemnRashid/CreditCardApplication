﻿namespace CreditCardApplications
{
    public class CreditCardApplicationEvaluator
    {
        private readonly IFrequentFlyerNumberValidator _validator;
        private const int AutoReferralMaxAge = 20;
        private const int HighIncomeThreshhold = 100_000;
        private const int LowIncomeThreshhold = 20_000;

        private readonly FraudLookup _fraudLookup;

        
        public int ValidatorLookupCount { get; private set; }

        public CreditCardApplicationEvaluator(IFrequentFlyerNumberValidator validator, FraudLookup fraudLookup=null)
        {
            
            _validator = validator ?? throw new System.ArgumentNullException(nameof(validator));

            _validator.ValidatorLookupPerformed += _validator_ValidatorLookupPerformed;
            _fraudLookup = fraudLookup;
        }

        private void _validator_ValidatorLookupPerformed(object sender, System.EventArgs e)
        {
            ValidatorLookupCount++;
        }

        public CreditCardApplicationDecision Evaluate(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            if(_fraudLookup!=null && _fraudLookup.isFraudRisk(application))
            {
                return CreditCardApplicationDecision.RefeeredToHumanFraudRisk;
            }

            //if(_validator.LicenseKey=="EXPIRED")
            //{
            //    return CreditCardApplicationDecision.ReferredToHuman;
            //}

            if (_validator.ServiceInformation.License.LicenseKey == "expired".ToUpper())
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            _validator.ValidationMode = application.Age >= 30 ? ValidationMode.Detailed : ValidationMode.Quick;


            bool isValidFrequentFlyerNumber;
            try
            {

                isValidFrequentFlyerNumber = _validator.IsValid(application.FrequentFlyerNumber);
            }
            catch (System.Exception)
            {

                return CreditCardApplicationDecision.ReferredToHuman;
            }



            if (!isValidFrequentFlyerNumber)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }

        public CreditCardApplicationDecision EvaluateUsingOut(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }


            _validator.IsValid(application.FrequentFlyerNumber, out var isValidFrequentFlyerNumber);


            if (!isValidFrequentFlyerNumber)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }
    }
}
