﻿namespace MyGardenPatch.Tests.TestData;

internal static class UserTestData
{
    public const string UnknownUserId = "{B0129B1B-AE17-4BB3-BAFC-45448D687EF5}";

    public static Gardener UnregisteredUser => new Gardener("John Doe", UnregisteredEmailAddress);

    public const string UnregisteredEmailAddress = "john.doe@email.com";

    /// <summary>
    /// A registered user id
    /// </summary>
    public const string PeterParkerUserId = "{DE1E23FC-9710-4889-A9C1-04FC1265B273}";

    public const string PeterParkerEmailAddress = "peter.parker@email.com";

    /// <summary>
    /// A registered user
    /// </summary>
    public static Gardener PeterParker => new Gardener(
        new Guid(PeterParkerUserId),
        "Peter Parker",
        PeterParkerEmailAddress,
        new DateTime(2022, 1, 1),
        true);
}
