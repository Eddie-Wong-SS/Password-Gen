using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Class for generating a secure password with high entropy
//Code adapted and modified from https://stackoverflow.com/a/26406613
public class PasswordGenerator
{
    #region Data Types
    public static string LowerCaseChars;
    public static string UpperCaseChars;
    public static string NumChars;
    public static string SpecialChars;

    private readonly RandomSecureVersion _randomSecure = new RandomSecureVersion();

    #endregion

    #region Constructors
    static PasswordGenerator()
    {
        // Define characters that are valid for each group
        //Use GetCharRange(minimum, maximum, exclusiveChars: exclusivechars) for adding into characters to be excluded
        LowerCaseChars = GetCharRange('a', 'z');
        UpperCaseChars = GetCharRange('A', 'Z');
        NumChars = GetCharRange('0', '9');
        SpecialChars = "!@#%*()$?+-=";
    }
    #endregion

    #region Public Functions

    public string Generate(int length, bool hasUpper, bool hasNum, bool hasSymbol)
    {
        var password = LowerCaseChars;
        //Adds additional characters only if their checkboxes in MainWindow is checked
        if (hasUpper) password += UpperCaseChars;
        if (hasNum) password += NumChars;
        if (hasSymbol) password += SpecialChars;

        var unshuffledResult = GetRandomString(password, length);

        // Shuffle the results so the order of the characters are unpredictable
        var result = unshuffledResult.ShuffleTextSecure();

        return result;
    }
    #endregion

    #region Private Functions
    //Randomises a string of the specified length from available characters and a specified string length
    //Note that this string is not truly secure and should not be used as a password on its own
    private string GetRandomString(string possibleChars, int length)
    {
        var result = string.Empty;
        for (var position = 0; position < length; position++)
        {
            var index = _randomSecure.Next(possibleChars.Length);
            result += possibleChars[index];
        }
        return result;
    }

    //Functionality to set the character ranges without needing to specify each individual character
    //Has functionality to exclude certain characters(Not currently used)
    private static string GetCharRange(char minimum, char maximum, string exclusiveChars = "")
    {
        var result = string.Empty;
        for (var value = minimum; value <= maximum; value++)
        {
            result += value;
        }
        //Excludes any characters stated to be excluded from the string
        if (!string.IsNullOrEmpty(exclusiveChars))
        {
            var inclusiveChars = result.Except(exclusiveChars).ToArray();
            //Converts the character array into a string
            result = new string(inclusiveChars);
        }
        return result;
    }
    #endregion
}

#region Internal Classes
//Provides functionality for shuffling the inputted string while ensuring high entropy
internal static class Extensions
{
    //Lazy<T> is used to defer execution of resource intensive/large objects until needed
    //Only creates one object instance no matter how many threads request initialization
    //https://learn.microsoft.com/en-us/dotnet/api/system.lazy-1?view=net-7.0
    #region Data Type
    private static readonly Lazy<RandomSecureVersion> RandomSecure =
        new Lazy<RandomSecureVersion>(() => new RandomSecureVersion());
    #endregion

    #region Functions
    public static IEnumerable<T> ShuffleSecure<T>(this IEnumerable<T> source)
    {
        var sourceArray = source.ToArray();
        for (int counter = 0; counter < sourceArray.Length; counter++)
        {
            int randomIndex = RandomSecure.Value.Next(counter, sourceArray.Length);
            yield return sourceArray[randomIndex];

            sourceArray[randomIndex] = sourceArray[counter];
        }
    }

    public static string ShuffleTextSecure(this string source)
    {
        var shuffledChars = source.ShuffleSecure().ToArray();
        return new string(shuffledChars);
    }
    #endregion
}

//Generates a randomized version of a string that is highly randomized and has high entropy
internal class RandomSecureVersion
{
    //Random() is not advisable for use in password generation due to being psuedo-random: https://stackify.com/csharp-random-numbers/
    //RNGCryptoServiceProvider() is the recommended method for generating high entropy passwords due to being secure random
    #region Data Type
    private readonly RNGCryptoServiceProvider _rngProvider = new RNGCryptoServiceProvider();
    #endregion

    #region Function 
    public int Next()
    {
        var randomBuffer = new byte[4];
        _rngProvider.GetBytes(randomBuffer);

        var result = BitConverter.ToInt32(randomBuffer, 0);
        return result;
    }

    public int Next(int maximumValue)
    {
        // Return a range instead of Next() % maximumValue to ensure secure distribution of string elements
        return Next(0, maximumValue);
    }

    public int Next(int minimumValue, int maximumValue)
    {
        var seed = Next();

        //  Generate uniformly distributed random integers within a given range.
        return new Random(seed).Next(minimumValue, maximumValue);
    }
    #endregion
}
#endregion
