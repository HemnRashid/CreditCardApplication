using System;

namespace CreditCardApplications
{

    public enum ValidationMode
    {
        Quick,
        Detailed
    }

    public interface ILicenseData
    {
        string LicenseKey { get; }
    }

    public interface IServiceInformation
    {
        ILicenseData License { get; set; }
    }


    public interface IFrequentFlyerNumberValidator
    {

        //string LicenseKey { get; }

        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);


        IServiceInformation ServiceInformation { get; } // nested properties, anropar IServiceInformation och sedan i sin tur ILicens.

        ValidationMode ValidationMode { get; set; }
        
    }
}