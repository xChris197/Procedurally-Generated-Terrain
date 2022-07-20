using System.Collections.Generic;

public static class SCR_FactorsFinder
{
    /// <summary>
    /// Finds and return a list of all factors of a number passed in
    /// </summary>
    /// <param name="num">The number that all factors will be found</param>
    /// <returns>A list containing all factors of the number passed in</returns>
    public static List<int> FindAllFactorsOfANum(int num)
    {
        List<int> propFactors = new List<int>();
        for (int i = 1; i <= num; i++)
        {
            if (num % i == 0)
            {
                propFactors.Add(i);
            }
        }

        return propFactors;
    }
}
