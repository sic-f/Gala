using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;

namespace Ui.Services;

public class ErrorsParser
{
    public ModelStateDictionary Errors { get; } = new();
    public int ErrorCount => Errors.ErrorCount;

    public ErrorsParser ParseErrors(IdentityResult from)
    {
        if (NoErrors(from)) return this;

        foreach (IdentityError error in from.Errors)
        {
            Errors.AddModelError(error.Code, error.Description);
        }

        return this;
    }

    private static bool NoErrors(IdentityResult from)
    {
        ICollection errors = (ICollection)from.Errors;

        return errors.Count.Equals(0);
    }
}