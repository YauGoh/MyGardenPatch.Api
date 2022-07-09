using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGardenPatch.Users.Exceptions;

public class UserNotAuthenticatedException : Exception
{
    public override string Message => "User not authenticated";
}
