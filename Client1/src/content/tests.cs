static void RunTests()
{
   string fn = nameof(Double);
   Action<int, int> test = (input, output) =>
   {
        var n = Double(input);
        AssertTrue(fn, "", n == output, $" Expected: {output} Actual: {n}");
   };
   test(3, 9);
   AllTestsPassed();
}