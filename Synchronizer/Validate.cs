using System;
using System.Text.RegularExpressions;
using System.Collections;

namespace DNACircSynchronizer.Processes
{

    sealed public class Validate
    {
        private Validate()
        {
        }

        static public bool IsCurrency(string inToValidate)
        {
            if (inToValidate.Length == 0)
                return false;

            //			Regex regexIsNumber = new Regex(@"^\${0,1}\d{0,4}.{0,1}\d{0,2}$");
            Regex regexIsNumber = new Regex(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$");

            return regexIsNumber.Match(inToValidate).Success;
        }


        static public bool IsNumeric(string inToValidate)
        {
            Regex regexIsNumber = new Regex(@"^\d+$");
            return regexIsNumber.Match(inToValidate).Success;
        }

        static public bool IsNumeric(char inToValidate)
        {
            switch (inToValidate)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
                default:
                    return false;
            }
        }


        static public bool IsValidMonthNumber(string inToValidate)
        {
            if (IsNumeric(inToValidate))
            {
                Int32 monthNumber = Int32.Parse(inToValidate);
                if (monthNumber >= 1 && monthNumber <= 12)
                    return true;
            }
            return false;
        }

        static public bool IsValidYYYYMMDD(string inToValidate)
        {
            if (!IsNumeric(inToValidate) || inToValidate.Length != 8)
            {
                return false;
            }

            string year, month, day;
            year = inToValidate.Substring(0, 4);
            month = inToValidate.Substring(4, 2);
            day = inToValidate.Substring(6, 2);

            // Try to create a date, if create an exception, return false.
            try
            {
                DateTime test = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                return true;
            }
            catch
            {
                return false;
            }
        }


        static public bool IsValidCreditCardYear(string inToValidate)
        {
            if (IsNumeric(inToValidate))
            {
                string currentYear = DateTime.Now.Year.ToString().Substring(2, 2);
                if (Int32.Parse(currentYear) <= Int32.Parse(inToValidate))
                {
                    return true;
                }
            }
            return false;
        }

        static public bool IsValidPbmNumber(string inToValidate)
        {
            if (inToValidate.Trim().Length == 8 &&
                IsNumeric(inToValidate))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determine if the specified Credit Card number is a well-constructed
        /// Credit Card number.  This method does NOT determine if this is an 
        /// GOOD number, just that it is well-constructed.
        /// </summary>
        /// <param name="inToValidate">Credit Card number to validate.</param>
        /// <returns>True if the number is well-constructed, else false.</returns>
        static public bool IsValidCreditCardNumber(string inToValidate)
        {
            if (inToValidate.Trim().Length == 0)
                return false;
            try
            {
                // Array to contain individual numbers
                System.Collections.ArrayList CheckNumbers = new ArrayList();
                // So, get length of card
                int CardLength = inToValidate.Length;

                // Double the value of alternate digits, starting with the second digit
                // from the right, i.e. back to front.
                // Loop through starting at the end
                for (int i = CardLength - 2; i >= 0; i = i - 2)
                {
                    // Now read the contents at each index, this
                    // can then be stored as an array of integers
                    // Double the number returned
                    CheckNumbers.Add(Int32.Parse(inToValidate[i].ToString()) * 2);
                }

                int CheckSum = 0;    // Will hold the total sum of all checksum digits

                // Second stage, add separate digits of all products
                for (int iCount = 0; iCount <= CheckNumbers.Count - 1; iCount++)
                {
                    int _count = 0;    // will hold the sum of the digits

                    // determine if current number has more than one digit
                    if ((int)CheckNumbers[iCount] > 9)
                    {
                        int _numLength = ((int)CheckNumbers[iCount]).ToString().Length;
                        // add count to each digit
                        for (int x = 0; x < _numLength; x++)
                        {
                            _count = _count + Int32.Parse(
                                ((int)CheckNumbers[iCount]).ToString()[x].ToString());
                        }
                    }
                    else
                    {
                        // single digit, just add it by itself
                        _count = (int)CheckNumbers[iCount];
                    }
                    CheckSum = CheckSum + _count;    // add sum to the total sum
                }
                // Stage 3, add the unaffected digits
                // Add all the digits that we didn't double still starting from the
                // right but this time we'll start from the rightmost number with 
                // alternating digits
                int OriginalSum = 0;
                for (int y = CardLength - 1; y >= 0; y = y - 2)
                {
                    OriginalSum = OriginalSum + Int32.Parse(inToValidate[y].ToString());
                }

                // Perform the final calculation, if the sum Mod 10 results in 0 then
                // it's valid, otherwise its false.
                return (((OriginalSum + CheckSum) % 10) == 0);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether or not the given credit card number is a valid type
        /// (of the ones we accept).  Types allowed: Visa, MasterCard, Discover, Amex.
        /// Actual code was found at: 
        /// http://www.codeproject.com/aspnet/creditcardvalidator.asp
        /// </summary>
        /// <param name="inToValidate">Credit Card Number to validate.</param>
        /// <returns>If the credit card type is one we accept, return true, 
        /// otherwise, return false.</returns>
        static public bool IsValidCreditCardType(string inToValidate)
        {
            // AMEX -- 34 or 37 -- 15 length
            if ((Regex.IsMatch(inToValidate, "^(34|37)")))
                return (15 == inToValidate.Length);

                // MasterCard -- 51 through 55 -- 16 length
            else if ((Regex.IsMatch(inToValidate, "^(51|52|53|54|55)")))
                return (16 == inToValidate.Length);

                // VISA -- 4 -- 13 and 16 length
            else if ((Regex.IsMatch(inToValidate, "^(4)")))
                return (13 == inToValidate.Length || 16 == inToValidate.Length);

                /*
                // Diners Club -- 300-305, 36 or 38 -- 14 length
            else if ( (Regex.IsMatch(inToValidate,"^(300|301|302|303|304|305|36|38)")) && 
                ((_cardTypes & CardType.DinersClub)!=0) )
                return (14==inToValidate.Length);
                */

                /*
                // enRoute -- 2014,2149 -- 15 length
            else if ( (Regex.IsMatch(inToValidate,"^(2014|2149)")) && 
                ((_cardTypes & CardType.DinersClub)!=0) )
                return (15==inToValidate.Length);
                */

                // Discover -- 6011 -- 16 length
            else if ((Regex.IsMatch(inToValidate, "^(6011)")))
                return (16 == inToValidate.Length);

                /*
                // JCB -- 3 -- 16 length
            else if ( (Regex.IsMatch(inToValidate,"^(3)")) && 
                ((_cardTypes & CardType.JCB)!=0) )
                return (16==inToValidate.Length);
                */

                /*
                // JCB -- 2131, 1800 -- 15 length
            else if ( (Regex.IsMatch(inToValidate,"^(2131|1800)")) && 
                ((_cardTypes & CardType.JCB)!=0) )
                return (15==inToValidate.Length);
                */
            else
            {
                return false;
            }
        }



        //TODO: document
        static public string GetCreditCardType(string inCCNumber)
        {
            // AMEX -- 34 or 37 -- 15 length
            if ((Regex.IsMatch(inCCNumber, "^(34|37)")))
                return "AE";

            // MasterCard -- 51 through 55 -- 16 length
            else if ((Regex.IsMatch(inCCNumber, "^(51|52|53|54|55)")))
                return "MC";

            // VISA -- 4 -- 13 and 16 length
            else if ((Regex.IsMatch(inCCNumber, "^(4)")))
                return "VI";

            // Discover -- 6011 -- 16 length
            else if ((Regex.IsMatch(inCCNumber, "^(6011)")))
                return "NS";

            else
            {
                return "";
            }
        }

        static public bool IsValidEmailAddress(string inEmailAddress)
        {
            Regex regexIsValidEmailAddress = new Regex(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

            bool toReturn = regexIsValidEmailAddress.IsMatch(inEmailAddress);

            return toReturn;
        }
    }
}