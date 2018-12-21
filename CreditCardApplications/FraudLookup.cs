using System;
using System.Collections.Generic;
using System.Text;

namespace CreditCardApplications
{
    public class FraudLookup
    {



        public bool isFraudRisk(CreditCardApplication application)
        {

            return CheckApplication(application);
        }

        protected virtual bool CheckApplication(CreditCardApplication application)
        {
            if (application.LastName == "Rashid")
            {
                return true;
            }

            return false;
        }
    }
}
