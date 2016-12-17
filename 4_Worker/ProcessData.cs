using System.Collections.Generic;

class ProcessData
{
    /// <summary>
    /// GetPrimeFactors generates a list of prime numbers whose product is the number passed into the method.
    /// Two is the first prime number.  We start with the first prime number
    /// and keep incrementing by 1, and testing to see if the number modulus divisor is zero.
    /// When number % divisor == 0, this divisor a prime number.
    /// We store the div, and reduce the number by dividing number/divisor to keep walking down
    /// the tree.
    /// https://en.wikipedia.org/wiki/Prime_factor
    /// </summary>
    /// <param name="number">Interger you want a list of prime factors for.</param>
    /// <returns></returns>
    static public List<int> GetPrimeFactors(int number)
    {
        var primeFactors = new List<int>();
        for (int divisor = 2; divisor <= number; divisor++)
        {
            while (number % divisor == 0)
            {
                primeFactors.Add(divisor);
                number = number / divisor;
            }
        }
        return primeFactors;
    }
}
