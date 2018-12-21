using System;
using Xunit;
using Moq;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {


        private string GetLicenseKeyExpireString()
        {
            //example, read from a vendor file 
            return "EXPIRED";
        }


        [Fact]
        public void AcceptHighIncomeApplications()
        {

            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.DefaultValue = DefaultValue.Mock;

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Loose);

            // Using It class for Argument matching in mocked methods
            // mockValidator.Setup(x => x.IsValid("x")).Returns(true);
            // mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            // mockValidator.Setup(x => x.IsValid(It.IsIn<string>("x","y","z"))).Returns(true);
            // mockValidator.Setup(x => x.IsValid(It.IsInRange("x","y",Range.Inclusive))).Returns(true);
            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]", System.Text.RegularExpressions.RegexOptions.None))).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,

                FrequentFlyerNumber = "x" // use this if it.isIn,It.IsInRange or is.valid method
                // using It class from moq.
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }


        [Fact]
        public void DeclineLowIncomeApplicationsOutDemo()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Loose);

            // Using It class for Argument matching in mocked methods

            bool isValid = true;
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,

                FrequentFlyerNumber = "x" // use this if it.isIn,It.IsInRange or is.valid method
                // using It class from moq.
            };

            CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }


        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]", System.Text.RegularExpressions.RegexOptions.None))).Returns(false);
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 20_000,
                Age = 42,
                FrequentFlyerNumber = "x" // use this if it.isIn,It.IsInRange or is.valid method
                // using It class from moq.
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }


        [Fact]
        public void ReferWhenLicenseKeyIsExpired()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Loose);
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns(GetLicenseKeyExpireString);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "x" // use this if it.isIn,It.IsInRange or is.valid method
                // using It class from moq.
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }


        [Fact]
        public void UseDetailedLookupForOlderApplication()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.SetupAllProperties(); // will remeber all the changes tracking of the properties.
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            //mockValidator.SetupProperty(x => x.ValidationMode); // will remeber change tracking of the property. 
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                Age = 30,
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }

        [Fact]
        public void ValidateFrequentNumberForLowIncomeApplication()
        {


            var mockValidation = new Mock<IFrequentFlyerNumberValidator>();

            mockValidation.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(mockValidation.Object);



            var application = new CreditCardApplication { FrequentFlyerNumber = "q" };


            sut.Evaluate(application);

            mockValidation.Verify(x => x.IsValid(It.IsAny<string>()), Times.Exactly(1)); // kontrollerar att den körs endast en gång!


        }

        //[Fact]
        //public void ShouldValidateFrequentNumberForLowIncomeApplication_CustomMessage()
        //{


        //    var mockValidation = new Mock<IFrequentFlyerNumberValidator>();

        //    mockValidation.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
        //    var sut = new CreditCardApplicationEvaluator(mockValidation.Object);




        //    var application = new CreditCardApplication { };


        //    sut.Evaluate(application);

        //    mockValidation.Verify(x => x.IsValid(It.IsNotNull<string>()),"Frequent flyer shouldnt be null");


        //}

        [Fact]
        public void NotValidateFrequentNumberForLowIncomeApplication()
        {


            var mockValidation = new Mock<IFrequentFlyerNumberValidator>();

            mockValidation.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(mockValidation.Object);




            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };


            sut.Evaluate(application);

            mockValidation.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);


        }



        [Fact]
        public void CheckLicenseKeyForLowIncomeApplication()
        {


            var mockValidation = new Mock<IFrequentFlyerNumberValidator>();

            mockValidation.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(mockValidation.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };

            sut.Evaluate(application);

            mockValidation.VerifyGet(x => x.ServiceInformation.License.LicenseKey,Times.Once);


        }


        [Fact]
        public void SetDetailedLookupForOlderApplications()
        {
            var mockValidation = new Mock<IFrequentFlyerNumberValidator>();

            mockValidation.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(mockValidation.Object);

            var application = new CreditCardApplication { Age = 30};

            sut.Evaluate(application);

            //mockValidation.VerifySet(x => x.ValidationMode=ValidationMode.Detailed); // we expect what the property should be set to in the lambda expresion
            mockValidation.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(),Times.Once); // om det inte spelar nåt roll vad man förväntar sig värdet plir för egenskapen. 

        }
    }
}
