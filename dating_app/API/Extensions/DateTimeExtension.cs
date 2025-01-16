using System;

namespace API.Extensions;

public static class DateTimeExtension
{
public static int CalculateAge (this DateOnly dat)
{
    var today = DateOnly.FromDateTime (DateTime.Now);
    var age = today.Year - dat.Year;

    if (dat > today.AddYears(-age)) age--;

    return age;
}

}
