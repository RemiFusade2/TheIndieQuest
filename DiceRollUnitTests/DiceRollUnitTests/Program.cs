using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DiceRollUnitTests
{
    class DiceRollUnitTests
    {
        // Test colors
        static ConsoleColor testTitleColor = ConsoleColor.Yellow;

        static ConsoleColor defaultColor = ConsoleColor.White;

        static ConsoleColor passedColor = ConsoleColor.DarkGreen;
        static ConsoleColor failedColor = ConsoleColor.Red;
        static ConsoleColor warningColor = ConsoleColor.DarkYellow;

        static ConsoleColor importantInfoColor = ConsoleColor.Cyan;

        /**
         * Roll X  N-faced dice and add a fixed bonus to the result.
         * */
        static int DiceRoll(int numberOfDice, int numberOfFacesOnADie, int fixedBonus)
        {
            // Well, you actually have to implement this method.
            throw new Exception("Method not implemented");
        }

        /**
         * Display the result of a test.
         * Color depends on the result (passed, failed, or warning)
         * */
        static void DisplayTestResult(bool testFailed, bool warning, int cursorCurrentPosition, int cursorFinalPosition)
        {
            string testPassedStr = "";
            if (testFailed)
            {
                Console.ForegroundColor = failedColor;
                testPassedStr = "> test failed!";
            }
            else if (warning)
            {
                Console.ForegroundColor = warningColor;
                testPassedStr = "> test passed?";
            }
            else
            {
                Console.ForegroundColor = passedColor;
                testPassedStr = "> test passed!";
            }
            
            Console.Write(" ");
            for (int i = cursorCurrentPosition; i < cursorFinalPosition; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine(testPassedStr);
        }

        /** This method will take a dice notation as a parameter and call the method DiceRoll() with the values from the dice notation.
         * Then it will compare the actual results with the expected results for this roll. 
         * */
        static bool TestDiceRoll(string diceNotation)
        {
            // Parse values from diceNotation
            Match match = Regex.Match(diceNotation, @"(\d*)d(\d+)(?:\+(\d+))?");
            int numberOfDice = 1;
            if (int.TryParse(match.Groups[1].Value, out int parsedNumberOfDiceValue))
            {
                numberOfDice = parsedNumberOfDiceValue;
            }

            int numberOfFacesOnADie = 1;
            if (int.TryParse(match.Groups[2].Value, out int parsedNumberOfFacesValue))
            {
                numberOfFacesOnADie = parsedNumberOfFacesValue;
            }

            int fixedBonus = 0;
            if (int.TryParse(match.Groups[3].Value, out int parsedFixedBonusValue))
            {
                fixedBonus = parsedFixedBonusValue;
            }

            // Settings (number of calls to TestDiceRoll())
            int numberOfTriesToDisplay = 20;
            int numberOfTriesToEstimateAverageAndStandardDeviation = 1000;
            int cursorMinFinalPosition = 110;

            // Cursor
            int cursorPosition = 0;

            // Theoretical values
            int expectedRangeLow = numberOfDice + fixedBonus;
            int expectedRangeHigh = numberOfDice * numberOfFacesOnADie  + fixedBonus;
            double theoreticalAverage = (numberOfDice * (1 + numberOfFacesOnADie) / 2.0 + fixedBonus);

            // Title for this test
            Console.ForegroundColor = testTitleColor;
            Console.Write($"*** Testing {diceNotation} (expected results range is [");
            Console.ForegroundColor = importantInfoColor;
            Console.Write($"{expectedRangeLow}..{expectedRangeHigh}");
            Console.ForegroundColor = testTitleColor;
            Console.WriteLine("]) ***");

            // First rolls, we check that no result was out of range
            bool testRangePassed = true;
            Console.ForegroundColor = defaultColor;
            Console.Write($"Actual results from {numberOfTriesToDisplay} rolls: ");
            cursorPosition += $"Actual results from {numberOfTriesToDisplay} rolls: ".Length;
            for (int i = 1; i <= numberOfTriesToDisplay; i++)
            {
                int result = DiceRoll(numberOfDice, numberOfFacesOnADie, fixedBonus);
                if (result >= expectedRangeLow && result <= expectedRangeHigh)
                {
                    // test passed
                    Console.ForegroundColor = importantInfoColor;
                    Console.Write(result.ToString());
                }
                else
                {
                    // test failed
                    testRangePassed = false;
                    Console.ForegroundColor = failedColor;
                    Console.Write(result.ToString());
                }
                cursorPosition += result.ToString().Length;

                if (i < numberOfTriesToDisplay)
                {
                    Console.ForegroundColor = defaultColor;
                    Console.Write(", ");
                    cursorPosition += ", ".Length;
                }

                if (cursorPosition >= cursorMinFinalPosition-6)
                {
                    Console.Write("...");
                    cursorPosition += "...".Length;
                    break;
                }
            }

            DisplayTestResult(!testRangePassed, false, cursorPosition, cursorMinFinalPosition);

            // Roll more and store them in a list
            List<int> rolls = new List<int>();
            for (int i = 1; i <= numberOfTriesToEstimateAverageAndStandardDeviation; i++)
            {
                rolls.Add(DiceRoll(numberOfDice, numberOfFacesOnADie, fixedBonus));
            }
            
            // Compute actual average out of lots of rolls
            int sum = 0;
            foreach (int roll in rolls)
            {
                sum += roll;
            }
            double practicalAverage = (1.0 * sum / rolls.Count);
            double averageErrorMargin = 1000.0 / numberOfTriesToEstimateAverageAndStandardDeviation;

            double actualDifferentBetweenTheoreticalAndPracticalAverage = (Math.Abs(practicalAverage - theoreticalAverage)) / theoreticalAverage;
            bool testAveragePassed = (actualDifferentBetweenTheoreticalAndPracticalAverage < averageErrorMargin);
            cursorPosition = 0;

            Console.ForegroundColor = defaultColor;
            Console.Write("Expected average is ");
            cursorPosition += "Expected average is ".Length;
            Console.ForegroundColor = importantInfoColor;
            Console.Write($"{theoreticalAverage}");
            cursorPosition += $"{theoreticalAverage}".Length;
            Console.ForegroundColor = defaultColor;
            Console.Write($". Out of {numberOfTriesToEstimateAverageAndStandardDeviation} rolls, the average was ");
            cursorPosition += $". Out of {numberOfTriesToEstimateAverageAndStandardDeviation} rolls, the average was ".Length;
            if (testAveragePassed)
            {
                Console.ForegroundColor = importantInfoColor;
                Console.Write($"{practicalAverage}");
            }
            else
            {
                Console.ForegroundColor = failedColor;
                Console.Write($"{practicalAverage}");
            }
            cursorPosition += $"{practicalAverage}".Length;

            DisplayTestResult(!testAveragePassed, false, cursorPosition, cursorMinFinalPosition);
            
            // Compute variance
            double theoreticalStandardDeviation = 0;
            double theoreticalVarianceForOneDie = 0;
            double theoreticalAverageForOneDie = (1 + numberOfFacesOnADie) / 2.0;
            for (int i = 1; i <= numberOfFacesOnADie; i++)
            {
                theoreticalVarianceForOneDie += Math.Pow(i - theoreticalAverageForOneDie, 2);
            }
            theoreticalVarianceForOneDie /= numberOfFacesOnADie;
            theoreticalStandardDeviation = Math.Sqrt(numberOfDice * theoreticalVarianceForOneDie);
                       
            double sumPracticalVariance = 0;
            foreach (int roll in rolls)
            {
                sumPracticalVariance += Math.Pow(roll - practicalAverage, 2);
            }
            double practicalStandardDeviation = Math.Sqrt(sumPracticalVariance / rolls.Count);

            double varianceWarningMargin = 0.05;
            double varianceErrorMargin = 0.1;
            double actualDifferenceBetweenTheoreticalAndPracticalVariance = (Math.Abs(practicalStandardDeviation - theoreticalStandardDeviation)) / theoreticalStandardDeviation;
            if (theoreticalStandardDeviation == 0)
            {
                actualDifferenceBetweenTheoreticalAndPracticalVariance = Math.Abs(practicalStandardDeviation - theoreticalStandardDeviation);
            }
            bool testVarianceFailed = (actualDifferenceBetweenTheoreticalAndPracticalVariance >= varianceErrorMargin);
            bool testVarianceWarning = (actualDifferenceBetweenTheoreticalAndPracticalVariance >= varianceWarningMargin);
            cursorPosition = 0;
            Console.ForegroundColor = defaultColor;
            Console.Write("Expected standard deviation is ");
            cursorPosition += "Expected standard deviation is ".Length;
            Console.ForegroundColor = importantInfoColor;
            Console.Write(theoreticalStandardDeviation.ToString("0.00"));
            cursorPosition += theoreticalStandardDeviation.ToString("0.00").Length;
            Console.ForegroundColor = defaultColor;
            Console.Write($". The actual standard deviation on {numberOfTriesToEstimateAverageAndStandardDeviation} rolls was ");
            cursorPosition += $". The actual standard deviation on {numberOfTriesToEstimateAverageAndStandardDeviation} rolls was ".Length;
            if (testVarianceFailed)
            {
                Console.ForegroundColor = failedColor;
                Console.Write(practicalStandardDeviation.ToString("0.00"));
                Console.ForegroundColor = defaultColor;
                Console.Write(" (off by ");
                Console.ForegroundColor = failedColor;
                Console.Write(actualDifferenceBetweenTheoreticalAndPracticalVariance.ToString("0.0%"));
                Console.ForegroundColor = defaultColor;
                Console.Write(")");
            }
            else if (testVarianceWarning)
            {
                // barely passed
                Console.ForegroundColor = warningColor;
                Console.Write(practicalStandardDeviation.ToString("0.00"));
                Console.ForegroundColor = defaultColor;
                Console.Write(" (off by ");
                Console.ForegroundColor = warningColor;
                Console.Write(actualDifferenceBetweenTheoreticalAndPracticalVariance.ToString("0.0%"));
                Console.ForegroundColor = defaultColor;
                Console.Write(")");
            }
            else
            {
                // passed
                Console.ForegroundColor = importantInfoColor;
                Console.Write(practicalStandardDeviation.ToString("0.00"));
                Console.ForegroundColor = defaultColor;
                Console.Write(" (off by ");
                Console.ForegroundColor = importantInfoColor;
                Console.Write(actualDifferenceBetweenTheoreticalAndPracticalVariance.ToString("0.0%"));
                Console.ForegroundColor = defaultColor;
                Console.Write(")");
            }
            cursorPosition += practicalStandardDeviation.ToString("0.00").Length + " (off by ".Length + actualDifferenceBetweenTheoreticalAndPracticalVariance.ToString("0.0%").Length + ")".Length;

            DisplayTestResult(testVarianceFailed, testVarianceWarning, cursorPosition, cursorMinFinalPosition);

            Console.ForegroundColor = defaultColor;
            Console.WriteLine("");

            return testRangePassed && testAveragePassed && !testVarianceFailed;
        }

        /**
         * Here's the Main method.
         * It will run multiple tests on DiceRoll()
         * */
        static void Main(string[] args)
        {
            Console.ForegroundColor = defaultColor;
            Console.WriteLine("Welcome to our test program.");
            Console.WriteLine("This program will test your method DiceRoll() by comparing the expected results with the actual results.");
            Console.WriteLine("");

            bool allTestPassed = true;
            allTestPassed &= TestDiceRoll("d6");
            allTestPassed &= TestDiceRoll("2d6");
            allTestPassed &= TestDiceRoll("d1+100");
            allTestPassed &= TestDiceRoll("5d8+12");
            allTestPassed &= TestDiceRoll("2d10");
            allTestPassed &= TestDiceRoll("15d4+20");

            if (allTestPassed)
            {
                // Display a trophy
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("              .-=========-.\n              \\'-=======-'/\n              _|   .=.   |_\n             ((|  {{1}}  |))\n              \\|   /|\\   |/\n               \\__ '`' __/\n                 _`) (`_\n               _/_______\\_\n              /___________\\\n             CONGRATULATIONS");
            }
            else
            {
                // Display a skull
                Console.ForegroundColor = failedColor;
                Console.WriteLine("                       ______\n                    .-\"      \"-.\n                   /            \\\n       _          |              |          _\n      ( \\         |,  .-.  .-.  ,|         / )\n       > \"=._     | )(__/  \\__)( |     _.=\" <\n      (_/\"=._\"=._ |/     /\\     \\| _.=\"_.=\"\\_)\n             \"=._ (_     ^^     _)\"_.=\"\n                 \"=\\__|IIIIII|__/=\"\n                _.=\"| \\IIIIII/ |\"=._\n      _     _.=\"_.=\"\\          /\"=._\"=._     _\n     ( \\_.=\"_.=\"     `--------`     \"=._\"=._/ )\n      > _.=\"                            \"=._ <\n     (_/                                    \\_)");

            }

            Console.ForegroundColor = defaultColor;
            Console.WriteLine("");
            Console.WriteLine("Press any key to close the program.");
            Console.ReadLine();
        }
    }
}
