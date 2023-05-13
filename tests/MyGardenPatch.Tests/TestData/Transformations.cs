namespace MyGardenPatch.Tests.TestData;

internal static class Transformations
{
    internal const string Identity = @"1 0 0
                                       0 1 0
                                       0 0 1";

    internal const string Move1y = @"1 0 0
                                     0 1 0
                                     0 1 1";

    internal const string Move1x = @"1 0 0
                                     0 1 0
                                     1 0 1";

    internal const string Move2_3 = @"1 0 0
                                      0 1 0
                                      2 3 1";

    internal const string RotateLeft90 = @"0 -1  0
                                           1  0  0
                                           0  0  1";
}
